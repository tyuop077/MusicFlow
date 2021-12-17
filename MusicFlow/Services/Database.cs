using System;
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
                int result = (int)(await command.ExecuteScalarAsync());
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
            catch { }
            return false;
        }
        public Task<bool> ChangeEmail(string id, string newEmail) => SetDBValue(id, "email", newEmail);
        public Task<bool> ChangePassword(string id, byte[] newHashedPassword) => SetDBValue(id, "password", newHashedPassword);
        public Task<bool> SetAvatarValue(string id, string avatar) => SetDBValue(id, "avatar", avatar);
    }
}
