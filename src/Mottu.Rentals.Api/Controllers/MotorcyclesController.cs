using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moto.Rentals.Api.Dtos;
using Moto.Rentals.Api.Domain;
using Moto.Rentals.Api.Infra;

namespace Moto.Rentals.Api.Controllers;

[ApiController]
[Route("api/v1/motorcycles")]
public sealed class MotorcyclesController(AppDbContext db, RabbitPublisher bus) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<MotorcycleResponse>> Create([FromBody] CreateMotorcycleRequest req, CancellationToken ct)
    {
        if (await db.Motorcycles.AnyAsync(m => m.Plate == req.Plate, ct))
            return Conflict(new { message = "Plate already exists" });

        var moto = new Motorcycle { Identifier = req.Identifier, Year = req.Year, Model = req.Model, Plate = req.Plate };
        db.Motorcycles.Add(moto);
        await db.SaveChangesAsync(ct);

        bus.Publish("motorcycle.created", new { moto.Id, moto.Identifier, moto.Year, moto.Model, moto.Plate });

        return CreatedAtAction(nameof(GetById), new { id = moto.Id },
            new MotorcycleResponse(moto.Id, moto.Identifier, moto.Year, moto.Model, moto.Plate));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<MotorcycleResponse>> GetById(Guid id, CancellationToken ct)
    {
        var m = await db.Motorcycles.FindAsync([id], ct);
        return m is null ? NotFound() :
            new MotorcycleResponse(m.Id, m.Identifier, m.Year, m.Model, m.Plate);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MotorcycleResponse>>> List([FromQuery] string? plate, CancellationToken ct)
    {
        var q = db.Motorcycles.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(plate)) q = q.Where(x => x.Plate == plate);
        var list = await q.Select(m => new MotorcycleResponse(m.Id, m.Identifier, m.Year, m.Model, m.Plate)).ToListAsync(ct);
        return Ok(list);
    }

    [HttpPatch("{id:guid}/plate")]
    public async Task<IActionResult> UpdatePlate(Guid id, [FromBody] UpdatePlateRequest req, CancellationToken ct)
    {
        if (await db.Motorcycles.AnyAsync(m => m.Plate == req.Plate && m.Id != id, ct))
            return Conflict(new { message = "Plate already exists" });

        var m = await db.Motorcycles.FindAsync([id], ct);
        if (m is null) return NotFound();

        m.Plate = req.Plate;
        await db.SaveChangesAsync(ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var m = await db.Motorcycles.Include(x => x.Rentals).FirstOrDefaultAsync(x => x.Id == id, ct);
        if (m is null) return NotFound();
        if (m.Rentals.Any()) return Conflict(new { message = "Cannot delete motorcycle with rentals" });

        db.Motorcycles.Remove(m);
        await db.SaveChangesAsync(ct);
        return NoContent();
    }
}
