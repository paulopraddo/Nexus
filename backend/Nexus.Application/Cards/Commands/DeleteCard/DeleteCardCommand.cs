using FluentResults;
using MediatR;

namespace Nexus.Application.Cards.Commands.DeleteCard;

public sealed record DeleteCardCommand(Guid CardId, Guid CurrentUserId) : IRequest<Result>;
