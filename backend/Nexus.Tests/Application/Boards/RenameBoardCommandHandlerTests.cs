using Nexus.Application.Boards.Commands.RenameBoard;
using Nexus.Domain.Boards;
using Nexus.Domain.Common;
using Nexus.Domain.Workspaces;
using Moq;

namespace Nexus.Tests.Application.Boards;

public class RenameBoardCommandHandlerTests
{
    private readonly Mock<IBoardRepository> _boardRepository = new();
    private readonly Mock<IWorkspaceRepository> _workspaceRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private RenameBoardCommandHandler CreateHandler() =>
        new(_boardRepository.Object, _workspaceRepository.Object, _unitOfWork.Object);

    [Fact]
    public async Task Handle_QuandoDonoDoWorkspace_RenomeiaBoard()
    {
        var ownerId = Guid.NewGuid();
        var workspace = Workspace.Create(WorkspaceName.Create("Time").Value, ownerId).Value;
        var board = Board.Create(BoardName.Create("Sprint 1").Value, workspace.Id).Value;
        _boardRepository.Setup(r => r.GetByIdAsync(board.Id, It.IsAny<CancellationToken>())).ReturnsAsync(board);
        _workspaceRepository.Setup(r => r.GetByIdAsync(workspace.Id, It.IsAny<CancellationToken>())).ReturnsAsync(workspace);

        var result = await CreateHandler().Handle(
            new RenameBoardCommand(board.Id, ownerId, "Sprint 2"), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("Sprint 2", result.Value.Name);
    }

    [Fact]
    public async Task Handle_QuandoNaoEDonoDoWorkspace_RetornaFalha()
    {
        var workspace = Workspace.Create(WorkspaceName.Create("Time").Value, Guid.NewGuid()).Value;
        var board = Board.Create(BoardName.Create("Sprint 1").Value, workspace.Id).Value;
        _boardRepository.Setup(r => r.GetByIdAsync(board.Id, It.IsAny<CancellationToken>())).ReturnsAsync(board);
        _workspaceRepository.Setup(r => r.GetByIdAsync(workspace.Id, It.IsAny<CancellationToken>())).ReturnsAsync(workspace);

        var result = await CreateHandler().Handle(
            new RenameBoardCommand(board.Id, Guid.NewGuid(), "Sprint 2"), CancellationToken.None);

        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task Handle_QuandoBoardNaoExiste_RetornaFalha()
    {
        _boardRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Board?)null);

        var result = await CreateHandler().Handle(
            new RenameBoardCommand(Guid.NewGuid(), Guid.NewGuid(), "Sprint 2"), CancellationToken.None);

        Assert.True(result.IsFailed);
    }
}
