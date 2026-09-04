using FluentResults;
using MediatR;

namespace Nexus.Application.Users.Commands.ResendVerificationCode;

public sealed record ResendVerificationCodeCommand(string Email) : IRequest<Result>;
