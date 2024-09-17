namespace GajinoAgencies.Dtos;

public record LocationResponseDto(string CityCode, int Id, string? Province=null, string? City = null);