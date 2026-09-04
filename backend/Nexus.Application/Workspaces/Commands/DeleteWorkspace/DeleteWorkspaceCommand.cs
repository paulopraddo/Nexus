using FluentResults;
using MediatR;

namespace Nexus.Application.Workspaces.Commands.DeleteWorkspace;

public sealed record DeleteWorkspaceCommand(Guid WorkspaceId, Guid CurrentUserId) : IRequest<Result>;
