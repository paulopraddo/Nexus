using Nexus.Application.Common;
using FluentResults;
using MediatR;

namespace Nexus.Application.Cards.Commands.CreateCard;

public sealed record CreateCardCommand(Guid BoardId, Guid CurrentUserId, string Title) : IRequest<Result<CardResult>>;
