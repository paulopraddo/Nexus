using Nexus.Application.Boards.Commands.CreateBoard;
using Nexus.Application.Boards.Commands.DeleteBoard;
using Nexus.Application.Boards.Commands.RenameBoard;
using Nexus.Application.Boards.Queries.GetBoardsByWorkspace;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Nexus.API.Controllers;

[ApiController]
[Authorize]
public sealed class BoardsController(ISender sender) : ControllerBase
{
    [HttpGet("api/workspaces/{workspaceId:guid}/boards")]
    public async Task<IActionResult> GetByWorkspace(Guid workspaceId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetBoardsByWorkspaceQuery(workspaceId, User.GetUserId()), cancellationToken);

        if (result.IsFailed)
        {
            return NotFound(result.Errors.Select(e => e.Message));
        }

        return Ok(result.Value);
    }

    [HttpPost("api/workspaces/{workspaceId:guid}/boards")]
    public async Task<IActionResult> Create(Guid workspaceId, CreateBoardRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new CreateBoardCommand(workspaceId, User.GetUserId(), request.Name), cancellationToken);

        if (result.IsFailed)
        {
            return BadRequest(result.Errors.Select(e => e.Message));
        }

        return Ok(result.Value);
    }

    [HttpPut("api/boards/{id:guid}")]
    public async Task<IActionResult> Rename(Guid id, RenameBoardRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new RenameBoardCommand(id, User.GetUserId(), request.Name), cancellationToken);

        if (result.IsFailed)
        {
            return NotFound(result.Errors.Select(e => e.Message));
        }

        return Ok(result.Value);
    }

    [HttpDelete("api/boards/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new DeleteBoardCommand(id, User.GetUserId()), cancellationToken);

        if (result.IsFailed)
        {
            return NotFound(result.Errors.Select(e => e.Message));
        }

        return NoContent();
    }
}

public sealed record CreateBoardRequest(string Name);

public sealed record RenameBoardRequest(string Name);
