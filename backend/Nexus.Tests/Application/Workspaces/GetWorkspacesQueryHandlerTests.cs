using Nexus.Application.Workspaces.Queries.GetWorkspaces;
using Nexus.Domain.Workspaces;
using Moq;

namespace Nexus.Tests.Application.Workspaces;

public class GetWorkspacesQueryHandlerTests
{
    private readonly Mock<IWorkspaceRepository> _workspaceRepository = new();
    private readonly Mock<IWorkspaceMemberRepository> _memberRepository = new();

    private GetWorkspacesQueryHandler CreateHandler() => new(_workspaceRepository.Object, _memberRepository.Object);

    [Fact]
    public async Task Handle_RetornaWorkspacesProprias()
    {
        var ownerId = Guid.NewGuid();
        var workspace = Workspace.Create(WorkspaceName.Create("Time").Value, ownerId).Value;
        _workspaceRepository.Setup(r => r.GetByOwnerIdAsync(ownerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([workspace]);
        _memberRepository.Setup(r => r.GetByUserIdAsync(ownerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var result = await CreateHandler().Handle(new GetWorkspacesQuery(ownerId), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value);
    }

    [Fact]
    public async Task Handle_IncluiWorkspacesOndeEMembro()
    {
        var userId = Guid.NewGuid();
        var otherOwnerWorkspace = Workspace.Create(WorkspaceName.Create("Time do Fulano").Value, Guid.NewGuid()).Value;
        var membership = WorkspaceMember.Create(otherOwnerWorkspace.Id, userId).Value;

        _workspaceRepository.Setup(r => r.GetByOwnerIdAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync([]);
        _memberRepository.Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync([membership]);
        _workspaceRepository.Setup(r => r.GetByIdsAsync(
                It.Is<IReadOnlyCollection<Guid>>(ids => ids.Contains(otherOwnerWorkspace.Id)), It.IsAny<CancellationToken>()))
            .ReturnsAsync([otherOwnerWorkspace]);

        var result = await CreateHandler().Handle(new GetWorkspacesQuery(userId), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value);
        Assert.Equal(otherOwnerWorkspace.Id, result.Value.Single().Id);
    }
}
