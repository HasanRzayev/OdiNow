namespace OdiNow.Contracts.Responses.Profile;

public class UserAddressResponse
{
    public Guid Id { get; set; }
    public string Label { get; set; } = default!;
    public string Line1 { get; set; } = default!;
    public string? Line2 { get; set; }
    public string City { get; set; } = default!;
    public string District { get; set; } = default!;
    public string PostalCode { get; set; } = default!;
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public bool IsDefault { get; set; }
}


