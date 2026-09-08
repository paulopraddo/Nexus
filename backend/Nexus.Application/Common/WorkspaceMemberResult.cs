namespace Nexus.Application.Common;

public sealed record WorkspaceMemberResult(Guid UserId, string Username, string Email, DateTime JoinedAt);
