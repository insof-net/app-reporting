using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Insof.AppReporting.Web.DataAccess.Records;

namespace Insof.AppReporting.Web.DataAccess.Repositories;

public interface IUserRepository
{
    Task<UserRecord?> GetUserByUsernameAsync(string username);    
    Task<List<RoleRecord>> GetRolesByUserIdAsync(int userId);
    Task<int> CreateUserAsync(UserRecord user);
    Task<UserRecord?> GetUserByIdAndSecretAsync(int userId, string secret);
}

public sealed class UserRepository : BaseRepository, IUserRepository
{
    public UserRepository(string connectionString) : base(connectionString)
    { }
    
    public async Task<UserRecord?> GetUserByUsernameAsync(string username)
    {
        using var connection = GetConnection();

        var result = await connection.QuerySingleOrDefaultAsync<UserRecord>(
            "GetUser",
            new { p_username = username },
            commandType: CommandType.StoredProcedure);

        return result;
    }

    public async Task<List<RoleRecord>> GetRolesByUserIdAsync(int userId)
    {
        using var connection = GetConnection();
        
        var results = await connection.QueryAsync<RoleRecord>(
            "GetUserRoles",
            new { p_userId = userId },
            commandType: CommandType.StoredProcedure);

        return results.AsList();
    }

    public async Task<int> CreateUserAsync(UserRecord user)
    {
        using var connection = GetConnection();
        
        return await connection.ExecuteScalarAsync<int>("CreateUser",
            new
            {
                p_username = user.Username,
                p_name = user.Name,
                p_secret = user.Secret
            },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<UserRecord?> GetUserByIdAndSecretAsync(int userId, string secret)
    {
        var connection = GetConnection();

        var result = await connection.QuerySingleOrDefaultAsync<UserRecord>(
            "GetUserByIdAndSecret",
            new { p_userId = userId, p_secret = secret },
            commandType: CommandType.StoredProcedure);
        
        return result;
    }
}