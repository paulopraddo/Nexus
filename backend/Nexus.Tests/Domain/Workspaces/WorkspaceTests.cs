using Nexus.Domain.Workspaces;

namespace Nexus.Tests.Domain.Workspaces;

public class WorkspaceTests
{
    [Fact]
    public void Create_ComDadosValidos_RetornaSucesso()
    {
        var ownerId = Guid.NewGuid();
        var result = Workspace.Create(WorkspaceName.Create("Time").Value, ownerId);

        Assert.True(result.IsSuccess);
        Assert.Equal(ownerId, result.Value.OwnerId);
    }

    [Fact]
    public void IsOwnedBy_ComIdDoDono_RetornaTrue()
    {
        var ownerId = Guid.NewGuid();
        var workspace = Workspace.Create(WorkspaceName.Create("Time").Value, ownerId).Value;

        Assert.True(workspace.IsOwnedBy(ownerId));
    }

    [Fact]
    public void IsOwnedBy_ComOutroId_RetornaFalse()
    {
        var workspace = Workspace.Create(WorkspaceName.Create("Time").Value, Guid.NewGuid()).Value;

        Assert.False(workspace.IsOwnedBy(Guid.NewGuid()));
    }

    [Fact]
    public void Rename_AtualizaNome()
    {
        var workspace = Workspace.Create(WorkspaceName.Create("Time").Value, Guid.NewGuid()).Value;

        workspace.Rename(WorkspaceName.Create("Novo Nome").Value);

        Assert.Equal("Novo Nome", workspace.Name.Value);
    }
}
