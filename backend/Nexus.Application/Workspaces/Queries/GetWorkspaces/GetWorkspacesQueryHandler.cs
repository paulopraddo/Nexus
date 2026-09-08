using Nexus.Application.Common;
using Nexus.Domain.Workspaces;
using FluentResults;
using MediatR;

namespace Nexus.Application.Workspaces.Queries.GetWorkspaces;

public sealed class GetWorkspacesQueryHandler(IWorkspaceRepository workspaceRepository, IWorkspaceMemberRepository memberRepository)
    : IRequestHandler<GetWorkspacesQuery, Result<IReadOnlyCollection<WorkspaceResult>>>
{
    public async Task<Result<IReadOnlyCollection<WorkspaceResult>>> Handle(
        GetWorkspacesQuery request, CancellationToken cancellationToken)
    {
        var ownedWorkspaces = await workspaceRepository.GetByOwnerIdAsync(request.CurrentUserId, cancellationToken);

        var memberships = await memberRepository.GetByUserIdAsync(request.CurrentUserId, cancellationToken);
        var memberWorkspaceIds = memberships.Select(m => m.WorkspaceId).ToList();
        var memberWorkspaces = memberWorkspaceIds.Count > 0
            ? await workspaceRepository.GetByIdsAsync(memberWorkspaceIds, cancellationToken)
            : [];

        IReadOnlyCollection<WorkspaceResult> result = ownedWorkspaces
            .Concat(memberWorkspaces)
            .Select(w => new WorkspaceResult(w.Id, w.Name.Value, w.OwnerId, w.CreatedAt))
            .ToList();

        return Result.Ok(result);
    }
}
