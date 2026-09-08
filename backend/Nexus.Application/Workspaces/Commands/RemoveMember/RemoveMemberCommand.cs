using FluentResults;
using MediatR;

namespace Nexus.Application.Workspaces.Commands.RemoveMember;

public sealed record RemoveMemberCommand(Guid WorkspaceId, Guid CurrentUserId, Guid MemberUserId) : IRequest<Result>;
