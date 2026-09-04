using Nexus.Application.Common;
using Nexus.Domain.Common;
using Nexus.Domain.Workspaces;
using FluentResults;
using MediatR;

namespace Nexus.Application.Workspaces.Commands.RenameWorkspace;

public sealed class RenameWorkspaceCommandHandler(IWorkspaceRepository workspaceRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<RenameWorkspaceCommand, Result<WorkspaceResult>>
{
    public async Task<Result<WorkspaceResult>> Handle(RenameWorkspaceCommand request, CancellationToken cancellationToken)
    {
        var workspace = await workspaceRepository.GetByIdAsync(request.WorkspaceId, cancellationToken);

        if (workspace is null || !workspace.IsOwnedBy(request.CurrentUserId))
        {
            return Result.Fail<WorkspaceResult>("Workspace não encontrado.");
        }

        var nameResult = WorkspaceName.Create(request.Name);

        if (nameResult.IsFailed)
        {
            return Result.Fail<WorkspaceResult>(nameResult.Errors);
        }

        workspace.Rename(nameResult.Value);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(new WorkspaceResult(workspace.Id, workspace.Name.Value, workspace.CreatedAt));
    }
}
