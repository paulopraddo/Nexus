using Nexus.Application.Common;
using Nexus.Domain.Boards;
using Nexus.Domain.Common;
using FluentResults;
using MediatR;

namespace Nexus.Application.Boards.Commands.RenameBoard;

public sealed class RenameBoardCommandHandler(
    IBoardRepository boardRepository,
    IWorkspaceAccessService workspaceAccessService,
    IUnitOfWork unitOfWork)
    : IRequestHandler<RenameBoardCommand, Result<BoardResult>>
{
    public async Task<Result<BoardResult>> Handle(RenameBoardCommand request, CancellationToken cancellationToken)
    {
        var board = await boardRepository.GetByIdAsync(request.BoardId, cancellationToken);

        if (board is null
            || !await workspaceAccessService.HasAccessAsync(board.WorkspaceId, request.CurrentUserId, cancellationToken))
        {
            return Result.Fail<BoardResult>("Board não encontrado.");
        }

        var nameResult = BoardName.Create(request.Name);

        if (nameResult.IsFailed)
        {
            return Result.Fail<BoardResult>(nameResult.Errors);
        }

        board.Rename(nameResult.Value);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(new BoardResult(board.Id, board.Name.Value, board.WorkspaceId, board.CreatedAt));
    }
}
