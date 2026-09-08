namespace Nexus.Application.Common;

public sealed record WorkspaceResult(Guid Id, string Name, Guid OwnerId, DateTime CreatedAt);
