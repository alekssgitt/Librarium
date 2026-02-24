namespace DefaultNamespace;

public class Member
{
    public string FirstName { get; set; } = null!;
    
    public string LastName { get; set; } = null!;
    
    [EmailAddress]
    public string Email { get; set; } = null!;
}