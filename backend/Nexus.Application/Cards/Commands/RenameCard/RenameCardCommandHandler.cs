using Nexus.Application.Common;
using Nexus.Domain.Boards;
using Nexus.Domain.Cards;
using Nexus.Domain.Common;
using Nexus.Domain.Workspaces;
using FluentResults;
using MediatR;

namespace Nexus.Application.Cards.Commands.RenameCard;

public sealed class RenameCardCommandHandler(
    ICardRepository cardRepository,
    IBoardRepository boardRepository,
    IWorkspaceRepository workspaceRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<RenameCardCommand, Result<CardResult>>
{
    public async Task<Result<CardResult>> Handle(RenameCardCommand request, CancellationToken cancellationToken)
    {
        var card = await cardRepository.GetByIdAsync(request.CardId, cancellationToken);

        if (card is null || !await IsOwnedByCurrentUserAsync(card.BoardId, request.CurrentUserId, cancellationToken))
        {
            return Result.Fail<CardResult>("Card não encontrado.");
        }

        var titleResult = CardTitle.Create(request.Title);

        if (titleResult.IsFailed)
        {
            return Result.Fail<CardResult>(titleResult.Errors);
        }

        card.Rename(titleResult.Value);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(new CardResult(card.Id, card.Title.Value, card.BoardId, card.CreatedAt));
    }

    private async Task<bool> IsOwnedByCurrentUserAsync(Guid boardId, Guid currentUserId, CancellationToken cancellationToken)
    {
        var board = await boardRepository.GetByIdAsync(boardId, cancellationToken);

        if (board is null)
        {
            return false;
        }

        var workspace = await workspaceRepository.GetByIdAsync(board.WorkspaceId, cancellationToken);
        return workspace is not null && workspace.IsOwnedBy(currentUserId);
    }
}
