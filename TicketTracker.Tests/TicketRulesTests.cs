using System.Net.Sockets;
using TicketTracker.Models;
using TicketTracker.Services;

namespace TicketTracker.Tests;

public class TicketRulesTests
{
    private static readonly DateTime Now =
        new(2026, 11, 23, 22, 44, 0, DateTimeKind.Utc);

    [Fact]
    public void Resolving_an_open_ticket_sets_ClosedAt()
    {
        var t = new Ticket { Status = TicketStatus.Open };

        TicketRules.ApplyStatus(t, TicketStatus.Resolved, Now);

        Assert.Equal(TicketStatus.Resolved, t.Status);
        Assert.Equal(Now, t.ClosedAt);
    }

    [Fact]
    public void Closing_a_resolved_ticket_keeps_the_original_ClosedAt()
    {
        var firstClosed = Now.AddHours(-2);
        var t = new Ticket { Status = TicketStatus.Resolved, ClosedAt = firstClosed };

        TicketRules.ApplyStatus(t, TicketStatus.Closed, Now);

        Assert.Equal(firstClosed, t.ClosedAt);
    }

    [Fact]
    public void Reopening_a_closed_ticket_clears_ClosedAt()
    {
        var t = new Ticket { Status = TicketStatus.Closed, ClosedAt = Now.AddHours(-1) };

        TicketRules.ApplyStatus(t, TicketStatus.Open, Now);

        Assert.Equal(TicketStatus.Open, t.Status);
        Assert.Null(t.ClosedAt);
    }

    [Theory]
    [InlineData(TicketStatus.Open)]
    [InlineData(TicketStatus.InProgress)]
    public void Non_closed_statuses_leave_ClosedAt_empty(TicketStatus status)
    {
        var t = new Ticket();

        TicketRules.ApplyStatus(t, status, Now);

        Assert.Null(t.ClosedAt);
    }
}