namespace OdiNow.Contracts.Responses.Catalog;

public class CategoryResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Slug { get; set; } = default!;
    public string? Description { get; set; }
    public int DisplayOrder { get; set; }
}


