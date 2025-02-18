namespace Insof.AppReporting.Web.DataAccess.Records;

public sealed class UserRecord
{
    public required int UserId { get; init; }
    public required string Username { get; init; }
    public required string Name { get; init; }
    public required string Secret { get; init; }
}