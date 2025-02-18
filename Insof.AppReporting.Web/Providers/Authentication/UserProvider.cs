using System;
using System.Linq;
using System.Threading.Tasks;
using Insof.AppReporting.Web.DataAccess.Records;
using Insof.AppReporting.Web.DataAccess.Repositories;
using Insof.AppReporting.Web.Models.Authentication;
using Insof.AppReporting.Web.Models.Exceptions;
using Microsoft.Extensions.Caching.Hybrid;

namespace Insof.AppReporting.Web.Providers.Authentication;

public interface IUserProvider
{
    Task<InsofUser> AuthenticateAsync(string username, string password);
    Task CreateUserAsync(string username, string name, string password);
    Task<InsofUser> GetFromPersistentIdentityAsync(PersistentIdentity identity);
}

public sealed class UserProvider : IUserProvider
{
    private readonly IUserRepository _userRepository;
    private readonly ISecretHasher _secretHasher;
    private readonly HybridCache _cache;

    public UserProvider(IUserRepository userRepository,
        ISecretHasher secretHasher,
        HybridCache cache)
    {
        _userRepository = userRepository;
        _secretHasher = secretHasher;
        _cache = cache;
    }

    public async Task<InsofUser> AuthenticateAsync(string username, string password)
    {
        var record = await _userRepository.GetUserByUsernameAsync(username);

        if (record == null)
            throw new InvalidLoginException();

        if (!_secretHasher.Verify(password, record.Secret))
            throw new InvalidLoginException();

        var roleRecords = await _userRepository.GetRolesByUserIdAsync(record.UserId);
        var roles = roleRecords.Select(r => new Role
        {
            Name = r.RoleName
        }).ToList();

        return new InsofUser
        {
            Username = record.Username,
            Name = record.Name,
            UserId = record.UserId,
            Roles = roles,
            Secret = record.Secret
        };
    }

    public async Task CreateUserAsync(string username, string name, string password)
    {
        var secret = _secretHasher.Hash(password);
        var userId = await _userRepository.CreateUserAsync(new UserRecord
        {
            Username = username,
            Name = name,
            Secret = secret,
            UserId = -1
        });
    }

    public async Task<InsofUser> GetFromPersistentIdentityAsync(PersistentIdentity identity)
    {
        if (identity.Expiration < DateTime.Now)
            return InsofUser.Empty;

        var cacheKey = $"{nameof(UserProvider)}:{identity.CacheKeySuffix}";

        var result = await _cache.GetOrCreateAsync(cacheKey, async _ 
            => await GetFromPersistentIdentityUncachedAsync(identity));

        return result;
    }

    private async Task<InsofUser> GetFromPersistentIdentityUncachedAsync(PersistentIdentity identity)
    {
        var record = await _userRepository.GetUserByIdAndSecretAsync(identity.UserId, identity.Secret);

        if (record == null)
            return InsofUser.Empty;

        var roleRecords = await _userRepository.GetRolesByUserIdAsync(record.UserId);
        var roles = roleRecords.Select(r => new Role
        {
            Name = r.RoleName
        }).ToList();

        return new InsofUser
        {
            Name = record.Name,
            UserId = record.UserId,
            Secret = record.Secret,
            Username = record.Username,
            Roles = roles
        };
    }
}
