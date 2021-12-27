using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using MusicFlow.Entities;

namespace MusicFlow.Services
{
    public enum DBReturnStatus
    {
        SUCCESS,
        ALREADY_EXISTS,
        NOT_FOUND,
        WRONG_PASSWORD
    }
    public class DBResult<T>
    {
        public DBReturnStatus Status;
        public T Data;
        public DBResult(DBReturnStatus status, T data = default(T))
        {
            this.Status = status;
            this.Data = data;
        }
        public static DBResult<A> Success<A>(A data) => new DBResult<A>(DBReturnStatus.SUCCESS, data);
        public static DBResult<A> NotFound<A>() => new DBResult<A>(DBReturnStatus.NOT_FOUND);
        public static DBResult<A> WrongPassword<A>() => new DBResult<A>(DBReturnStatus.WRONG_PASSWORD);
    }
    public class Database
    {
        private SqlConnection connection;
        public Database(string connectionString)
        {
            connection = new SqlConnection(connectionString);
            connection.Open();
        }
        public async Task<DBReturnStatus> RegisterUser(User user)
        {
            SqlCommand command = new SqlCommand("INSERT INTO Users(email, username, password) OUTPUT inserted.id VALUES(@email, @username, @password)", connection);
            command.Parameters.AddWithValue("@email", user.Email);
            command.Parameters.AddWithValue("@username", user.Username);
            command.Parameters.AddWithValue("@password", user.Password);
            try
            {
                int result = (int)await command.ExecuteScalarAsync();
                user.Id = result;
                return DBReturnStatus.SUCCESS;
            }
            catch (SqlException e)
            {
                if (e.Number == 2627) // Violation in unique constraint error
                {
                    return DBReturnStatus.ALREADY_EXISTS;
                }
                else throw;
            }
        }
        public async Task<DBResult<User>> LoginUser(string email, byte[] hashedPassword)
        {
            SqlCommand command = new SqlCommand("SELECT id, password, username, role FROM Users WHERE email = @email", connection);
            command.Parameters.AddWithValue("@email", email);
            using (SqlDataReader reader = await command.ExecuteReaderAsync())
            {
                if (reader.HasRows)
                {
                    await reader.ReadAsync();
                    byte[] pwd = new byte[32];
                    reader.GetBytes(1, 0, pwd, 0, 32);
                    if (hashedPassword.SequenceEqual(pwd))
                    {
                        User user = new User
                        {
                            Id = reader.GetInt32(0),
                            Username = reader.GetString(2),
                            Role = (UserRole)reader.GetByte(3)
                        };
                        return DBResult<User>.Success(user);
                    }
                    return DBResult<User>.WrongPassword<User>();
                }
                return DBResult<User>.NotFound<User>();
            }
        }
        public async Task<bool> VerifyPasword(string id, byte[] hashedPassword)
        {
            SqlCommand command = new SqlCommand("SELECT password FROM Users WHERE id = @id", connection);
            command.Parameters.AddWithValue("@id", id);
            using (SqlDataReader reader = await command.ExecuteReaderAsync())
            {
                if (reader.HasRows)
                {
                    await reader.ReadAsync();
                    byte[] pwd = new byte[32];
                    reader.GetBytes(0, 0, pwd, 0, 32);
                    if (hashedPassword.SequenceEqual(pwd))
                    {
                        return true;
                    }
                }
                return false;
            }
        }
        public async Task<User> FetchUser(string id)
        {
            SqlCommand command = new SqlCommand("SELECT email, avatar FROM Users WHERE id = @id", connection);
            command.Parameters.AddWithValue("@id", id);
            using (SqlDataReader reader = await command.ExecuteReaderAsync())
            {
                if (reader.HasRows)
                {
                    await reader.ReadAsync();
                    return new User
                    {
                        Email = reader.GetString(0),
                        Avatar = reader.IsDBNull(1) ? null : reader.GetString(1)
                    };
                }
                else throw new Exception();
            }
        }
        async Task<bool> SetDBValue(string id, string key, object value)
        {
            SqlCommand command = new SqlCommand($"UPDATE Users SET {key} = @value OUTPUT @@ROWCOUNT WHERE id = @id", connection);
            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@value", value);
            try
            {
                int result = (int)await command.ExecuteScalarAsync();
                if (result == 1)
                {
                    return true;
                }
            }
            catch (SqlException) { }
            return false;
        }
        public Task<bool> ChangeEmail(string id, string newEmail) => SetDBValue(id, "email", newEmail);
        public Task<bool> ChangePassword(string id, byte[] newHashedPassword) => SetDBValue(id, "password", newHashedPassword);
        public Task<bool> SetAvatarValue(string id, string avatar) => SetDBValue(id, "avatar", avatar);
        public async Task<int> CreateForumThread(string oid, string topic)
        {
            SqlCommand command = new SqlCommand("INSERT INTO ForumThreads(topic, oid) OUTPUT inserted.tid VALUES(@topic, @oid)", connection);
            command.Parameters.AddWithValue("@topic", topic);
            command.Parameters.AddWithValue("@oid", oid);
            return (int)await command.ExecuteScalarAsync();
        }
        public async Task<DBResult<int>> GetForumMessageOwnerId(string id)
        {
            SqlCommand command = new SqlCommand("SELECT oid FROM ThreadsContents WHERE(id = @id)", connection);
            command.Parameters.AddWithValue("@id", id);
            try
            {
                int oid = (int)await command.ExecuteScalarAsync();
                return DBResult<int>.Success(oid);
            } catch (SqlException) { }
            return DBResult<int>.NotFound<int>();
        }
        public async Task<int> CreateThreadMessage(int tid, string oid, string content, int rid)
        {
            SqlCommand command = new SqlCommand("INSERT INTO ThreadsContents(tid, oid, content, rid) OUTPUT inserted.id VALUES(@tid, @oid, @content, @rid)", connection);
            command.Parameters.AddWithValue("@tid", tid);
            command.Parameters.AddWithValue("@oid", oid);
            command.Parameters.AddWithValue("@content", content);
            command.Parameters.AddWithValue("@rid", rid is 0 ? DBNull.Value : rid);
            return (int)await command.ExecuteScalarAsync();
        }
        public async Task<DBReturnStatus> DeleteForumMessage(int id, string oid)
        {
            SqlCommand command = new SqlCommand("DELETE ThreadsContents OUTPUT 1 WHERE (id = @id AND oid = @oid)", connection);
            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@oid", oid);
            if ((int)await command.ExecuteScalarAsync() is 1)
                return DBReturnStatus.SUCCESS;
            return DBReturnStatus.NOT_FOUND;
        }
        public async Task<DBReturnStatus> EditForumMessage(int id, string oid, string content)
        {
            SqlCommand command = new SqlCommand("UPDATE ThreadsContents SET content = @content OUTPUT 1 WHERE (id = @id AND oid = @oid)", connection);
            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@oid", oid);
            command.Parameters.AddWithValue("@content", content);
            try
            {
                if ((int)await command.ExecuteScalarAsync() is 1)
                    return DBReturnStatus.SUCCESS;
            }
            catch (SqlException) { }
            return DBReturnStatus.NOT_FOUND;
        }
        public async Task<List<ForumThread>> FetchForumThreads(int page)
        {
            SqlCommand command = new SqlCommand("SELECT tid, topic, oid, pinned, locked, username, avatar, email FROM ForumThreads INNER JOIN Users U on U.id = ForumThreads.oid ORDER BY tid OFFSET @offset ROWS FETCH NEXT 20 ROWS ONLY", connection);
            command.Parameters.AddWithValue("@offset", 20 * (page - 1));
            using (SqlDataReader reader = await command.ExecuteReaderAsync())
            {
                List<ForumThread> threads = new();
                while (reader.Read())
                {
                    try
                    {
                        threads.Add(new ForumThread
                        {
                            Tid = reader.GetInt32(0),
                            Topic = reader.GetString(1),
                            Oid = reader.GetInt32(2),
                            Pinned = reader.GetBoolean(3),
                            Locked = reader.GetBoolean(4),
                            Owner = new User
                            {
                                Username = reader.GetString(5),
                                Avatar = reader.IsDBNull(6) ? null : reader.GetString(6),
                                Email = reader.GetString(7)
                            }
                        });
                    } catch (SqlException)
                    {
                        threads.Add(new ForumThread { Topic = "Failed to preview thread" } );
                    }
                }
                return threads;
            }
        }
        public async Task<List<ForumContent>> FetchThreadContents(int tid, int page)
        {
            SqlCommand command = new SqlCommand("SELECT TC.id, oid, content, rid, username, avatar, email FROM ThreadsContents TC INNER JOIN Users U on U.id = TC.oid WHERE tid = @tid ORDER BY id OFFSET @offset ROWS FETCH NEXT 20 ROWS ONLY", connection);
            command.Parameters.AddWithValue("@tid", tid);
            command.Parameters.AddWithValue("@offset", 20 * (page - 1));
            using (SqlDataReader reader = await command.ExecuteReaderAsync())
            {
                List<ForumContent> contents = new();
                while (reader.Read())
                {
                    try
                    {
                        contents.Add(new ForumContent
                        {
                            Id = reader.GetInt32(0),
                            Oid = reader.GetInt32(1),
                            Content = reader.GetString(2),
                            Rid = reader.IsDBNull(3) ? 0 : reader.GetInt32(3),
                            Owner = new User
                            {
                                Username = reader.GetString(4),
                                Avatar = reader.IsDBNull(5) ? null : reader.GetString(5),
                                Email = reader.GetString(6)
                            }
                        });
                    }
                    catch (SqlException)
                    {
                        contents.Add(new ForumContent { Content = "Failed to preview message" });
                    }
                }
                return contents;
            }
        }
        public async Task<DBResult<ForumThread>> FetchForumThread(int tid)
        {
            SqlCommand command = new SqlCommand("SELECT topic, oid, pinned, locked, username, avatar, email FROM ForumThreads INNER JOIN Users U on U.id = ForumThreads.oid WHERE tid = @tid", connection);
            command.Parameters.AddWithValue("@tid", tid);
            using (SqlDataReader reader = await command.ExecuteReaderAsync())
            {
                if (reader.HasRows)
                {
                    try
                    {
                        await reader.ReadAsync();
                        return DBResult<ForumThread>.Success(new ForumThread
                        {
                            Topic = reader.GetString(0),
                            Oid = reader.GetInt32(1),
                            Pinned = reader.GetBoolean(2),
                            Locked = reader.GetBoolean(3),
                            Owner = new User
                            {
                                Username = reader.GetString(4),
                                Avatar = reader.IsDBNull(5) ? null : reader.GetString(5),
                                Email = reader.GetString(6)
                            }
                        });
                    } catch (SqlException) { }
                }
                return DBResult<ForumThread>.NotFound<ForumThread>();
            }
        }
    }
}
