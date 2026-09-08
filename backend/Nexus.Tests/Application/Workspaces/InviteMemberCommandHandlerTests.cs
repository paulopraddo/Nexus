using Nexus.Application.Workspaces.Commands.InviteMember;
using Nexus.Domain.Common;
using Nexus.Domain.Users;
using Nexus.Domain.Workspaces;
using Moq;

namespace Nexus.Tests.Application.Workspaces;

public class InviteMemberCommandHandlerTests
{
    private readonly Mock<IWorkspaceRepository> _workspaceRepository = new();
    private readonly Mock<IWorkspaceMemberRepository> _memberRepository = new();
    private readonly Mock<IUserRepository> _userRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private InviteMemberCommandHandler CreateHandler() =>
        new(_workspaceRepository.Object, _memberRepository.Object, _userRepository.Object, _unitOfWork.Object);

    private static User CreateUser(string email = "convidado@example.com") =>
        User.Create(Username.Create("convidado").Value, Email.Create(email).Value, "hash").Value;

    [Fact]
    public async Task Handle_ComUsuarioExistenteENaoMembro_AdicionaMembro()
    {
        var ownerId = Guid.NewGuid();
        var workspace = Workspace.Create(WorkspaceName.Create("Time").Value, ownerId).Value;
        var invitedUser = CreateUser();
        _workspaceRepository.Setup(r => r.GetByIdAsync(workspace.Id, It.IsAny<CancellationToken>())).ReturnsAsync(workspace);
        _userRepository.Setup(r => r.GetByEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>())).ReturnsAsync(invitedUser);
        _memberRepository.Setup(r => r.ExistsAsync(workspace.Id, invitedUser.Id, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var result = await CreateHandler().Handle(
            new InviteMemberCommand(workspace.Id, ownerId, "convidado@example.com"), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(invitedUser.Id, result.Value.UserId);
        _memberRepository.Verify(r => r.AddAsync(It.IsAny<WorkspaceMember>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_QuandoNaoEDono_RetornaFalha()
    {
        var workspace = Workspace.Create(WorkspaceName.Create("Time").Value, Guid.NewGuid()).Value;
        _workspaceRepository.Setup(r => r.GetByIdAsync(workspace.Id, It.IsAny<CancellationToken>())).ReturnsAsync(workspace);

        var result = await CreateHandler().Handle(
            new InviteMemberCommand(workspace.Id, Guid.NewGuid(), "convidado@example.com"), CancellationToken.None);

        Assert.True(result.IsFailed);
        _memberRepository.Verify(r => r.AddAsync(It.IsAny<WorkspaceMember>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ComUsuarioInexistente_RetornaFalha()
    {
        var ownerId = Guid.NewGuid();
        var workspace = Workspace.Create(WorkspaceName.Create("Time").Value, ownerId).Value;
        _workspaceRepository.Setup(r => r.GetByIdAsync(workspace.Id, It.IsAny<CancellationToken>())).ReturnsAsync(workspace);
        _userRepository.Setup(r => r.GetByEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);

        var result = await CreateHandler().Handle(
            new InviteMemberCommand(workspace.Id, ownerId, "naoexiste@example.com"), CancellationToken.None);

        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task Handle_ComUsuarioJaMembro_RetornaFalha()
    {
        var ownerId = Guid.NewGuid();
        var workspace = Workspace.Create(WorkspaceName.Create("Time").Value, ownerId).Value;
        var invitedUser = CreateUser();
        _workspaceRepository.Setup(r => r.GetByIdAsync(workspace.Id, It.IsAny<CancellationToken>())).ReturnsAsync(workspace);
        _userRepository.Setup(r => r.GetByEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>())).ReturnsAsync(invitedUser);
        _memberRepository.Setup(r => r.ExistsAsync(workspace.Id, invitedUser.Id, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var result = await CreateHandler().Handle(
            new InviteMemberCommand(workspace.Id, ownerId, "convidado@example.com"), CancellationToken.None);

        Assert.True(result.IsFailed);
        _memberRepository.Verify(r => r.AddAsync(It.IsAny<WorkspaceMember>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
