using System.ComponentModel.DataAnnotations;

namespace UserWebApi.Models.Dtos;

public class UserDto
{
    [Required, MinLength(4), MaxLength(16)]
    public string Login { get; set; }
    
    [Required, MinLength(8), MaxLength(20)]
    public string Password { get; set; }
    
    [Required]
    public string GroupCode { get; set; }
}