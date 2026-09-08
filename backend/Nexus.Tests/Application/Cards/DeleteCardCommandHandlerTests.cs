using Nexus.Application.Cards.Commands.DeleteCard;
using Nexus.Application.Common;
using Nexus.Domain.Boards;
using Nexus.Domain.Cards;
using Nexus.Domain.Common;
using Moq;

namespace Nexus.Tests.Application.Cards;

public class DeleteCardCommandHandlerTests
{
    private readonly Mock<ICardRepository> _cardRepository = new();
    private readonly Mock<IBoardRepository> _boardRepository = new();
    private readonly Mock<IWorkspaceAccessService> _workspaceAccessService = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private DeleteCardCommandHandler CreateHandler() =>
        new(_cardRepository.Object, _boardRepository.Object, _workspaceAccessService.Object, _unitOfWork.Object);

    [Fact]
    public async Task Handle_QuandoTemAcessoAoWorkspace_RemoveCard()
    {
        var userId = Guid.NewGuid();
        var board = Board.Create(BoardName.Create("Sprint 1").Value, Guid.NewGuid()).Value;
        var card = Card.Create(CardTitle.Create("Tarefa 1").Value, board.Id).Value;
        _cardRepository.Setup(r => r.GetByIdAsync(card.Id, It.IsAny<CancellationToken>())).ReturnsAsync(card);
        _boardRepository.Setup(r => r.GetByIdAsync(board.Id, It.IsAny<CancellationToken>())).ReturnsAsync(board);
        _workspaceAccessService.Setup(s => s.HasAccessAsync(board.WorkspaceId, userId, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var result = await CreateHandler().Handle(new DeleteCardCommand(card.Id, userId), CancellationToken.None);

        Assert.True(result.IsSuccess);
        _cardRepository.Verify(r => r.Remove(card), Times.Once);
    }

    [Fact]
    public async Task Handle_QuandoNaoTemAcessoAoWorkspace_RetornaFalhaSemRemover()
    {
        var board = Board.Create(BoardName.Create("Sprint 1").Value, Guid.NewGuid()).Value;
        var card = Card.Create(CardTitle.Create("Tarefa 1").Value, board.Id).Value;
        _cardRepository.Setup(r => r.GetByIdAsync(card.Id, It.IsAny<CancellationToken>())).ReturnsAsync(card);
        _boardRepository.Setup(r => r.GetByIdAsync(board.Id, It.IsAny<CancellationToken>())).ReturnsAsync(board);
        _workspaceAccessService.Setup(s => s.HasAccessAsync(board.WorkspaceId, It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var result = await CreateHandler().Handle(new DeleteCardCommand(card.Id, Guid.NewGuid()), CancellationToken.None);

        Assert.True(result.IsFailed);
        _cardRepository.Verify(r => r.Remove(It.IsAny<Card>()), Times.Never);
    }
}
