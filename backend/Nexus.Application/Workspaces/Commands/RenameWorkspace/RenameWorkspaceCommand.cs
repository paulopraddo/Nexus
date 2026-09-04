using Nexus.Application.Common;
using FluentResults;
using MediatR;

namespace Nexus.Application.Workspaces.Commands.RenameWorkspace;

public sealed record RenameWorkspaceCommand(Guid WorkspaceId, Guid CurrentUserId, string Name)
    : IRequest<Result<WorkspaceResult>>;
