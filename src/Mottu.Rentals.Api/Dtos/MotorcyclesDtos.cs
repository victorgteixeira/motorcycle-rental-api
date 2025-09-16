namespace Moto.Rentals.Api.Dtos;

public sealed record CreateMotorcycleRequest(string Identifier, int Year, string Model, string Plate);
public sealed record MotorcycleResponse(Guid Id, string Identifier, int Year, string Model, string Plate);
public sealed record UpdatePlateRequest(string Plate);
