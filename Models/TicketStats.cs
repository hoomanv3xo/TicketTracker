namespace TicketTracker.Models;

public class TicketStats
{
    public int TotalTickets { get; set; }
    public int? OpenTickets { get; set; }
    public int? InProgressTickets { get; set; }
    public int? ClosedTickets { get; set; }
    public double? AvgMinutesToResolve { get; set; }
}
