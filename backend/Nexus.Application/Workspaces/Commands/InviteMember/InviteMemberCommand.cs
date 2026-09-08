using Nexus.Application.Common;
using FluentResults;
using MediatR;

namespace Nexus.Application.Workspaces.Commands.InviteMember;

public sealed record InviteMemberCommand(Guid WorkspaceId, Guid CurrentUserId, string Email)
    : IRequest<Result<WorkspaceMemberResult>>;
