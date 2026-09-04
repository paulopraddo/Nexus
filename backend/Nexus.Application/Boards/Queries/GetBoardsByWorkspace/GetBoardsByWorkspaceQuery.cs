using Nexus.Application.Common;
using FluentResults;
using MediatR;

namespace Nexus.Application.Boards.Queries.GetBoardsByWorkspace;

public sealed record GetBoardsByWorkspaceQuery(Guid WorkspaceId, Guid CurrentUserId)
    : IRequest<Result<IReadOnlyCollection<BoardResult>>>;
