using Nexus.Application.Common;
using FluentResults;
using MediatR;

namespace Nexus.Application.Users.Commands.Login;

public sealed record LoginCommand(string Email, string Password) : IRequest<Result<AuthResult>>;
