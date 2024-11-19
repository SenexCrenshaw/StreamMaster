namespace StreamMaster.Domain.Models;

public class User
{
    public int Id { get; set; }
    public Guid Identifier { get; set; }
    public required string Password { get; set; }
    public required string Username { get; set; }
}
