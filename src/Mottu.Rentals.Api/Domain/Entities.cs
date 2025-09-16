using System.ComponentModel.DataAnnotations;

namespace Moto.Rentals.Api.Domain;

public sealed class Motorcycle
{
    public Guid Id { get; set; } = Guid.NewGuid();
    [Required] public string Identifier { get; set; } = default!;
    [Required] public int Year { get; set; }
    [Required] public string Model { get; set; } = default!;
    [Required] public string Plate { get; set; } = default!;
    public List<Rental> Rentals { get; set; } = new();
}

public sealed class Courier
{
    public Guid Id { get; set; } = Guid.NewGuid();
    [Required] public string Identifier { get; set; } = default!;
    [Required] public string Name { get; set; } = default!;
    [Required] public string Cnpj { get; set; } = default!;
    [Required] public DateOnly BirthDate { get; set; }
    [Required] public string CnhNumber { get; set; } = default!;
    [Required] public CnhType CnhType { get; set; }
    public string? CnhImageUrl { get; set; }
    public List<Rental> Rentals { get; set; } = new();
}

public sealed class Rental
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CourierId { get; set; }
    public Courier Courier { get; set; } = default!;
    public Guid MotorcycleId { get; set; }
    public Motorcycle Motorcycle { get; set; } = default!;
    public int PlanDays { get; set; }
    public decimal DailyPrice { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly ExpectedEndDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public decimal? TotalAmount { get; set; }
    public RentalStatus Status { get; set; } = RentalStatus.Active;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public sealed class YearNotification
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid MotorcycleId { get; set; }
    public int Year { get; set; }
    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
}
