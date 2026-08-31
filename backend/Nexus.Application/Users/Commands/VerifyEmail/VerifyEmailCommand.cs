using Nexus.Application.Common;
using FluentResults;
using MediatR;

namespace Nexus.Application.Users.Commands.VerifyEmail;

public sealed record VerifyEmailCommand(string Email, string Code) : IRequest<Result<AuthResult>>;
