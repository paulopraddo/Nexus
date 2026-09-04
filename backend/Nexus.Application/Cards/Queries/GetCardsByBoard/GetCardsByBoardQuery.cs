using Nexus.Application.Common;
using FluentResults;
using MediatR;

namespace Nexus.Application.Cards.Queries.GetCardsByBoard;

public sealed record GetCardsByBoardQuery(Guid BoardId, Guid CurrentUserId) : IRequest<Result<IReadOnlyCollection<CardResult>>>;
