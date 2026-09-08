using Nexus.Domain.Workspaces;

namespace Nexus.Application.Common;

public interface IWorkspaceAccessService
{
    Task<bool> HasAccessAsync(Guid workspaceId, Guid userId, CancellationToken cancellationToken = default);
}

public sealed class WorkspaceAccessService(IWorkspaceRepository workspaceRepository, IWorkspaceMemberRepository memberRepository)
    : IWorkspaceAccessService
{
    public async Task<bool> HasAccessAsync(Guid workspaceId, Guid userId, CancellationToken cancellationToken = default)
    {
        var workspace = await workspaceRepository.GetByIdAsync(workspaceId, cancellationToken);

        if (workspace is null)
        {
            return false;
        }

        return workspace.IsOwnedBy(userId) || await memberRepository.ExistsAsync(workspaceId, userId, cancellationToken);
    }
}
