using Nexus.Domain.Workspaces;

namespace Nexus.Tests.Domain.Workspaces;

public class WorkspaceMemberTests
{
    [Fact]
    public void Create_ComDadosValidos_RetornaSucesso()
    {
        var workspaceId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var result = WorkspaceMember.Create(workspaceId, userId);

        Assert.True(result.IsSuccess);
        Assert.Equal(workspaceId, result.Value.WorkspaceId);
        Assert.Equal(userId, result.Value.UserId);
    }
}
