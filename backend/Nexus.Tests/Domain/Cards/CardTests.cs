using Nexus.Domain.Cards;

namespace Nexus.Tests.Domain.Cards;

public class CardTests
{
    [Fact]
    public void Create_ComDadosValidos_RetornaSucesso()
    {
        var boardId = Guid.NewGuid();
        var result = Card.Create(CardTitle.Create("Tarefa 1").Value, boardId);

        Assert.True(result.IsSuccess);
        Assert.Equal(boardId, result.Value.BoardId);
    }

    [Fact]
    public void Rename_AtualizaTitulo()
    {
        var card = Card.Create(CardTitle.Create("Tarefa 1").Value, Guid.NewGuid()).Value;

        card.Rename(CardTitle.Create("Tarefa 2").Value);

        Assert.Equal("Tarefa 2", card.Title.Value);
    }
}
