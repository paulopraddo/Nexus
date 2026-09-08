using Nexus.Application.Boards.Commands.RenameBoard;
using Nexus.Application.Common;
using Nexus.Domain.Boards;
using Nexus.Domain.Common;
using Moq;

namespace Nexus.Tests.Application.Boards;

public class RenameBoardCommandHandlerTests
{
    private readonly Mock<IBoardRepository> _boardRepository = new();
    private readonly Mock<IWorkspaceAccessService> _workspaceAccessService = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private RenameBoardCommandHandler CreateHandler() =>
        new(_boardRepository.Object, _workspaceAccessService.Object, _unitOfWork.Object);

    [Fact]
    public async Task Handle_QuandoTemAcessoAoWorkspace_RenomeiaBoard()
    {
        var userId = Guid.NewGuid();
        var board = Board.Create(BoardName.Create("Sprint 1").Value, Guid.NewGuid()).Value;
        _boardRepository.Setup(r => r.GetByIdAsync(board.Id, It.IsAny<CancellationToken>())).ReturnsAsync(board);
        _workspaceAccessService.Setup(s => s.HasAccessAsync(board.WorkspaceId, userId, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var result = await CreateHandler().Handle(
            new RenameBoardCommand(board.Id, userId, "Sprint 2"), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("Sprint 2", result.Value.Name);
    }

    [Fact]
    public async Task Handle_QuandoNaoTemAcessoAoWorkspace_RetornaFalha()
    {
        var board = Board.Create(BoardName.Create("Sprint 1").Value, Guid.NewGuid()).Value;
        _boardRepository.Setup(r => r.GetByIdAsync(board.Id, It.IsAny<CancellationToken>())).ReturnsAsync(board);
        _workspaceAccessService.Setup(s => s.HasAccessAsync(board.WorkspaceId, It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);

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
