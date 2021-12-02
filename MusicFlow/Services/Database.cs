using System;
using System.Collections.Generic;
using System.Data;
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
            SqlCommand command = new SqlCommand("SELECT id, password, username FROM Users WHERE email = @email", connection);
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
                            Username = reader.GetString(2)
                        };
                        return DBResult<User>.Success(user);
                    }
                    return DBResult<User>.WrongPassword<User>();
                }
                return DBResult<User>.NotFound<User>();
            }
        }
    }
}
