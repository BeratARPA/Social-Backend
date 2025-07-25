﻿using AuthService.Dtos;
using MediatR;

namespace AuthService.Commands.Register
{
    public record RegisterCommand(string Username, string Password, string Email) : IRequest<AuthResultDto>;
}
