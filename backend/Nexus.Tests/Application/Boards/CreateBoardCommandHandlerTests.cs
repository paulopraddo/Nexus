using Nexus.Application.Boards.Commands.CreateBoard;
using Nexus.Domain.Boards;
using Nexus.Domain.Common;
using Nexus.Domain.Workspaces;
using Moq;

namespace Nexus.Tests.Application.Boards;

public class CreateBoardCommandHandlerTests
{
    private readonly Mock<IWorkspaceRepository> _workspaceRepository = new();
    private readonly Mock<IBoardRepository> _boardRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private CreateBoardCommandHandler CreateHandler() =>
        new(_workspaceRepository.Object, _boardRepository.Object, _unitOfWork.Object);

    [Fact]
    public async Task Handle_QuandoDonoDoWorkspace_CriaBoard()
    {
        var ownerId = Guid.NewGuid();
        var workspace = Workspace.Create(WorkspaceName.Create("Time").Value, ownerId).Value;
        _workspaceRepository.Setup(r => r.GetByIdAsync(workspace.Id, It.IsAny<CancellationToken>())).ReturnsAsync(workspace);

        var result = await CreateHandler().Handle(
            new CreateBoardCommand(workspace.Id, ownerId, "Sprint 1"), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("Sprint 1", result.Value.Name);
        _boardRepository.Verify(r => r.AddAsync(It.IsAny<Board>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_QuandoNaoEDonoDoWorkspace_RetornaFalhaSemCriar()
    {
        var workspace = Workspace.Create(WorkspaceName.Create("Time").Value, Guid.NewGuid()).Value;
        _workspaceRepository.Setup(r => r.GetByIdAsync(workspace.Id, It.IsAny<CancellationToken>())).ReturnsAsync(workspace);

        var result = await CreateHandler().Handle(
            new CreateBoardCommand(workspace.Id, Guid.NewGuid(), "Sprint 1"), CancellationToken.None);

        Assert.True(result.IsFailed);
        _boardRepository.Verify(r => r.AddAsync(It.IsAny<Board>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_QuandoWorkspaceNaoExiste_RetornaFalha()
    {
        _workspaceRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Workspace?)null);

        var result = await CreateHandler().Handle(
            new CreateBoardCommand(Guid.NewGuid(), Guid.NewGuid(), "Sprint 1"), CancellationToken.None);

        Assert.True(result.IsFailed);
    }
}
