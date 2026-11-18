using System;
using System.Collections.Generic;

namespace WpfDemoExam02.Models;

public partial class ProductList
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public int UnitId { get; set; }

    public decimal Price { get; set; }

    public int ManufacturerId { get; set; }

    public int CategoryId { get; set; }

    public int Discount { get; set; }

    public int AvailableAmount { get; set; }

    public string? Description { get; set; }

    public string? Image { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual Manufacturer Manufacturer { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;

    public virtual Unit Unit { get; set; } = null!;
}
