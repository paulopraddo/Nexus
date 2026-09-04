using Nexus.Application.Common;
using FluentResults;
using MediatR;

namespace Nexus.Application.Cards.Commands.RenameCard;

public sealed record RenameCardCommand(Guid CardId, Guid CurrentUserId, string Title) : IRequest<Result<CardResult>>;
