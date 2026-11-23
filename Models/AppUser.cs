namespace TicketTracker.Models;

public enum UserRole { Requester, Technician }

public class AppUser
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public UserRole Role { get; set; }
}