using Nexus.Application.Workspaces.Commands.RemoveMember;
using Nexus.Domain.Common;
using Nexus.Domain.Workspaces;
using Moq;

namespace Nexus.Tests.Application.Workspaces;

public class RemoveMemberCommandHandlerTests
{
    private readonly Mock<IWorkspaceRepository> _workspaceRepository = new();
    private readonly Mock<IWorkspaceMemberRepository> _memberRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private RemoveMemberCommandHandler CreateHandler() =>
        new(_workspaceRepository.Object, _memberRepository.Object, _unitOfWork.Object);

    [Fact]
    public async Task Handle_QuandoDonoRemoveMembro_RemoveComSucesso()
    {
        var ownerId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        var workspace = Workspace.Create(WorkspaceName.Create("Time").Value, ownerId).Value;
        var member = WorkspaceMember.Create(workspace.Id, memberId).Value;
        _workspaceRepository.Setup(r => r.GetByIdAsync(workspace.Id, It.IsAny<CancellationToken>())).ReturnsAsync(workspace);
        _memberRepository.Setup(r => r.GetAsync(workspace.Id, memberId, It.IsAny<CancellationToken>())).ReturnsAsync(member);

        var result = await CreateHandler().Handle(
            new RemoveMemberCommand(workspace.Id, ownerId, memberId), CancellationToken.None);

        Assert.True(result.IsSuccess);
        _memberRepository.Verify(r => r.Remove(member), Times.Once);
    }

    [Fact]
    public async Task Handle_QuandoNaoEDono_RetornaFalha()
    {
        var workspace = Workspace.Create(WorkspaceName.Create("Time").Value, Guid.NewGuid()).Value;
        _workspaceRepository.Setup(r => r.GetByIdAsync(workspace.Id, It.IsAny<CancellationToken>())).ReturnsAsync(workspace);

        var result = await CreateHandler().Handle(
            new RemoveMemberCommand(workspace.Id, Guid.NewGuid(), Guid.NewGuid()), CancellationToken.None);

        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task Handle_TentandoRemoverODono_RetornaFalha()
    {
        var ownerId = Guid.NewGuid();
        var workspace = Workspace.Create(WorkspaceName.Create("Time").Value, ownerId).Value;
        _workspaceRepository.Setup(r => r.GetByIdAsync(workspace.Id, It.IsAny<CancellationToken>())).ReturnsAsync(workspace);

        var result = await CreateHandler().Handle(
            new RemoveMemberCommand(workspace.Id, ownerId, ownerId), CancellationToken.None);

        Assert.True(result.IsFailed);
    }
}
