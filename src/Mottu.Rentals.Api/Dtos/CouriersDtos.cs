using Moto.Rentals.Api.Domain;

namespace Moto.Rentals.Api.Dtos;

public sealed record CreateCourierRequest(
    string Identifier, string Name, string Cnpj, DateOnly BirthDate, string CnhNumber, CnhType CnhType);

public sealed record CourierResponse(
    Guid Id, string Identifier, string Name, string Cnpj, DateOnly BirthDate, string CnhNumber, CnhType CnhType, string? CnhImageUrl);
