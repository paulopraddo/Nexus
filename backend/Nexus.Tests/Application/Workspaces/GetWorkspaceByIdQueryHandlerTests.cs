using Nexus.Application.Common;
using Nexus.Application.Workspaces.Queries.GetWorkspaceById;
using Nexus.Domain.Workspaces;
using Moq;

namespace Nexus.Tests.Application.Workspaces;

public class GetWorkspaceByIdQueryHandlerTests
{
    private readonly Mock<IWorkspaceRepository> _workspaceRepository = new();
    private readonly Mock<IWorkspaceAccessService> _workspaceAccessService = new();

    private GetWorkspaceByIdQueryHandler CreateHandler() => new(_workspaceRepository.Object, _workspaceAccessService.Object);

    [Fact]
    public async Task Handle_QuandoTemAcesso_RetornaWorkspace()
    {
        var ownerId = Guid.NewGuid();
        var workspace = Workspace.Create(WorkspaceName.Create("Time").Value, ownerId).Value;
        _workspaceRepository.Setup(r => r.GetByIdAsync(workspace.Id, It.IsAny<CancellationToken>())).ReturnsAsync(workspace);
        _workspaceAccessService.Setup(s => s.HasAccessAsync(workspace.Id, ownerId, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var result = await CreateHandler().Handle(new GetWorkspaceByIdQuery(workspace.Id, ownerId), CancellationToken.None);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_QuandoNaoTemAcesso_RetornaFalha()
    {
        var workspace = Workspace.Create(WorkspaceName.Create("Time").Value, Guid.NewGuid()).Value;
        _workspaceRepository.Setup(r => r.GetByIdAsync(workspace.Id, It.IsAny<CancellationToken>())).ReturnsAsync(workspace);
        _workspaceAccessService.Setup(s => s.HasAccessAsync(workspace.Id, It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var result = await CreateHandler().Handle(
            new GetWorkspaceByIdQuery(workspace.Id, Guid.NewGuid()), CancellationToken.None);

        Assert.True(result.IsFailed);
    }
}
