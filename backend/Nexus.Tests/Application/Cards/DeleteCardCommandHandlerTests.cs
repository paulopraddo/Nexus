using Nexus.Application.Cards.Commands.DeleteCard;
using Nexus.Domain.Boards;
using Nexus.Domain.Cards;
using Nexus.Domain.Common;
using Nexus.Domain.Workspaces;
using Moq;

namespace Nexus.Tests.Application.Cards;

public class DeleteCardCommandHandlerTests
{
    private readonly Mock<ICardRepository> _cardRepository = new();
    private readonly Mock<IBoardRepository> _boardRepository = new();
    private readonly Mock<IWorkspaceRepository> _workspaceRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private DeleteCardCommandHandler CreateHandler() =>
        new(_cardRepository.Object, _boardRepository.Object, _workspaceRepository.Object, _unitOfWork.Object);

    [Fact]
    public async Task Handle_QuandoDonoDoWorkspace_RemoveCard()
    {
        var ownerId = Guid.NewGuid();
        var workspace = Workspace.Create(WorkspaceName.Create("Time").Value, ownerId).Value;
        var board = Board.Create(BoardName.Create("Sprint 1").Value, workspace.Id).Value;
        var card = Card.Create(CardTitle.Create("Tarefa 1").Value, board.Id).Value;
        _cardRepository.Setup(r => r.GetByIdAsync(card.Id, It.IsAny<CancellationToken>())).ReturnsAsync(card);
        _boardRepository.Setup(r => r.GetByIdAsync(board.Id, It.IsAny<CancellationToken>())).ReturnsAsync(board);
        _workspaceRepository.Setup(r => r.GetByIdAsync(workspace.Id, It.IsAny<CancellationToken>())).ReturnsAsync(workspace);

        var result = await CreateHandler().Handle(new DeleteCardCommand(card.Id, ownerId), CancellationToken.None);

        Assert.True(result.IsSuccess);
        _cardRepository.Verify(r => r.Remove(card), Times.Once);
    }

    [Fact]
    public async Task Handle_QuandoNaoEDonoDoWorkspace_RetornaFalhaSemRemover()
    {
        var workspace = Workspace.Create(WorkspaceName.Create("Time").Value, Guid.NewGuid()).Value;
        var board = Board.Create(BoardName.Create("Sprint 1").Value, workspace.Id).Value;
        var card = Card.Create(CardTitle.Create("Tarefa 1").Value, board.Id).Value;
        _cardRepository.Setup(r => r.GetByIdAsync(card.Id, It.IsAny<CancellationToken>())).ReturnsAsync(card);
        _boardRepository.Setup(r => r.GetByIdAsync(board.Id, It.IsAny<CancellationToken>())).ReturnsAsync(board);
        _workspaceRepository.Setup(r => r.GetByIdAsync(workspace.Id, It.IsAny<CancellationToken>())).ReturnsAsync(workspace);

        var result = await CreateHandler().Handle(new DeleteCardCommand(card.Id, Guid.NewGuid()), CancellationToken.None);

        Assert.True(result.IsFailed);
        _cardRepository.Verify(r => r.Remove(It.IsAny<Card>()), Times.Never);
    }
}
