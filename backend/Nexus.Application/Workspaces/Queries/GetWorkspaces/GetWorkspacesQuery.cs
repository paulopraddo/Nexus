using Nexus.Application.Common;
using FluentResults;
using MediatR;

namespace Nexus.Application.Workspaces.Queries.GetWorkspaces;

public sealed record GetWorkspacesQuery(Guid CurrentUserId) : IRequest<Result<IReadOnlyCollection<WorkspaceResult>>>;
