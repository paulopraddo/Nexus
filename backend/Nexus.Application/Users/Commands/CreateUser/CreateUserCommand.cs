using Nexus.Application.Common;
using FluentResults;
using MediatR;

namespace Nexus.Application.Users.Commands.CreateUser;

public sealed record CreateUserCommand(string Username, string Email, string Password) : IRequest<Result<RegisterResult>>;
