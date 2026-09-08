using Nexus.Application.Boards.Commands.DeleteBoard;
using Nexus.Application.Common;
using Nexus.Domain.Boards;
using Nexus.Domain.Common;
using Moq;

namespace Nexus.Tests.Application.Boards;

public class DeleteBoardCommandHandlerTests
{
    private readonly Mock<IBoardRepository> _boardRepository = new();
    private readonly Mock<IWorkspaceAccessService> _workspaceAccessService = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private DeleteBoardCommandHandler CreateHandler() =>
        new(_boardRepository.Object, _workspaceAccessService.Object, _unitOfWork.Object);

    [Fact]
    public async Task Handle_QuandoTemAcessoAoWorkspace_RemoveBoard()
    {
        var userId = Guid.NewGuid();
        var board = Board.Create(BoardName.Create("Sprint 1").Value, Guid.NewGuid()).Value;
        _boardRepository.Setup(r => r.GetByIdAsync(board.Id, It.IsAny<CancellationToken>())).ReturnsAsync(board);
        _workspaceAccessService.Setup(s => s.HasAccessAsync(board.WorkspaceId, userId, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var result = await CreateHandler().Handle(new DeleteBoardCommand(board.Id, userId), CancellationToken.None);

        Assert.True(result.IsSuccess);
        _boardRepository.Verify(r => r.Remove(board), Times.Once);
    }

    [Fact]
    public async Task Handle_QuandoNaoTemAcessoAoWorkspace_RetornaFalhaSemRemover()
    {
        var board = Board.Create(BoardName.Create("Sprint 1").Value, Guid.NewGuid()).Value;
        _boardRepository.Setup(r => r.GetByIdAsync(board.Id, It.IsAny<CancellationToken>())).ReturnsAsync(board);
        _workspaceAccessService.Setup(s => s.HasAccessAsync(board.WorkspaceId, It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var result = await CreateHandler().Handle(new DeleteBoardCommand(board.Id, Guid.NewGuid()), CancellationToken.None);

        Assert.True(result.IsFailed);
        _boardRepository.Verify(r => r.Remove(It.IsAny<Board>()), Times.Never);
    }
}
