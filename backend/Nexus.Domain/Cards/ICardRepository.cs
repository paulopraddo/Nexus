namespace Nexus.Domain.Cards;

public interface ICardRepository
{
    Task<Card?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Card>> GetByBoardIdAsync(Guid boardId, CancellationToken cancellationToken = default);

    Task AddAsync(Card card, CancellationToken cancellationToken = default);

    void Remove(Card card);
}
