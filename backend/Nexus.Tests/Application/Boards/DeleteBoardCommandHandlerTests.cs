using Nexus.Application.Boards.Commands.DeleteBoard;
using Nexus.Domain.Boards;
using Nexus.Domain.Common;
using Nexus.Domain.Workspaces;
using Moq;

namespace Nexus.Tests.Application.Boards;

public class DeleteBoardCommandHandlerTests
{
    private readonly Mock<IBoardRepository> _boardRepository = new();
    private readonly Mock<IWorkspaceRepository> _workspaceRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private DeleteBoardCommandHandler CreateHandler() =>
        new(_boardRepository.Object, _workspaceRepository.Object, _unitOfWork.Object);

    [Fact]
    public async Task Handle_QuandoDonoDoWorkspace_RemoveBoard()
    {
        var ownerId = Guid.NewGuid();
        var workspace = Workspace.Create(WorkspaceName.Create("Time").Value, ownerId).Value;
        var board = Board.Create(BoardName.Create("Sprint 1").Value, workspace.Id).Value;
        _boardRepository.Setup(r => r.GetByIdAsync(board.Id, It.IsAny<CancellationToken>())).ReturnsAsync(board);
        _workspaceRepository.Setup(r => r.GetByIdAsync(workspace.Id, It.IsAny<CancellationToken>())).ReturnsAsync(workspace);

        var result = await CreateHandler().Handle(new DeleteBoardCommand(board.Id, ownerId), CancellationToken.None);

        Assert.True(result.IsSuccess);
        _boardRepository.Verify(r => r.Remove(board), Times.Once);
    }

    [Fact]
    public async Task Handle_QuandoNaoEDonoDoWorkspace_RetornaFalhaSemRemover()
    {
        var workspace = Workspace.Create(WorkspaceName.Create("Time").Value, Guid.NewGuid()).Value;
        var board = Board.Create(BoardName.Create("Sprint 1").Value, workspace.Id).Value;
        _boardRepository.Setup(r => r.GetByIdAsync(board.Id, It.IsAny<CancellationToken>())).ReturnsAsync(board);
        _workspaceRepository.Setup(r => r.GetByIdAsync(workspace.Id, It.IsAny<CancellationToken>())).ReturnsAsync(workspace);

        var result = await CreateHandler().Handle(new DeleteBoardCommand(board.Id, Guid.NewGuid()), CancellationToken.None);

        Assert.True(result.IsFailed);
        _boardRepository.Verify(r => r.Remove(It.IsAny<Board>()), Times.Never);
    }
}
