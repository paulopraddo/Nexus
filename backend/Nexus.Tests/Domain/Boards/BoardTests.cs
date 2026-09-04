using Nexus.Domain.Boards;

namespace Nexus.Tests.Domain.Boards;

public class BoardTests
{
    [Fact]
    public void Create_ComDadosValidos_RetornaSucesso()
    {
        var workspaceId = Guid.NewGuid();
        var result = Board.Create(BoardName.Create("Sprint 1").Value, workspaceId);

        Assert.True(result.IsSuccess);
        Assert.Equal(workspaceId, result.Value.WorkspaceId);
    }

    [Fact]
    public void Rename_AtualizaNome()
    {
        var board = Board.Create(BoardName.Create("Sprint 1").Value, Guid.NewGuid()).Value;

        board.Rename(BoardName.Create("Sprint 2").Value);

        Assert.Equal("Sprint 2", board.Name.Value);
    }
}
