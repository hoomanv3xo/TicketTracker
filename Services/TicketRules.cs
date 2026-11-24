using TicketTracker.Models;

namespace TicketTracker.Services;

public static class TicketRules
{
    public static void ApplyStatus(Ticket ticket, TicketStatus newStatus, DateTime utcNow)
    {
        ticket.Status = newStatus;
        bool isClosed = newStatus is TicketStatus.Resolved or TicketStatus.Closed;
        ticket.ClosedAt = isClosed ? (ticket.ClosedAt ?? utcNow) : null;
    }
}