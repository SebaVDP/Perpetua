namespace Library.Reservations.Contracts;

// HTTP contract for the Reservations service's /reservations endpoint. Owned by Reservations.

public record ReserveRequest(Guid MemberId, Guid TitleId);
public record ReserveResponse(Guid ReservationId, string Status);
