using System;

namespace Insof.AppReporting.Web.Models.Authentication;

public sealed class PersistentIdentity
{
    public required int UserId { get; init; }
    public required string Secret { get; init; }
    public required DateTime Expiration { get; init; }

    internal string CacheKeySuffix => $"{UserId}:{Secret.GetHashCode()}";
}
