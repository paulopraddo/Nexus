using Nexus.Application.Common;
using Nexus.Domain.Boards;
using Nexus.Domain.Common;
using Nexus.Domain.Workspaces;
using FluentResults;
using MediatR;

namespace Nexus.Application.Boards.Commands.CreateBoard;

public sealed class CreateBoardCommandHandler(
    IWorkspaceRepository workspaceRepository,
    IBoardRepository boardRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<CreateBoardCommand, Result<BoardResult>>
{
    public async Task<Result<BoardResult>> Handle(CreateBoardCommand request, CancellationToken cancellationToken)
    {
        var workspace = await workspaceRepository.GetByIdAsync(request.WorkspaceId, cancellationToken);

        if (workspace is null || !workspace.IsOwnedBy(request.CurrentUserId))
        {
            return Result.Fail<BoardResult>("Workspace não encontrado.");
        }

        var nameResult = BoardName.Create(request.Name);

        if (nameResult.IsFailed)
        {
            return Result.Fail<BoardResult>(nameResult.Errors);
        }

        var board = Board.Create(nameResult.Value, workspace.Id).Value;

        await boardRepository.AddAsync(board, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(new BoardResult(board.Id, board.Name.Value, board.WorkspaceId, board.CreatedAt));
    }
}
