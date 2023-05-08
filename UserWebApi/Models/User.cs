using System.ComponentModel.DataAnnotations;


namespace UserWebApi.Models;

public class User : IEquatable<User>
{
    [Key]
    public  int Id { get; set; }
    
    [Required, MinLength(4), MaxLength(16)]
    public string Login { get; set; }
    
    [Required, MinLength(8), MaxLength(20)]
    public string Password { get; set; }
    
    [Required]
    public DateTime CreatedDate { get; set; }
    
    [Required]
    public UserGroup UserGroup { get; set; }
    
    [Required]
    public UserState UserState { get; set; }

    public bool Equals(User? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id == other.Id && 
               Login == other.Login && 
               Password == other.Password &&
               CreatedDate.Equals(other.CreatedDate) && 
               UserGroup.Equals(other.UserGroup) && 
               UserState.Equals(other.UserState);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((User)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Login, Password, CreatedDate, UserGroup, UserState);
    }
}