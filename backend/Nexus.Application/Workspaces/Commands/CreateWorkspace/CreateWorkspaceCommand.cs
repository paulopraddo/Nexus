using Nexus.Application.Common;
using FluentResults;
using MediatR;

namespace Nexus.Application.Workspaces.Commands.CreateWorkspace;

public sealed record CreateWorkspaceCommand(Guid OwnerId, string Name) : IRequest<Result<WorkspaceResult>>;
