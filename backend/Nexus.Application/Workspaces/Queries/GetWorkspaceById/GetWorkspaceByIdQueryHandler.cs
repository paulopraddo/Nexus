using Nexus.Application.Common;
using Nexus.Domain.Workspaces;
using FluentResults;
using MediatR;

namespace Nexus.Application.Workspaces.Queries.GetWorkspaceById;

public sealed class GetWorkspaceByIdQueryHandler(IWorkspaceRepository workspaceRepository, IWorkspaceAccessService workspaceAccessService)
    : IRequestHandler<GetWorkspaceByIdQuery, Result<WorkspaceResult>>
{
    public async Task<Result<WorkspaceResult>> Handle(GetWorkspaceByIdQuery request, CancellationToken cancellationToken)
    {
        var workspace = await workspaceRepository.GetByIdAsync(request.WorkspaceId, cancellationToken);

        if (workspace is null
            || !await workspaceAccessService.HasAccessAsync(workspace.Id, request.CurrentUserId, cancellationToken))
        {
            return Result.Fail<WorkspaceResult>("Workspace não encontrado.");
        }

        return Result.Ok(new WorkspaceResult(workspace.Id, workspace.Name.Value, workspace.OwnerId, workspace.CreatedAt));
    }
}
