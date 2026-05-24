using System.ComponentModel.DataAnnotations;

namespace Speakat.Application.Auth.Dtos;

public class CheckNicknameRequestDto
{
    [Required]
    [MaxLength(50)]
    public required string Nickname { get; set; }
}