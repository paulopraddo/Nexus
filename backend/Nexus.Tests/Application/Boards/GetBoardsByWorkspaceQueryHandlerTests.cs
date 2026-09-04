using Nexus.Application.Boards.Queries.GetBoardsByWorkspace;
using Nexus.Domain.Boards;
using Nexus.Domain.Workspaces;
using Moq;

namespace Nexus.Tests.Application.Boards;

public class GetBoardsByWorkspaceQueryHandlerTests
{
    private readonly Mock<IWorkspaceRepository> _workspaceRepository = new();
    private readonly Mock<IBoardRepository> _boardRepository = new();

    private GetBoardsByWorkspaceQueryHandler CreateHandler() =>
        new(_workspaceRepository.Object, _boardRepository.Object);

    [Fact]
    public async Task Handle_QuandoDono_RetornaBoardsDoWorkspace()
    {
        var ownerId = Guid.NewGuid();
        var workspace = Workspace.Create(WorkspaceName.Create("Time").Value, ownerId).Value;
        var board = Board.Create(BoardName.Create("Sprint 1").Value, workspace.Id).Value;
        _workspaceRepository.Setup(r => r.GetByIdAsync(workspace.Id, It.IsAny<CancellationToken>())).ReturnsAsync(workspace);
        _boardRepository.Setup(r => r.GetByWorkspaceIdAsync(workspace.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync([board]);

        var result = await CreateHandler().Handle(
            new GetBoardsByWorkspaceQuery(workspace.Id, ownerId), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value);
    }

    [Fact]
    public async Task Handle_QuandoNaoEDono_RetornaFalha()
    {
        var workspace = Workspace.Create(WorkspaceName.Create("Time").Value, Guid.NewGuid()).Value;
        _workspaceRepository.Setup(r => r.GetByIdAsync(workspace.Id, It.IsAny<CancellationToken>())).ReturnsAsync(workspace);

        var result = await CreateHandler().Handle(
            new GetBoardsByWorkspaceQuery(workspace.Id, Guid.NewGuid()), CancellationToken.None);

        Assert.True(result.IsFailed);
    }
}
