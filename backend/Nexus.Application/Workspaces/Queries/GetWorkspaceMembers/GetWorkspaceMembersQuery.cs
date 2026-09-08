using Nexus.Application.Common;
using FluentResults;
using MediatR;

namespace Nexus.Application.Workspaces.Queries.GetWorkspaceMembers;

public sealed record GetWorkspaceMembersQuery(Guid WorkspaceId, Guid CurrentUserId)
    : IRequest<Result<IReadOnlyCollection<WorkspaceMemberResult>>>;
