using Nexus.Application.Workspaces.Queries.GetWorkspaces;
using Nexus.Domain.Workspaces;
using Moq;

namespace Nexus.Tests.Application.Workspaces;

public class GetWorkspacesQueryHandlerTests
{
    private readonly Mock<IWorkspaceRepository> _workspaceRepository = new();

    private GetWorkspacesQueryHandler CreateHandler() => new(_workspaceRepository.Object);

    [Fact]
    public async Task Handle_RetornaApenasWorkspacesDoUsuario()
    {
        var ownerId = Guid.NewGuid();
        var workspace = Workspace.Create(WorkspaceName.Create("Time").Value, ownerId).Value;
        _workspaceRepository.Setup(r => r.GetByOwnerIdAsync(ownerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([workspace]);

        var result = await CreateHandler().Handle(new GetWorkspacesQuery(ownerId), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value);
    }
}
