using Nexus.Application.Workspaces.Queries.GetWorkspaceById;
using Nexus.Domain.Workspaces;
using Moq;

namespace Nexus.Tests.Application.Workspaces;

public class GetWorkspaceByIdQueryHandlerTests
{
    private readonly Mock<IWorkspaceRepository> _workspaceRepository = new();

    private GetWorkspaceByIdQueryHandler CreateHandler() => new(_workspaceRepository.Object);

    [Fact]
    public async Task Handle_QuandoDono_RetornaWorkspace()
    {
        var ownerId = Guid.NewGuid();
        var workspace = Workspace.Create(WorkspaceName.Create("Time").Value, ownerId).Value;
        _workspaceRepository.Setup(r => r.GetByIdAsync(workspace.Id, It.IsAny<CancellationToken>())).ReturnsAsync(workspace);

        var result = await CreateHandler().Handle(new GetWorkspaceByIdQuery(workspace.Id, ownerId), CancellationToken.None);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_QuandoNaoEDono_RetornaFalha()
    {
        var workspace = Workspace.Create(WorkspaceName.Create("Time").Value, Guid.NewGuid()).Value;
        _workspaceRepository.Setup(r => r.GetByIdAsync(workspace.Id, It.IsAny<CancellationToken>())).ReturnsAsync(workspace);

        var result = await CreateHandler().Handle(
            new GetWorkspaceByIdQuery(workspace.Id, Guid.NewGuid()), CancellationToken.None);

        Assert.True(result.IsFailed);
    }
}
