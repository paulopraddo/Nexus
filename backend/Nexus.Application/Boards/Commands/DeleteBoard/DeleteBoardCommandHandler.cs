using Nexus.Domain.Boards;
using Nexus.Domain.Common;
using Nexus.Domain.Workspaces;
using FluentResults;
using MediatR;

namespace Nexus.Application.Boards.Commands.DeleteBoard;

public sealed class DeleteBoardCommandHandler(
    IBoardRepository boardRepository,
    IWorkspaceRepository workspaceRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteBoardCommand, Result>
{
    public async Task<Result> Handle(DeleteBoardCommand request, CancellationToken cancellationToken)
    {
        var board = await boardRepository.GetByIdAsync(request.BoardId, cancellationToken);

        if (board is null)
        {
            return Result.Fail("Board não encontrado.");
        }

        var workspace = await workspaceRepository.GetByIdAsync(board.WorkspaceId, cancellationToken);

        if (workspace is null || !workspace.IsOwnedBy(request.CurrentUserId))
        {
            return Result.Fail("Board não encontrado.");
        }

        boardRepository.Remove(board);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}
