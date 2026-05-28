using System.ComponentModel.DataAnnotations;

namespace Speakat.Application.Auth.Dtos;

public class RefreshTokenRequestDto
{
    [Required]
    public required string RefreshToken { get; set; }
}