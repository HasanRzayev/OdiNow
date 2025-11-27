namespace OdiNow.Contracts.Responses.Catalog;

public class MenuItemResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public decimal BasePrice { get; set; }
    public bool IsAvailable { get; set; }
    public int PreparationTimeMinutes { get; set; }
    public string? ImageUrl { get; set; }
    public string CategoryName { get; set; } = default!;
}


