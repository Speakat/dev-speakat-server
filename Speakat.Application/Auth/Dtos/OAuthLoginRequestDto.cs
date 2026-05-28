using System.ComponentModel.DataAnnotations;

namespace Speakat.Application.Auth.Dtos;

public class OAuthLoginRequestDto
{
    [Required]
    public required string AuthorizationCode { get; set; } 
}