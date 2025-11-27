using System.ComponentModel.DataAnnotations;
using OdiNow.Models;

namespace OdiNow.Contracts.Requests.Profile;

public class CreateFavoriteRequest
{
    [Required]
    public FavoriteType FavoriteType { get; set; }

    [Required]
    public Guid TargetId { get; set; }
}


