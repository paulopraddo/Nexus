using Nexus.Application.Workspaces.Commands.DeleteWorkspace;
using Nexus.Domain.Common;
using Nexus.Domain.Workspaces;
using Moq;

namespace Nexus.Tests.Application.Workspaces;

public class DeleteWorkspaceCommandHandlerTests
{
    private readonly Mock<IWorkspaceRepository> _workspaceRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private DeleteWorkspaceCommandHandler CreateHandler() =>
        new(_workspaceRepository.Object, _unitOfWork.Object);

    [Fact]
    public async Task Handle_QuandoDono_RemoveWorkspace()
    {
        var ownerId = Guid.NewGuid();
        var workspace = Workspace.Create(WorkspaceName.Create("Time").Value, ownerId).Value;
        _workspaceRepository.Setup(r => r.GetByIdAsync(workspace.Id, It.IsAny<CancellationToken>())).ReturnsAsync(workspace);

        var result = await CreateHandler().Handle(new DeleteWorkspaceCommand(workspace.Id, ownerId), CancellationToken.None);

        Assert.True(result.IsSuccess);
        _workspaceRepository.Verify(r => r.Remove(workspace), Times.Once);
    }

    [Fact]
    public async Task Handle_QuandoNaoEDono_RetornaFalhaSemRemover()
    {
        var workspace = Workspace.Create(WorkspaceName.Create("Time").Value, Guid.NewGuid()).Value;
        _workspaceRepository.Setup(r => r.GetByIdAsync(workspace.Id, It.IsAny<CancellationToken>())).ReturnsAsync(workspace);

        var result = await CreateHandler().Handle(
            new DeleteWorkspaceCommand(workspace.Id, Guid.NewGuid()), CancellationToken.None);

        Assert.True(result.IsFailed);
        _workspaceRepository.Verify(r => r.Remove(It.IsAny<Workspace>()), Times.Never);
    }
}
