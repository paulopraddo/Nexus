using Nexus.Application.Cards.Queries.GetCardsByBoard;
using Nexus.Domain.Boards;
using Nexus.Domain.Cards;
using Nexus.Domain.Workspaces;
using Moq;

namespace Nexus.Tests.Application.Cards;

public class GetCardsByBoardQueryHandlerTests
{
    private readonly Mock<IBoardRepository> _boardRepository = new();
    private readonly Mock<IWorkspaceRepository> _workspaceRepository = new();
    private readonly Mock<ICardRepository> _cardRepository = new();

    private GetCardsByBoardQueryHandler CreateHandler() =>
        new(_boardRepository.Object, _workspaceRepository.Object, _cardRepository.Object);

    [Fact]
    public async Task Handle_QuandoDono_RetornaCardsDoBoard()
    {
        var ownerId = Guid.NewGuid();
        var workspace = Workspace.Create(WorkspaceName.Create("Time").Value, ownerId).Value;
        var board = Board.Create(BoardName.Create("Sprint 1").Value, workspace.Id).Value;
        var card = Card.Create(CardTitle.Create("Tarefa 1").Value, board.Id).Value;
        _boardRepository.Setup(r => r.GetByIdAsync(board.Id, It.IsAny<CancellationToken>())).ReturnsAsync(board);
        _workspaceRepository.Setup(r => r.GetByIdAsync(workspace.Id, It.IsAny<CancellationToken>())).ReturnsAsync(workspace);
        _cardRepository.Setup(r => r.GetByBoardIdAsync(board.Id, It.IsAny<CancellationToken>())).ReturnsAsync([card]);

        var result = await CreateHandler().Handle(new GetCardsByBoardQuery(board.Id, ownerId), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value);
    }

    [Fact]
    public async Task Handle_QuandoNaoEDono_RetornaFalha()
    {
        var workspace = Workspace.Create(WorkspaceName.Create("Time").Value, Guid.NewGuid()).Value;
        var board = Board.Create(BoardName.Create("Sprint 1").Value, workspace.Id).Value;
        _boardRepository.Setup(r => r.GetByIdAsync(board.Id, It.IsAny<CancellationToken>())).ReturnsAsync(board);
        _workspaceRepository.Setup(r => r.GetByIdAsync(workspace.Id, It.IsAny<CancellationToken>())).ReturnsAsync(workspace);

        var result = await CreateHandler().Handle(
            new GetCardsByBoardQuery(board.Id, Guid.NewGuid()), CancellationToken.None);

        Assert.True(result.IsFailed);
    }
}
