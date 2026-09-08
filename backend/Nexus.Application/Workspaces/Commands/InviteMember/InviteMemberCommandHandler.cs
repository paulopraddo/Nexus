using Nexus.Application.Common;
using Nexus.Domain.Common;
using Nexus.Domain.Users;
using Nexus.Domain.Workspaces;
using FluentResults;
using MediatR;

namespace Nexus.Application.Workspaces.Commands.InviteMember;

public sealed class InviteMemberCommandHandler(
    IWorkspaceRepository workspaceRepository,
    IWorkspaceMemberRepository memberRepository,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<InviteMemberCommand, Result<WorkspaceMemberResult>>
{
    public async Task<Result<WorkspaceMemberResult>> Handle(InviteMemberCommand request, CancellationToken cancellationToken)
    {
        var workspace = await workspaceRepository.GetByIdAsync(request.WorkspaceId, cancellationToken);

        if (workspace is null || !workspace.IsOwnedBy(request.CurrentUserId))
        {
            return Result.Fail<WorkspaceMemberResult>("Workspace não encontrado.");
        }

        var emailResult = Email.Create(request.Email);

        if (emailResult.IsFailed)
        {
            return Result.Fail<WorkspaceMemberResult>("Nenhum usuário encontrado com esse e-mail.");
        }

        var user = await userRepository.GetByEmailAsync(emailResult.Value, cancellationToken);

        if (user is null)
        {
            return Result.Fail<WorkspaceMemberResult>("Nenhum usuário encontrado com esse e-mail.");
        }

        if (user.Id == workspace.OwnerId || await memberRepository.ExistsAsync(workspace.Id, user.Id, cancellationToken))
        {
            return Result.Fail<WorkspaceMemberResult>("Esse usuário já faz parte do workspace.");
        }

        var member = WorkspaceMember.Create(workspace.Id, user.Id).Value;

        await memberRepository.AddAsync(member, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(new WorkspaceMemberResult(user.Id, user.Username.Value, user.Email.Value, member.CreatedAt));
    }
}
