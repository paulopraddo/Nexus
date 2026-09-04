using FluentResults;
using MediatR;

namespace Nexus.Application.Users.Commands.ForgotPassword;

public sealed record ForgotPasswordCommand(string Email) : IRequest<Result>;
