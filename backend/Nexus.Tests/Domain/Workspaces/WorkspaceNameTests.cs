using Nexus.Domain.Workspaces;

namespace Nexus.Tests.Domain.Workspaces;

public class WorkspaceNameTests
{
    [Fact]
    public void Create_ComNomeValido_RetornaSucesso()
    {
        var result = WorkspaceName.Create("Time de Produto");

        Assert.True(result.IsSuccess);
        Assert.Equal("Time de Produto", result.Value.Value);
    }

    [Fact]
    public void Create_ComNomeVazio_RetornaFalha()
    {
        var result = WorkspaceName.Create("");

        Assert.True(result.IsFailed);
    }

    [Fact]
    public void Create_ComMaisDoQueOMaximo_RetornaFalha()
    {
        var result = WorkspaceName.Create(new string('a', WorkspaceName.MaxLength + 1));

        Assert.True(result.IsFailed);
    }

    [Fact]
    public void Create_RemoveEspacosNasExtremidades()
    {
        var result = WorkspaceName.Create("  Time  ");

        Assert.True(result.IsSuccess);
        Assert.Equal("Time", result.Value.Value);
    }
}
