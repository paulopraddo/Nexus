using Nexus.Application.Common;
using FluentResults;
using MediatR;

namespace Nexus.Application.Boards.Commands.RenameBoard;

public sealed record RenameBoardCommand(Guid BoardId, Guid CurrentUserId, string Name) : IRequest<Result<BoardResult>>;
