using Nexus.Application.Cards.Commands.CreateCard;
using Nexus.Application.Cards.Commands.DeleteCard;
using Nexus.Application.Cards.Commands.RenameCard;
using Nexus.Application.Cards.Queries.GetCardsByBoard;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Nexus.API.Controllers;

[ApiController]
[Authorize]
public sealed class CardsController(ISender sender) : ControllerBase
{
    [HttpGet("api/boards/{boardId:guid}/cards")]
    public async Task<IActionResult> GetByBoard(Guid boardId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetCardsByBoardQuery(boardId, User.GetUserId()), cancellationToken);

        if (result.IsFailed)
        {
            return NotFound(result.Errors.Select(e => e.Message));
        }

        return Ok(result.Value);
    }

    [HttpPost("api/boards/{boardId:guid}/cards")]
    public async Task<IActionResult> Create(Guid boardId, CreateCardRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new CreateCardCommand(boardId, User.GetUserId(), request.Title), cancellationToken);

        if (result.IsFailed)
        {
            return BadRequest(result.Errors.Select(e => e.Message));
        }

        return Ok(result.Value);
    }

    [HttpPut("api/cards/{id:guid}")]
    public async Task<IActionResult> Rename(Guid id, RenameCardRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new RenameCardCommand(id, User.GetUserId(), request.Title), cancellationToken);

        if (result.IsFailed)
        {
            return NotFound(result.Errors.Select(e => e.Message));
        }

        return Ok(result.Value);
    }

    [HttpDelete("api/cards/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new DeleteCardCommand(id, User.GetUserId()), cancellationToken);

        if (result.IsFailed)
        {
            return NotFound(result.Errors.Select(e => e.Message));
        }

        return NoContent();
    }
}

public sealed record CreateCardRequest(string Title);

public sealed record RenameCardRequest(string Title);
