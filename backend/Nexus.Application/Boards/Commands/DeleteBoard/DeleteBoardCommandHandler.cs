using Nexus.Application.Common;
using Nexus.Domain.Boards;
using Nexus.Domain.Common;
using FluentResults;
using MediatR;

namespace Nexus.Application.Boards.Commands.DeleteBoard;

public sealed class DeleteBoardCommandHandler(
    IBoardRepository boardRepository,
    IWorkspaceAccessService workspaceAccessService,
    IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteBoardCommand, Result>
{
    public async Task<Result> Handle(DeleteBoardCommand request, CancellationToken cancellationToken)
    {
        var board = await boardRepository.GetByIdAsync(request.BoardId, cancellationToken);

        if (board is null
            || !await workspaceAccessService.HasAccessAsync(board.WorkspaceId, request.CurrentUserId, cancellationToken))
        {
            return Result.Fail("Board não encontrado.");
        }

        boardRepository.Remove(board);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}
