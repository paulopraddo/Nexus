using Nexus.Application.Common;
using Nexus.Domain.Common;
using Nexus.Domain.Workspaces;
using FluentResults;
using MediatR;

namespace Nexus.Application.Workspaces.Commands.CreateWorkspace;

public sealed class CreateWorkspaceCommandHandler(IWorkspaceRepository workspaceRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<CreateWorkspaceCommand, Result<WorkspaceResult>>
{
    public async Task<Result<WorkspaceResult>> Handle(CreateWorkspaceCommand request, CancellationToken cancellationToken)
    {
        var nameResult = WorkspaceName.Create(request.Name);

        if (nameResult.IsFailed)
        {
            return Result.Fail<WorkspaceResult>(nameResult.Errors);
        }

        var workspace = Workspace.Create(nameResult.Value, request.OwnerId).Value;

        await workspaceRepository.AddAsync(workspace, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(new WorkspaceResult(workspace.Id, workspace.Name.Value, workspace.CreatedAt));
    }
}
