namespace Moto.Rentals.Api.Dtos;

public sealed record CreateRentalRequest(Guid CourierId, Guid MotorcycleId, int PlanDays);
public sealed record RentalResponse(Guid Id, Guid CourierId, Guid MotorcycleId, int PlanDays, decimal DailyPrice,
    DateOnly StartDate, DateOnly ExpectedEndDate, DateOnly? EndDate, decimal? TotalAmount, string Status);

public sealed record ReturnRentalRequest(DateOnly ReturnDate);
public sealed record ReturnRentalResponse(Guid RentalId, int UsedDays, int RemainingDays, int ExtraDays, decimal Total);
