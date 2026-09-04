using Nexus.Application.Common;
using Nexus.Domain.Boards;
using Nexus.Domain.Workspaces;
using FluentResults;
using MediatR;

namespace Nexus.Application.Boards.Queries.GetBoardsByWorkspace;

public sealed class GetBoardsByWorkspaceQueryHandler(
    IWorkspaceRepository workspaceRepository,
    IBoardRepository boardRepository)
    : IRequestHandler<GetBoardsByWorkspaceQuery, Result<IReadOnlyCollection<BoardResult>>>
{
    public async Task<Result<IReadOnlyCollection<BoardResult>>> Handle(
        GetBoardsByWorkspaceQuery request, CancellationToken cancellationToken)
    {
        var workspace = await workspaceRepository.GetByIdAsync(request.WorkspaceId, cancellationToken);

        if (workspace is null || !workspace.IsOwnedBy(request.CurrentUserId))
        {
            return Result.Fail<IReadOnlyCollection<BoardResult>>("Workspace não encontrado.");
        }

        var boards = await boardRepository.GetByWorkspaceIdAsync(request.WorkspaceId, cancellationToken);

        IReadOnlyCollection<BoardResult> result = boards
            .Select(b => new BoardResult(b.Id, b.Name.Value, b.WorkspaceId, b.CreatedAt))
            .ToList();

        return Result.Ok(result);
    }
}
