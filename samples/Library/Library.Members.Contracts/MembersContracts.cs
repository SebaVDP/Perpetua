namespace Library.Members.Contracts;

// HTTP contract for the Members service's /members/check-rights endpoint. Owned by Members.

public record BorrowingRightsRequest(Guid MemberId);
public record BorrowingRightsResponse(bool Allowed, string Reason);
