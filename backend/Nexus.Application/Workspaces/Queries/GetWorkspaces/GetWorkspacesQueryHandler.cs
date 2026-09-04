using Nexus.Application.Common;
using Nexus.Domain.Workspaces;
using FluentResults;
using MediatR;

namespace Nexus.Application.Workspaces.Queries.GetWorkspaces;

public sealed class GetWorkspacesQueryHandler(IWorkspaceRepository workspaceRepository)
    : IRequestHandler<GetWorkspacesQuery, Result<IReadOnlyCollection<WorkspaceResult>>>
{
    public async Task<Result<IReadOnlyCollection<WorkspaceResult>>> Handle(
        GetWorkspacesQuery request, CancellationToken cancellationToken)
    {
        var workspaces = await workspaceRepository.GetByOwnerIdAsync(request.CurrentUserId, cancellationToken);

        IReadOnlyCollection<WorkspaceResult> result = workspaces
            .Select(w => new WorkspaceResult(w.Id, w.Name.Value, w.CreatedAt))
            .ToList();

        return Result.Ok(result);
    }
}
