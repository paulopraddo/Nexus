using Nexus.Application.Boards.Queries.GetBoardsByWorkspace;
using Nexus.Application.Common;
using Nexus.Domain.Boards;
using Moq;

namespace Nexus.Tests.Application.Boards;

public class GetBoardsByWorkspaceQueryHandlerTests
{
    private readonly Mock<IWorkspaceAccessService> _workspaceAccessService = new();
    private readonly Mock<IBoardRepository> _boardRepository = new();

    private GetBoardsByWorkspaceQueryHandler CreateHandler() =>
        new(_workspaceAccessService.Object, _boardRepository.Object);

    [Fact]
    public async Task Handle_QuandoTemAcesso_RetornaBoardsDoWorkspace()
    {
        var workspaceId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var board = Board.Create(BoardName.Create("Sprint 1").Value, workspaceId).Value;
        _workspaceAccessService.Setup(s => s.HasAccessAsync(workspaceId, userId, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _boardRepository.Setup(r => r.GetByWorkspaceIdAsync(workspaceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([board]);

        var result = await CreateHandler().Handle(
            new GetBoardsByWorkspaceQuery(workspaceId, userId), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value);
    }

    [Fact]
    public async Task Handle_QuandoNaoTemAcesso_RetornaFalha()
    {
        var workspaceId = Guid.NewGuid();
        _workspaceAccessService.Setup(s => s.HasAccessAsync(workspaceId, It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var result = await CreateHandler().Handle(
            new GetBoardsByWorkspaceQuery(workspaceId, Guid.NewGuid()), CancellationToken.None);

        Assert.True(result.IsFailed);
    }
}
