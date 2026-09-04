using FluentResults;
using MediatR;

namespace Nexus.Application.Users.Commands.ResetPassword;

public sealed record ResetPasswordCommand(string Email, string Code, string NewPassword) : IRequest<Result>;
