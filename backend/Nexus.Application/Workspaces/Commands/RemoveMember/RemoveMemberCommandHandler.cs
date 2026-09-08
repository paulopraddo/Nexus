using Nexus.Domain.Common;
using Nexus.Domain.Workspaces;
using FluentResults;
using MediatR;

namespace Nexus.Application.Workspaces.Commands.RemoveMember;

public sealed class RemoveMemberCommandHandler(
    IWorkspaceRepository workspaceRepository,
    IWorkspaceMemberRepository memberRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<RemoveMemberCommand, Result>
{
    public async Task<Result> Handle(RemoveMemberCommand request, CancellationToken cancellationToken)
    {
        var workspace = await workspaceRepository.GetByIdAsync(request.WorkspaceId, cancellationToken);

        if (workspace is null || !workspace.IsOwnedBy(request.CurrentUserId))
        {
            return Result.Fail("Workspace não encontrado.");
        }

        if (request.MemberUserId == workspace.OwnerId)
        {
            return Result.Fail("O dono do workspace não pode ser removido.");
        }

        var member = await memberRepository.GetAsync(workspace.Id, request.MemberUserId, cancellationToken);

        if (member is null)
        {
            return Result.Fail("Esse usuário não é membro do workspace.");
        }

        memberRepository.Remove(member);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}
