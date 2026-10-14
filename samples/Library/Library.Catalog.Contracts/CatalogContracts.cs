namespace Library.Catalog.Contracts;

/// <summary>
/// Command telling Catalog to hold a copy of a title for a member.
/// Sent point-to-point (Send), so it is owned by its receiver: Catalog.
/// </summary>
public record ReserveCopy(Guid ReservationId, Guid MemberId, Guid TitleId);
