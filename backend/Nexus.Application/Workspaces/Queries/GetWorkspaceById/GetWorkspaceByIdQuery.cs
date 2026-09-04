using Nexus.Application.Common;
using FluentResults;
using MediatR;

namespace Nexus.Application.Workspaces.Queries.GetWorkspaceById;

public sealed record GetWorkspaceByIdQuery(Guid WorkspaceId, Guid CurrentUserId) : IRequest<Result<WorkspaceResult>>;
