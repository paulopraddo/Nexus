using Nexus.Domain.Common;
using FluentResults;

namespace Nexus.Domain.Cards;

public sealed class Card : Entity
{
    public CardTitle Title { get; private set; }
    public Guid BoardId { get; }
    public DateTime CreatedAt { get; }

    private Card(Guid id, CardTitle title, Guid boardId, DateTime createdAt)
        : base(id)
    {
        Title = title;
        BoardId = boardId;
        CreatedAt = createdAt;
    }

    public static Result<Card> Create(CardTitle title, Guid boardId)
    {
        return Result.Ok(new Card(Guid.NewGuid(), title, boardId, DateTime.UtcNow));
    }

    public void Rename(CardTitle title)
    {
        Title = title;
    }
}
