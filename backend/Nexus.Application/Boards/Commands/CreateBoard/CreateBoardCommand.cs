using Nexus.Application.Common;
using FluentResults;
using MediatR;

namespace Nexus.Application.Boards.Commands.CreateBoard;

public sealed record CreateBoardCommand(Guid WorkspaceId, Guid CurrentUserId, string Name) : IRequest<Result<BoardResult>>;
