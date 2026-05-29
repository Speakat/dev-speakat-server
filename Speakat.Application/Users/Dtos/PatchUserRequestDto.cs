using System.ComponentModel.DataAnnotations;

namespace Speakat.Application.Users.Dtos;

public class PatchUserRequestDto
{
    [MaxLength(50)]
    public string? Nickname { get; set; }

    public string? ProfileImageKey { get; set; }
}
