using Nexus.Application.Cards.Commands.CreateCard;
using Nexus.Domain.Boards;
using Nexus.Domain.Cards;
using Nexus.Domain.Common;
using Nexus.Domain.Workspaces;
using Moq;

namespace Nexus.Tests.Application.Cards;

public class CreateCardCommandHandlerTests
{
    private readonly Mock<IBoardRepository> _boardRepository = new();
    private readonly Mock<IWorkspaceRepository> _workspaceRepository = new();
    private readonly Mock<ICardRepository> _cardRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private CreateCardCommandHandler CreateHandler() =>
        new(_boardRepository.Object, _workspaceRepository.Object, _cardRepository.Object, _unitOfWork.Object);

    [Fact]
    public async Task Handle_QuandoDonoDoWorkspace_CriaCard()
    {
        var ownerId = Guid.NewGuid();
        var workspace = Workspace.Create(WorkspaceName.Create("Time").Value, ownerId).Value;
        var board = Board.Create(BoardName.Create("Sprint 1").Value, workspace.Id).Value;
        _boardRepository.Setup(r => r.GetByIdAsync(board.Id, It.IsAny<CancellationToken>())).ReturnsAsync(board);
        _workspaceRepository.Setup(r => r.GetByIdAsync(workspace.Id, It.IsAny<CancellationToken>())).ReturnsAsync(workspace);

        var result = await CreateHandler().Handle(
            new CreateCardCommand(board.Id, ownerId, "Tarefa 1"), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("Tarefa 1", result.Value.Title);
        _cardRepository.Verify(r => r.AddAsync(It.IsAny<Card>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_QuandoNaoEDonoDoWorkspace_RetornaFalhaSemCriar()
    {
        var workspace = Workspace.Create(WorkspaceName.Create("Time").Value, Guid.NewGuid()).Value;
        var board = Board.Create(BoardName.Create("Sprint 1").Value, workspace.Id).Value;
        _boardRepository.Setup(r => r.GetByIdAsync(board.Id, It.IsAny<CancellationToken>())).ReturnsAsync(board);
        _workspaceRepository.Setup(r => r.GetByIdAsync(workspace.Id, It.IsAny<CancellationToken>())).ReturnsAsync(workspace);

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
