using System.ComponentModel.DataAnnotations;

namespace UserWebApi.Models;

public enum StateCode
{
    Active = 1,
    Blocked
}

public class UserState
{
    [Key]
    public int Id { get; init; }
    
    [Required, MaxLength(10)]
    public string Code { get; init; }
    
    [Required, MaxLength(120)]
    public string Description { get; init; }
    
    public static string StateDescription(StateCode code)
    {
        return code == StateCode.Active
            ? "Active User State"
            : "Deleted User State";
    }

    public static string CodeStringValue(StateCode code) => code.ToString();
}