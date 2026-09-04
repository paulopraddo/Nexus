using Nexus.Domain.Cards;
using Microsoft.EntityFrameworkCore;

namespace Nexus.Infrastructure.Persistence.Repositories;

public sealed class CardRepository(NexusDbContext dbContext) : ICardRepository
{
    public Task<Card?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.Cards.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public async Task<IReadOnlyCollection<Card>> GetByBoardIdAsync(
        Guid boardId, CancellationToken cancellationToken = default) =>
        await dbContext.Cards.Where(c => c.BoardId == boardId).ToListAsync(cancellationToken);

    public async Task AddAsync(Card card, CancellationToken cancellationToken = default) =>
        await dbContext.Cards.AddAsync(card, cancellationToken);

    public void Remove(Card card) => dbContext.Cards.Remove(card);
}
