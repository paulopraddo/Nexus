using Nexus.Application.Common;
using Nexus.Domain.Users;
using Nexus.Domain.Workspaces;
using FluentResults;
using MediatR;

namespace Nexus.Application.Workspaces.Queries.GetWorkspaceMembers;

public sealed class GetWorkspaceMembersQueryHandler(
    IWorkspaceRepository workspaceRepository,
    IWorkspaceMemberRepository memberRepository,
    IUserRepository userRepository)
    : IRequestHandler<GetWorkspaceMembersQuery, Result<IReadOnlyCollection<WorkspaceMemberResult>>>
{
    public async Task<Result<IReadOnlyCollection<WorkspaceMemberResult>>> Handle(
        GetWorkspaceMembersQuery request, CancellationToken cancellationToken)
    {
        var workspace = await workspaceRepository.GetByIdAsync(request.WorkspaceId, cancellationToken);

        if (workspace is null)
        {
            return Result.Fail<IReadOnlyCollection<WorkspaceMemberResult>>("Workspace não encontrado.");
        }

        var isMember = workspace.IsOwnedBy(request.CurrentUserId)
            || await memberRepository.ExistsAsync(workspace.Id, request.CurrentUserId, cancellationToken);

        if (!isMember)
        {
            return Result.Fail<IReadOnlyCollection<WorkspaceMemberResult>>("Workspace não encontrado.");
        }

        var members = await memberRepository.GetByWorkspaceIdAsync(workspace.Id, cancellationToken);
        var memberUserIds = members.Select(m => m.UserId).ToList();
        var users = await userRepository.GetByIdsAsync([workspace.OwnerId, .. memberUserIds], cancellationToken);
        var usersById = users.ToDictionary(u => u.Id);

        var result = new List<WorkspaceMemberResult>
        {
            new(workspace.OwnerId, usersById[workspace.OwnerId].Username.Value, usersById[workspace.OwnerId].Email.Value, workspace.CreatedAt),
        };

        result.AddRange(members
            .Where(m => usersById.ContainsKey(m.UserId))
            .Select(m => new WorkspaceMemberResult(m.UserId, usersById[m.UserId].Username.Value, usersById[m.UserId].Email.Value, m.CreatedAt)));

        return Result.Ok<IReadOnlyCollection<WorkspaceMemberResult>>(result);
    }
}
