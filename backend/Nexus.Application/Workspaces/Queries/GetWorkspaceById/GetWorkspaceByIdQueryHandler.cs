using Nexus.Application.Common;
using Nexus.Domain.Workspaces;
using FluentResults;
using MediatR;

namespace Nexus.Application.Workspaces.Queries.GetWorkspaceById;

public sealed class GetWorkspaceByIdQueryHandler(IWorkspaceRepository workspaceRepository)
    : IRequestHandler<GetWorkspaceByIdQuery, Result<WorkspaceResult>>
{
    public async Task<Result<WorkspaceResult>> Handle(GetWorkspaceByIdQuery request, CancellationToken cancellationToken)
    {
        var workspace = await workspaceRepository.GetByIdAsync(request.WorkspaceId, cancellationToken);

        if (workspace is null || !workspace.IsOwnedBy(request.CurrentUserId))
        {
            return Result.Fail<WorkspaceResult>("Workspace não encontrado.");
        }

        return Result.Ok(new WorkspaceResult(workspace.Id, workspace.Name.Value, workspace.CreatedAt));
    }
}
