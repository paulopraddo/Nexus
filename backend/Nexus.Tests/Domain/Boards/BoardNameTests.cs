using Nexus.Domain.Boards;

namespace Nexus.Tests.Domain.Boards;

public class BoardNameTests
{
    [Fact]
    public void Create_ComNomeValido_RetornaSucesso()
    {
        var result = BoardName.Create("Sprint 1");

        Assert.True(result.IsSuccess);
        Assert.Equal("Sprint 1", result.Value.Value);
    }

    [Fact]
    public void Create_ComNomeVazio_RetornaFalha()
    {
        var result = BoardName.Create("   ");

        Assert.True(result.IsFailed);
    }

    [Fact]
    public void Create_ComMaisDoQueOMaximo_RetornaFalha()
    {
        var result = BoardName.Create(new string('a', BoardName.MaxLength + 1));

        Assert.True(result.IsFailed);
    }
}
