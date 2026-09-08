using Nexus.Application.Cards.Queries.GetCardsByBoard;
using Nexus.Application.Common;
using Nexus.Domain.Boards;
using Nexus.Domain.Cards;
using Moq;

namespace Nexus.Tests.Application.Cards;

public class GetCardsByBoardQueryHandlerTests
{
    private readonly Mock<IBoardRepository> _boardRepository = new();
    private readonly Mock<IWorkspaceAccessService> _workspaceAccessService = new();
    private readonly Mock<ICardRepository> _cardRepository = new();

    private GetCardsByBoardQueryHandler CreateHandler() =>
        new(_boardRepository.Object, _workspaceAccessService.Object, _cardRepository.Object);

    [Fact]
    public async Task Handle_QuandoTemAcesso_RetornaCardsDoBoard()
    {
        var userId = Guid.NewGuid();
        var board = Board.Create(BoardName.Create("Sprint 1").Value, Guid.NewGuid()).Value;
        var card = Card.Create(CardTitle.Create("Tarefa 1").Value, board.Id).Value;
        _boardRepository.Setup(r => r.GetByIdAsync(board.Id, It.IsAny<CancellationToken>())).ReturnsAsync(board);
        _workspaceAccessService.Setup(s => s.HasAccessAsync(board.WorkspaceId, userId, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _cardRepository.Setup(r => r.GetByBoardIdAsync(board.Id, It.IsAny<CancellationToken>())).ReturnsAsync([card]);

        var result = await CreateHandler().Handle(new GetCardsByBoardQuery(board.Id, userId), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value);
    }

    [Fact]
    public async Task Handle_QuandoNaoTemAcesso_RetornaFalha()
    {
        var board = Board.Create(BoardName.Create("Sprint 1").Value, Guid.NewGuid()).Value;
        _boardRepository.Setup(r => r.GetByIdAsync(board.Id, It.IsAny<CancellationToken>())).ReturnsAsync(board);
        _workspaceAccessService.Setup(s => s.HasAccessAsync(board.WorkspaceId, It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var result = await CreateHandler().Handle(
            new GetCardsByBoardQuery(board.Id, Guid.NewGuid()), CancellationToken.None);

        Assert.True(result.IsFailed);
    }
}
