using Nexus.Application.Cards.Commands.RenameCard;
using Nexus.Application.Common;
using Nexus.Domain.Boards;
using Nexus.Domain.Cards;
using Nexus.Domain.Common;
using Moq;

namespace Nexus.Tests.Application.Cards;

public class RenameCardCommandHandlerTests
{
    private readonly Mock<ICardRepository> _cardRepository = new();
    private readonly Mock<IBoardRepository> _boardRepository = new();
    private readonly Mock<IWorkspaceAccessService> _workspaceAccessService = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private RenameCardCommandHandler CreateHandler() =>
        new(_cardRepository.Object, _boardRepository.Object, _workspaceAccessService.Object, _unitOfWork.Object);

    [Fact]
    public async Task Handle_QuandoTemAcessoAoWorkspace_RenomeiaCard()
    {
        var userId = Guid.NewGuid();
        var board = Board.Create(BoardName.Create("Sprint 1").Value, Guid.NewGuid()).Value;
        var card = Card.Create(CardTitle.Create("Tarefa 1").Value, board.Id).Value;
        _cardRepository.Setup(r => r.GetByIdAsync(card.Id, It.IsAny<CancellationToken>())).ReturnsAsync(card);
        _boardRepository.Setup(r => r.GetByIdAsync(board.Id, It.IsAny<CancellationToken>())).ReturnsAsync(board);
        _workspaceAccessService.Setup(s => s.HasAccessAsync(board.WorkspaceId, userId, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var result = await CreateHandler().Handle(
            new RenameCardCommand(card.Id, userId, "Tarefa 2"), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("Tarefa 2", result.Value.Title);
    }

    [Fact]
    public async Task Handle_QuandoNaoTemAcessoAoWorkspace_RetornaFalha()
    {
        var board = Board.Create(BoardName.Create("Sprint 1").Value, Guid.NewGuid()).Value;
        var card = Card.Create(CardTitle.Create("Tarefa 1").Value, board.Id).Value;
        _cardRepository.Setup(r => r.GetByIdAsync(card.Id, It.IsAny<CancellationToken>())).ReturnsAsync(card);
        _boardRepository.Setup(r => r.GetByIdAsync(board.Id, It.IsAny<CancellationToken>())).ReturnsAsync(board);
        _workspaceAccessService.Setup(s => s.HasAccessAsync(board.WorkspaceId, It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);

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
