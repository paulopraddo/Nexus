using Nexus.Application.Workspaces.Commands.CreateWorkspace;
using Nexus.Domain.Common;
using Nexus.Domain.Workspaces;
using Moq;

namespace Nexus.Tests.Application.Workspaces;

public class CreateWorkspaceCommandHandlerTests
{
    private readonly Mock<IWorkspaceRepository> _workspaceRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private CreateWorkspaceCommandHandler CreateHandler() =>
        new(_workspaceRepository.Object, _unitOfWork.Object);

    [Fact]
    public async Task Handle_ComNomeValido_CriaWorkspace()
    {
        var ownerId = Guid.NewGuid();

        var result = await CreateHandler().Handle(new CreateWorkspaceCommand(ownerId, "Time"), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("Time", result.Value.Name);
        _workspaceRepository.Verify(r => r.AddAsync(It.IsAny<Workspace>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ComNomeInvalido_RetornaFalhaSemPersistir()
    {
        var result = await CreateHandler().Handle(new CreateWorkspaceCommand(Guid.NewGuid(), ""), CancellationToken.None);

        Assert.True(result.IsFailed);
        _workspaceRepository.Verify(r => r.AddAsync(It.IsAny<Workspace>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
