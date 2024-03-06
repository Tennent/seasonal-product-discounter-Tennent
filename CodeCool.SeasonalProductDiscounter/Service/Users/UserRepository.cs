using System.Data;
using System.Runtime.InteropServices;
using CodeCool.SeasonalProductDiscounter.Model.Users;
using CodeCool.SeasonalProductDiscounter.Service.Logger;
using CodeCool.SeasonalProductDiscounter.Service.Persistence;

namespace CodeCool.SeasonalProductDiscounter.Service.Users;

public class UserRepository : SqLiteConnector, IUserRepository
{
    private readonly string _tableName;

    public UserRepository(string dbFile, ILogger logger) : base(dbFile, logger)
    {
        _tableName = DatabaseManager.UsersTableName;
    }

    public IEnumerable<User> GetAll()
    {
        var query = @$"SELECT * FROM {_tableName}";
        var ret = new List<User>();

        try
        {
            using var connection = GetPhysicalDbConnection();
            using var command = GetCommand(query, connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var user = new User(reader.GetInt32(0), reader.GetString(1), reader.GetString(2));
                ret.Add(user);
            }
        }
        catch (Exception e)
        {
            Logger.LogError(e.Message);
            throw;
        }

        return ret;
    }

    public bool Add(User user)
    {
        var query = @$"INSERT INTO {_tableName} (id, user_name, password)
                    VALUES ({user.Id}, '{user.UserName}', '{user.Password}')";
        
        return ExecuteNonQuery(query);
    }

    public User Get(string name)
    {
        var query = @$"SELECT * FROM {_tableName}
                    WHERE user_name = {name}";

        User ret = null;

        try
        {
            using var connection = GetPhysicalDbConnection();
            using var command = GetCommand(query, connection);
            using var reader = command.ExecuteReader();
            Logger.LogInfo($"{GetType().Name} executing query: {query}");

            while (reader.Read())
            {
                var user = new User
                (
                    reader.GetInt32(reader.GetOrdinal("id")),
                    reader.GetString(reader.GetOrdinal("user_name")),
                    reader.GetString(reader.GetOrdinal("password"))
                );

                ret = user;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error retrieving user: {name}!\n" + e.Message);
            throw;
        }

        return ret;
    }
}