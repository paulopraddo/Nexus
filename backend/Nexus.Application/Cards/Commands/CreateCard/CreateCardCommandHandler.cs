using Nexus.Application.Common;
using Nexus.Domain.Boards;
using Nexus.Domain.Cards;
using Nexus.Domain.Common;
using Nexus.Domain.Workspaces;
using FluentResults;
using MediatR;

namespace Nexus.Application.Cards.Commands.CreateCard;

public sealed class CreateCardCommandHandler(
    IBoardRepository boardRepository,
    IWorkspaceRepository workspaceRepository,
    ICardRepository cardRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<CreateCardCommand, Result<CardResult>>
{
    public async Task<Result<CardResult>> Handle(CreateCardCommand request, CancellationToken cancellationToken)
    {
        var board = await boardRepository.GetByIdAsync(request.BoardId, cancellationToken);

        if (board is null || !await IsOwnedByCurrentUserAsync(board.WorkspaceId, request.CurrentUserId, cancellationToken))
        {
            return Result.Fail<CardResult>("Board não encontrado.");
        }

        var titleResult = CardTitle.Create(request.Title);

        if (titleResult.IsFailed)
        {
            return Result.Fail<CardResult>(titleResult.Errors);
        }

        var card = Card.Create(titleResult.Value, board.Id).Value;

        await cardRepository.AddAsync(card, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(new CardResult(card.Id, card.Title.Value, card.BoardId, card.CreatedAt));
    }

    private async Task<bool> IsOwnedByCurrentUserAsync(Guid workspaceId, Guid currentUserId, CancellationToken cancellationToken)
    {
        var workspace = await workspaceRepository.GetByIdAsync(workspaceId, cancellationToken);
        return workspace is not null && workspace.IsOwnedBy(currentUserId);
    }
}
