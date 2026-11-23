-- 02_tsql_objects.sql
-- T-SQL objects for the TicketTracker database (safe to re-run)
USE TicketTracker;
GO

-- INDEXES
DROP INDEX IF EXISTS IX_Tickets_Status_Covering ON dbo.Tickets;
CREATE INDEX IX_Tickets_Status_Covering
    ON dbo.Tickets (Status) INCLUDE (Priority, CreatedAt, ClosedAt);

DROP INDEX IF EXISTS IX_TicketComments_TicketId_CreatedAt ON dbo.TicketComments;
CREATE INDEX IX_TicketComments_TicketId_CreatedAt
    ON dbo.TicketComments (TicketId, CreatedAt);
GO

-- STORED PROCEDURE: dashboard numbers
CREATE OR ALTER PROCEDURE dbo.usp_TicketStats
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        COUNT(*) AS TotalTickets,
        SUM(CASE WHEN Status = 'Open' THEN 1 ELSE 0 END) AS OpenTickets,
        SUM(CASE WHEN Status = 'InProgress' THEN 1 ELSE 0 END) AS InProgressTickets,
        SUM(CASE WHEN Status IN ('Resolved', 'Closed') THEN 1 ELSE 0 END) AS ClosedTickets,
        AVG(CASE WHEN ClosedAt IS NOT NULL
                 THEN CAST(DATEDIFF(MINUTE, CreatedAt, ClosedAt) AS float) END) AS AvgMinutesToResolve
    FROM dbo.Tickets;
END;
GO

-- STORED PROCEDURE: filtered list with an optional parameter
CREATE OR ALTER PROCEDURE dbo.usp_GetTickets
    @Status NVARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Title, Status, Priority, CreatedAt, ClosedAt
    FROM dbo.Tickets
    WHERE (@Status IS NULL OR Status = @Status)
    ORDER BY CreatedAt DESC;
END;
GO

-- VIEW: latest comment per ticket (CTE + ROW_NUMBER window function)
CREATE OR ALTER VIEW dbo.vw_TicketLatestComment
AS
WITH ranked AS (
    SELECT c.Id, c.TicketId, c.UserId, c.Body, c.CreatedAt,
           ROW_NUMBER() OVER (PARTITION BY c.TicketId
                              ORDER BY c.CreatedAt DESC, c.Id DESC) AS rn
    FROM dbo.TicketComments c
)
SELECT t.Id AS TicketId,
       t.Title,
       t.Status,
       u.Name AS LatestCommenter,
       r.Body AS LatestComment,
       r.CreatedAt AS LatestCommentAt
FROM dbo.Tickets t
LEFT JOIN ranked r ON r.TicketId = t.Id AND r.rn = 1
LEFT JOIN dbo.Users u ON u.Id = r.UserId;
GO