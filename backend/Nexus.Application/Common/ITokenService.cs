using Nexus.Domain.Users;

namespace Nexus.Application.Common;

public interface ITokenService
{
    string GenerateToken(User user);
}
