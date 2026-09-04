using Nexus.Application.Common;
using Nexus.Domain.Boards;
using Nexus.Domain.Cards;
using Nexus.Domain.Workspaces;
using FluentResults;
using MediatR;

namespace Nexus.Application.Cards.Queries.GetCardsByBoard;

public sealed class GetCardsByBoardQueryHandler(
    IBoardRepository boardRepository,
    IWorkspaceRepository workspaceRepository,
    ICardRepository cardRepository)
    : IRequestHandler<GetCardsByBoardQuery, Result<IReadOnlyCollection<CardResult>>>
{
    public async Task<Result<IReadOnlyCollection<CardResult>>> Handle(
        GetCardsByBoardQuery request, CancellationToken cancellationToken)
    {
        var board = await boardRepository.GetByIdAsync(request.BoardId, cancellationToken);

        if (board is null)
        {
            return Result.Fail<IReadOnlyCollection<CardResult>>("Board não encontrado.");
        }

        var workspace = await workspaceRepository.GetByIdAsync(board.WorkspaceId, cancellationToken);

        if (workspace is null || !workspace.IsOwnedBy(request.CurrentUserId))
        {
            return Result.Fail<IReadOnlyCollection<CardResult>>("Board não encontrado.");
        }

        var cards = await cardRepository.GetByBoardIdAsync(request.BoardId, cancellationToken);

        IReadOnlyCollection<CardResult> result = cards
            .Select(c => new CardResult(c.Id, c.Title.Value, c.BoardId, c.CreatedAt))
            .ToList();

        return Result.Ok(result);
    }
}
