using Nexus.Domain.Cards;

namespace Nexus.Tests.Domain.Cards;

public class CardTitleTests
{
    [Fact]
    public void Create_ComTituloValido_RetornaSucesso()
    {
        var result = CardTitle.Create("Corrigir bug de login");

        Assert.True(result.IsSuccess);
        Assert.Equal("Corrigir bug de login", result.Value.Value);
    }

    [Fact]
    public void Create_ComTituloVazio_RetornaFalha()
    {
        var result = CardTitle.Create("   ");

        Assert.True(result.IsFailed);
    }

    [Fact]
    public void Create_ComMaisDoQueOMaximo_RetornaFalha()
    {
        var result = CardTitle.Create(new string('a', CardTitle.MaxLength + 1));

        Assert.True(result.IsFailed);
    }
}
