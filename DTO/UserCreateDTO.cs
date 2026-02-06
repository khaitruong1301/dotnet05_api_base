public class UserCreateDTO
{
    
    public string Username { get; set; } = null!;

    public string? FullName { get; set; }

    public string Email { get; set; } = null!;

    public string? Phone { get; set; }

    public string? Avatar { get; set; }

    public string Password { get; set; } = null!;

    public string? Address { get; set; }
}