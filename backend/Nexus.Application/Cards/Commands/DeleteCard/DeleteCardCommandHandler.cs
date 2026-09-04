using Nexus.Domain.Boards;
using Nexus.Domain.Cards;
using Nexus.Domain.Common;
using Nexus.Domain.Workspaces;
using FluentResults;
using MediatR;

namespace Nexus.Application.Cards.Commands.DeleteCard;

public sealed class DeleteCardCommandHandler(
    ICardRepository cardRepository,
    IBoardRepository boardRepository,
    IWorkspaceRepository workspaceRepository,
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

        if (board is null)
        {
            return Result.Fail("Card não encontrado.");
        }

        var workspace = await workspaceRepository.GetByIdAsync(board.WorkspaceId, cancellationToken);

        if (workspace is null || !workspace.IsOwnedBy(request.CurrentUserId))
        {
            return Result.Fail("Card não encontrado.");
        }

        cardRepository.Remove(card);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}
