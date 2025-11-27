namespace OdiNow.Contracts.Responses.Profile;

public class ProfileResponse
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string? Email { get; set; }
    public string PhoneNumber { get; set; } = default!;
    public string? ProfilePhotoUrl { get; set; }
    public IEnumerable<UserAddressResponse> Addresses { get; set; } = Array.Empty<UserAddressResponse>();
    public int FavoritesCount { get; set; }
}


