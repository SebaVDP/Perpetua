namespace Library.Loans.Contracts;

// Contracts owned by the Loans service.

// HTTP contract for the Loans service's /loans endpoint.
public record BorrowRequest(Guid MemberId, Guid BookId);
public record BorrowResponse(Guid LoanId, string Status);

/// <summary>
/// Event published by Loans once a book has been borrowed.
/// Consumed independently by Catalog, Notifications and Reservations (fan-out).
/// </summary>
public record BookBorrowed(Guid LoanId, Guid MemberId, Guid BookId, DateTime BorrowedAtUtc);
