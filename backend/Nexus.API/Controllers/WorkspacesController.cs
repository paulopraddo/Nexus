using Nexus.Application.Workspaces.Commands.CreateWorkspace;
using Nexus.Application.Workspaces.Commands.DeleteWorkspace;
using Nexus.Application.Workspaces.Commands.RenameWorkspace;
using Nexus.Application.Workspaces.Queries.GetWorkspaceById;
using Nexus.Application.Workspaces.Queries.GetWorkspaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Nexus.API.Controllers;

[ApiController]
[Authorize]
[Route("api/workspaces")]
public sealed class WorkspacesController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetWorkspacesQuery(User.GetUserId()), cancellationToken);

        return Ok(result.Value);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetWorkspaceByIdQuery(id, User.GetUserId()), cancellationToken);

        if (result.IsFailed)
        {
            return NotFound(result.Errors.Select(e => e.Message));
        }

        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateWorkspaceRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new CreateWorkspaceCommand(User.GetUserId(), request.Name), cancellationToken);

        if (result.IsFailed)
        {
            return BadRequest(result.Errors.Select(e => e.Message));
        }

        return Ok(result.Value);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Rename(Guid id, RenameWorkspaceRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new RenameWorkspaceCommand(id, User.GetUserId(), request.Name), cancellationToken);

        if (result.IsFailed)
        {
            return NotFound(result.Errors.Select(e => e.Message));
        }

        return Ok(result.Value);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new DeleteWorkspaceCommand(id, User.GetUserId()), cancellationToken);

        if (result.IsFailed)
        {
            return NotFound(result.Errors.Select(e => e.Message));
        }

        return NoContent();
    }
}

public sealed record CreateWorkspaceRequest(string Name);

public sealed record RenameWorkspaceRequest(string Name);
