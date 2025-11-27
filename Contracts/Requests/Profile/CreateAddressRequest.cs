using System.ComponentModel.DataAnnotations;

namespace OdiNow.Contracts.Requests.Profile;

public class CreateAddressRequest
{
    [MaxLength(40)]
    public string Label { get; set; } = "Home";

    [Required]
    [MaxLength(160)]
    public string Line1 { get; set; } = default!;

    [MaxLength(160)]
    public string? Line2 { get; set; }

    [Required]
    [MaxLength(80)]
    public string City { get; set; } = default!;

    [Required]
    [MaxLength(80)]
    public string District { get; set; } = default!;

    [Required]
    [MaxLength(16)]
    public string PostalCode { get; set; } = default!;

    public double? Latitude { get; set; }

    public double? Longitude { get; set; }

    public bool IsDefault { get; set; }
}


