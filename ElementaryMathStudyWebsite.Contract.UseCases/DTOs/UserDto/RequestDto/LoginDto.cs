﻿namespace ElementaryMathStudyWebsite.Contract.UseCases.DTOs.UserDto.RequestDto
{
    public class LoginDto
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
    }
}
