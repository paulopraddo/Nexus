using Nexus.Application.Cards.Commands.CreateCard;
using Nexus.Application.Common;
using Nexus.Domain.Boards;
using Nexus.Domain.Cards;
using Nexus.Domain.Common;
using Moq;

namespace Nexus.Tests.Application.Cards;

public class CreateCardCommandHandlerTests
{
    private readonly Mock<IBoardRepository> _boardRepository = new();
    private readonly Mock<IWorkspaceAccessService> _workspaceAccessService = new();
    private readonly Mock<ICardRepository> _cardRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private CreateCardCommandHandler CreateHandler() =>
        new(_boardRepository.Object, _workspaceAccessService.Object, _cardRepository.Object, _unitOfWork.Object);

    [Fact]
    public async Task Handle_QuandoTemAcessoAoWorkspace_CriaCard()
    {
        var userId = Guid.NewGuid();
        var board = Board.Create(BoardName.Create("Sprint 1").Value, Guid.NewGuid()).Value;
        _boardRepository.Setup(r => r.GetByIdAsync(board.Id, It.IsAny<CancellationToken>())).ReturnsAsync(board);
        _workspaceAccessService.Setup(s => s.HasAccessAsync(board.WorkspaceId, userId, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var result = await CreateHandler().Handle(
            new CreateCardCommand(board.Id, userId, "Tarefa 1"), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("Tarefa 1", result.Value.Title);
        _cardRepository.Verify(r => r.AddAsync(It.IsAny<Card>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_QuandoNaoTemAcessoAoWorkspace_RetornaFalhaSemCriar()
    {
        var board = Board.Create(BoardName.Create("Sprint 1").Value, Guid.NewGuid()).Value;
        _boardRepository.Setup(r => r.GetByIdAsync(board.Id, It.IsAny<CancellationToken>())).ReturnsAsync(board);
        _workspaceAccessService.Setup(s => s.HasAccessAsync(board.WorkspaceId, It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var result = await CreateHandler().Handle(
            new CreateCardCommand(board.Id, Guid.NewGuid(), "Tarefa 1"), CancellationToken.None);

        Assert.True(result.IsFailed);
        _cardRepository.Verify(r => r.AddAsync(It.IsAny<Card>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_QuandoBoardNaoExiste_RetornaFalha()
    {
        _boardRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Board?)null);

        var result = await CreateHandler().Handle(
            new CreateCardCommand(Guid.NewGuid(), Guid.NewGuid(), "Tarefa 1"), CancellationToken.None);

        Assert.True(result.IsFailed);
    }
}
