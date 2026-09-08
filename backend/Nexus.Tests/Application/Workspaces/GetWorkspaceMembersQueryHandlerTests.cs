using Nexus.Application.Workspaces.Queries.GetWorkspaceMembers;
using Nexus.Domain.Users;
using Nexus.Domain.Workspaces;
using Moq;

namespace Nexus.Tests.Application.Workspaces;

public class GetWorkspaceMembersQueryHandlerTests
{
    private readonly Mock<IWorkspaceRepository> _workspaceRepository = new();
    private readonly Mock<IWorkspaceMemberRepository> _memberRepository = new();
    private readonly Mock<IUserRepository> _userRepository = new();

    private GetWorkspaceMembersQueryHandler CreateHandler() =>
        new(_workspaceRepository.Object, _memberRepository.Object, _userRepository.Object);

    private static User CreateUser(Guid? id = null, string username = "usuario", string email = "usuario@example.com")
    {
        var user = User.Create(Username.Create(username).Value, Email.Create(email).Value, "hash").Value;
        return user;
    }

    [Fact]
    public async Task Handle_QuandoDono_RetornaDonoEMembros()
    {
        var owner = CreateUser(username: "dono", email: "dono@example.com");
        var workspace = Workspace.Create(WorkspaceName.Create("Time").Value, owner.Id).Value;
        var memberUser = CreateUser(username: "membro", email: "membro@example.com");
        var member = WorkspaceMember.Create(workspace.Id, memberUser.Id).Value;

        _workspaceRepository.Setup(r => r.GetByIdAsync(workspace.Id, It.IsAny<CancellationToken>())).ReturnsAsync(workspace);
        _memberRepository.Setup(r => r.GetByWorkspaceIdAsync(workspace.Id, It.IsAny<CancellationToken>())).ReturnsAsync([member]);
        _userRepository.Setup(r => r.GetByIdsAsync(It.IsAny<IReadOnlyCollection<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([owner, memberUser]);

        var result = await CreateHandler().Handle(new GetWorkspaceMembersQuery(workspace.Id, owner.Id), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Count);
    }

    [Fact]
    public async Task Handle_QuandoNaoEDonoNemMembro_RetornaFalha()
    {
        var workspace = Workspace.Create(WorkspaceName.Create("Time").Value, Guid.NewGuid()).Value;
        _workspaceRepository.Setup(r => r.GetByIdAsync(workspace.Id, It.IsAny<CancellationToken>())).ReturnsAsync(workspace);
        _memberRepository.Setup(r => r.ExistsAsync(workspace.Id, It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var result = await CreateHandler().Handle(
            new GetWorkspaceMembersQuery(workspace.Id, Guid.NewGuid()), CancellationToken.None);

        Assert.True(result.IsFailed);
    }
}
