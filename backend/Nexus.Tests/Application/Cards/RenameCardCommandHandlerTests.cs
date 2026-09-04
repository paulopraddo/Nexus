using Nexus.Application.Cards.Commands.RenameCard;
using Nexus.Domain.Boards;
using Nexus.Domain.Cards;
using Nexus.Domain.Common;
using Nexus.Domain.Workspaces;
using Moq;

namespace Nexus.Tests.Application.Cards;

public class RenameCardCommandHandlerTests
{
    private readonly Mock<ICardRepository> _cardRepository = new();
    private readonly Mock<IBoardRepository> _boardRepository = new();
    private readonly Mock<IWorkspaceRepository> _workspaceRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private RenameCardCommandHandler CreateHandler() =>
        new(_cardRepository.Object, _boardRepository.Object, _workspaceRepository.Object, _unitOfWork.Object);

    [Fact]
    public async Task Handle_QuandoDonoDoWorkspace_RenomeiaCard()
    {
        var ownerId = Guid.NewGuid();
        var workspace = Workspace.Create(WorkspaceName.Create("Time").Value, ownerId).Value;
        var board = Board.Create(BoardName.Create("Sprint 1").Value, workspace.Id).Value;
        var card = Card.Create(CardTitle.Create("Tarefa 1").Value, board.Id).Value;
        _cardRepository.Setup(r => r.GetByIdAsync(card.Id, It.IsAny<CancellationToken>())).ReturnsAsync(card);
        _boardRepository.Setup(r => r.GetByIdAsync(board.Id, It.IsAny<CancellationToken>())).ReturnsAsync(board);
        _workspaceRepository.Setup(r => r.GetByIdAsync(workspace.Id, It.IsAny<CancellationToken>())).ReturnsAsync(workspace);

        var result = await CreateHandler().Handle(
            new RenameCardCommand(card.Id, ownerId, "Tarefa 2"), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("Tarefa 2", result.Value.Title);
    }

    [Fact]
    public async Task Handle_QuandoNaoEDonoDoWorkspace_RetornaFalha()
    {
        var workspace = Workspace.Create(WorkspaceName.Create("Time").Value, Guid.NewGuid()).Value;
        var board = Board.Create(BoardName.Create("Sprint 1").Value, workspace.Id).Value;
        var card = Card.Create(CardTitle.Create("Tarefa 1").Value, board.Id).Value;
        _cardRepository.Setup(r => r.GetByIdAsync(card.Id, It.IsAny<CancellationToken>())).ReturnsAsync(card);
        _boardRepository.Setup(r => r.GetByIdAsync(board.Id, It.IsAny<CancellationToken>())).ReturnsAsync(board);
        _workspaceRepository.Setup(r => r.GetByIdAsync(workspace.Id, It.IsAny<CancellationToken>())).ReturnsAsync(workspace);

        var result = await CreateHandler().Handle(
            new RenameCardCommand(card.Id, Guid.NewGuid(), "Tarefa 2"), CancellationToken.None);

        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task Handle_QuandoCardNaoExiste_RetornaFalha()
    {
        _cardRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Card?)null);

        var result = await CreateHandler().Handle(
            new RenameCardCommand(Guid.NewGuid(), Guid.NewGuid(), "Tarefa 2"), CancellationToken.None);

        Assert.True(result.IsFailed);
    }
}
