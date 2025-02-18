using System.Data;
using MySqlConnector;

namespace Insof.AppReporting.Web.DataAccess.Repositories;

public abstract class BaseRepository
{
    private readonly string _connectionString;
    
    protected BaseRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    protected IDbConnection GetConnection()
    {
        return new MySqlConnection(_connectionString);
    }
}