namespace Nexus.Application.Common;

public sealed record BoardResult(Guid Id, string Name, Guid WorkspaceId, DateTime CreatedAt);
