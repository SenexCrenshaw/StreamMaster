namespace StreamMasterDomain.Models;

public class User
{
    public int Id { get; set; }
    public Guid Identifier { get; set; }
    public string Password { get; set; }
    public string Username { get; set; }
}
