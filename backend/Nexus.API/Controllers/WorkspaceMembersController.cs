using Nexus.Application.Workspaces.Commands.InviteMember;
using Nexus.Application.Workspaces.Commands.RemoveMember;
using Nexus.Application.Workspaces.Queries.GetWorkspaceMembers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Nexus.API.Controllers;

[ApiController]
[Authorize]
public sealed class WorkspaceMembersController(ISender sender) : ControllerBase
{
    [HttpGet("api/workspaces/{workspaceId:guid}/members")]
    public async Task<IActionResult> GetMembers(Guid workspaceId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetWorkspaceMembersQuery(workspaceId, User.GetUserId()), cancellationToken);

        if (result.IsFailed)
        {
            return NotFound(result.Errors.Select(e => e.Message));
        }

        return Ok(result.Value);
    }

    [HttpPost("api/workspaces/{workspaceId:guid}/members")]
    public async Task<IActionResult> Invite(Guid workspaceId, InviteMemberRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new InviteMemberCommand(workspaceId, User.GetUserId(), request.Email), cancellationToken);

        if (result.IsFailed)
        {
            return BadRequest(result.Errors.Select(e => e.Message));
        }

        return Ok(result.Value);
    }

    [HttpDelete("api/workspaces/{workspaceId:guid}/members/{userId:guid}")]
    public async Task<IActionResult> Remove(Guid workspaceId, Guid userId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new RemoveMemberCommand(workspaceId, User.GetUserId(), userId), cancellationToken);

        if (result.IsFailed)
        {
            return BadRequest(result.Errors.Select(e => e.Message));
        }

        return NoContent();
    }
}

public sealed record InviteMemberRequest(string Email);
