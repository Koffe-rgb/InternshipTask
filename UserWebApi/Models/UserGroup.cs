using System.ComponentModel.DataAnnotations;

namespace UserWebApi.Models;

public enum GroupCode
{
    Admin = 1,
    User
}

public class UserGroup
{
    [Key]
    public int Id { get; init; }
    
    [Required, MaxLength(10)]
    public string Code { get; init; }
    
    [Required, MaxLength(120)]
    public string Description { get; init; }

    public static string GroupDescription(GroupCode code)
    {
        return code == GroupCode.Admin
            ? "Administration User Group"
            : "Ordinary User Group";
    }

    public static string CodeStringValue(GroupCode code) => code.ToString();
}