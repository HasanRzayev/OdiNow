using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OdiNow.Models;

public class OrderItem
{
    [Key]
    public Guid Id { get; set; }

    [ForeignKey(nameof(Order))]
    public Guid OrderId { get; set; }

    [ForeignKey(nameof(MenuItem))]
    public Guid MenuItemId { get; set; }

    [MaxLength(160)]
    public string Name { get; set; } = default!;

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal DiscountAmount { get; set; }

    public Order Order { get; set; } = default!;

    public MenuItem MenuItem { get; set; } = default!;
}


