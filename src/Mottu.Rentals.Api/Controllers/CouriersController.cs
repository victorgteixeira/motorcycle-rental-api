using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mottu.Rentals.Api.Domain;
using Mottu.Rentals.Api.Dtos;
using Mottu.Rentals.Api.Infra;
using Mottu.Rentals.Api.Services;

namespace Mottu.Rentals.Api.Controllers;

[ApiController]
[Route("api/v1/couriers")]
public sealed class CouriersController(AppDbContext db, FileStorageService storage) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<CourierResponse>> Create([FromBody] CreateCourierRequest req, CancellationToken ct)
    {
        if (req.CnhType is not (CnhType.A or CnhType.APlusB))
            return BadRequest(new { message = "Courier must have CNH A or A+B" });

        if (await db.Couriers.AnyAsync(c => c.Cnpj == req.Cnpj, ct))
            return Conflict(new { message = "CNPJ already exists" });
        if (await db.Couriers.AnyAsync(c => c.CnhNumber == req.CnhNumber, ct))
            return Conflict(new { message = "CNH number already exists" });

        var c = new Courier {
            Identifier = req.Identifier, Name = req.Name, Cnpj = req.Cnpj, BirthDate = req.BirthDate,
            CnhNumber = req.CnhNumber, CnhType = req.CnhType
        };
        db.Couriers.Add(c);
        await db.SaveChangesAsync(ct);

        return CreatedAtAction(nameof(GetById), new { id = c.Id },
            new CourierResponse(c.Id, c.Identifier, c.Name, c.Cnpj, c.BirthDate, c.CnhNumber, c.CnhType, c.CnhImageUrl));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CourierResponse>> GetById(Guid id, CancellationToken ct)
    {
        var c = await db.Couriers.FindAsync([id], ct);
        return c is null ? NotFound() :
            new CourierResponse(c.Id, c.Identifier, c.Name, c.Cnpj, c.BirthDate, c.CnhNumber, c.CnhType, c.CnhImageUrl);
    }

    [HttpPut("{id:guid}/cnh-image")]
    public async Task<IActionResult> UploadCnh(Guid id, IFormFile file, CancellationToken ct)
    {
        var c = await db.Couriers.FindAsync([id], ct);
        if (c is null) return NotFound();
        var path = await storage.SaveCnhAsync(id, file, ct);
        c.CnhImageUrl = path;
        await db.SaveChangesAsync(ct);
        return NoContent();
    }
}
