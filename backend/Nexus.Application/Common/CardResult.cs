namespace Nexus.Application.Common;

public sealed record CardResult(Guid Id, string Title, Guid BoardId, DateTime CreatedAt);
