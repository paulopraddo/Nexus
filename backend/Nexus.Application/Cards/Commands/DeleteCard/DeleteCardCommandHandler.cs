using Nexus.Application.Common;
using Nexus.Domain.Boards;
using Nexus.Domain.Cards;
using Nexus.Domain.Common;
using FluentResults;
using MediatR;

namespace Nexus.Application.Cards.Commands.DeleteCard;

public sealed class DeleteCardCommandHandler(
    ICardRepository cardRepository,
    IBoardRepository boardRepository,
    IWorkspaceAccessService workspaceAccessService,
    IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteCardCommand, Result>
{
    public async Task<Result> Handle(DeleteCardCommand request, CancellationToken cancellationToken)
    {
        var card = await cardRepository.GetByIdAsync(request.CardId, cancellationToken);

        if (card is null)
        {
            return Result.Fail("Card não encontrado.");
        }

        var board = await boardRepository.GetByIdAsync(card.BoardId, cancellationToken);

        if (board is null
            || !await workspaceAccessService.HasAccessAsync(board.WorkspaceId, request.CurrentUserId, cancellationToken))
        {
            return Result.Fail("Card não encontrado.");
        }

        cardRepository.Remove(card);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}
