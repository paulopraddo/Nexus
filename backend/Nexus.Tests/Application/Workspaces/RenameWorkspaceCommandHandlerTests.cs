using Nexus.Application.Workspaces.Commands.RenameWorkspace;
using Nexus.Domain.Common;
using Nexus.Domain.Workspaces;
using Moq;

namespace Nexus.Tests.Application.Workspaces;

public class RenameWorkspaceCommandHandlerTests
{
    private readonly Mock<IWorkspaceRepository> _workspaceRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private RenameWorkspaceCommandHandler CreateHandler() =>
        new(_workspaceRepository.Object, _unitOfWork.Object);

    [Fact]
    public async Task Handle_QuandoDono_RenomeiaWorkspace()
    {
        var ownerId = Guid.NewGuid();
        var workspace = Workspace.Create(WorkspaceName.Create("Time").Value, ownerId).Value;
        _workspaceRepository.Setup(r => r.GetByIdAsync(workspace.Id, It.IsAny<CancellationToken>())).ReturnsAsync(workspace);

        var result = await CreateHandler().Handle(
            new RenameWorkspaceCommand(workspace.Id, ownerId, "Novo Nome"), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("Novo Nome", result.Value.Name);
    }

    [Fact]
    public async Task Handle_QuandoNaoEDono_RetornaFalha()
    {
        var workspace = Workspace.Create(WorkspaceName.Create("Time").Value, Guid.NewGuid()).Value;
        _workspaceRepository.Setup(r => r.GetByIdAsync(workspace.Id, It.IsAny<CancellationToken>())).ReturnsAsync(workspace);

        var result = await CreateHandler().Handle(
            new RenameWorkspaceCommand(workspace.Id, Guid.NewGuid(), "Novo Nome"), CancellationToken.None);

        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task Handle_QuandoWorkspaceNaoExiste_RetornaFalha()
    {
        _workspaceRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Workspace?)null);

        var result = await CreateHandler().Handle(
            new RenameWorkspaceCommand(Guid.NewGuid(), Guid.NewGuid(), "Novo Nome"), CancellationToken.None);

        Assert.True(result.IsFailed);
    }
}
