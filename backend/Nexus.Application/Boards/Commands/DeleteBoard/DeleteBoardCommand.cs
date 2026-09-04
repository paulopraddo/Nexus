using FluentResults;
using MediatR;

namespace Nexus.Application.Boards.Commands.DeleteBoard;

public sealed record DeleteBoardCommand(Guid BoardId, Guid CurrentUserId) : IRequest<Result>;
