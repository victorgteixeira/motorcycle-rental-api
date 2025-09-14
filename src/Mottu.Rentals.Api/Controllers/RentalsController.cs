using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mottu.Rentals.Api.Domain;
using Mottu.Rentals.Api.Dtos;
using Mottu.Rentals.Api.Infra;

namespace Mottu.Rentals.Api.Controllers;

[ApiController]
[Route("api/v1/rentals")]
public sealed class RentalsController(AppDbContext db) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<RentalResponse>> Create([FromBody] CreateRentalRequest req, CancellationToken ct)
    {
        var courier = await db.Couriers.FindAsync([req.CourierId], ct);
        var moto = await db.Motorcycles.FindAsync([req.MotorcycleId], ct);
        if (courier is null || moto is null) return NotFound(new { message = "Courier or Motorcycle not found" });

        if (courier.CnhType is not (CnhType.A or CnhType.APlusB))
            return BadRequest(new { message = "Courier must have CNH A or A+B" });

        var activeRental = await db.Rentals.AnyAsync(r => r.MotorcycleId == moto.Id && r.Status == RentalStatus.Active, ct);
        if (activeRental) return Conflict(new { message = "Motorcycle already rented" });

        var price = RentalPricingRules.DailyPriceFor(req.PlanDays);
        var created = DateOnly.FromDateTime(DateTime.UtcNow.Date);
        var start = created.AddDays(1);
        var expected = start.AddDays(req.PlanDays - 1);

        var rental = new Rental {
            CourierId = courier.Id, MotorcycleId = moto.Id, PlanDays = req.PlanDays, DailyPrice = price,
            StartDate = start, ExpectedEndDate = expected
        };
        db.Rentals.Add(rental);
        await db.SaveChangesAsync(ct);

        return CreatedAtAction(nameof(GetById), new { id = rental.Id }, ToResponse(rental));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<RentalResponse>> GetById(Guid id, CancellationToken ct)
    {
        var r = await db.Rentals.FindAsync([id], ct);
        return r is null ? NotFound() : ToResponse(r);
    }

    [HttpPost("{id:guid}/return")]
    public async Task<ActionResult<ReturnRentalResponse>> Return(Guid id, [FromBody] ReturnRentalRequest req, CancellationToken ct)
    {
        var r = await db.Rentals.FindAsync([id], ct);
        if (r is null) return NotFound();
        if (r.Status == RentalStatus.Closed) return Conflict(new { message = "Rental already closed" });

        var (used, remaining, extra, total) =
            RentalPricingRules.CalculateReturnTotal(r.StartDate, r.ExpectedEndDate, req.ReturnDate, r.PlanDays, r.DailyPrice);

        r.EndDate = req.ReturnDate;
        r.TotalAmount = total;
        r.Status = RentalStatus.Closed;

        await db.SaveChangesAsync(ct);
        return Ok(new ReturnRentalResponse(r.Id, used, remaining, extra, total));
    }

    private static RentalResponse ToResponse(Rental r) =>
        new(r.Id, r.CourierId, r.MotorcycleId, r.PlanDays, r.DailyPrice, r.StartDate, r.ExpectedEndDate, r.EndDate, r.TotalAmount, r.Status.ToString());
}
