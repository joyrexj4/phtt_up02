using System;
using System.Collections.Generic;

namespace WpfDemoExam02.Models;

public partial class Product
{
    public int Id { get; set; }

    public string? Article { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<ProductList> ProductLists { get; set; } = new List<ProductList>();

    public virtual ICollection<ProductSupplier> ProductSuppliers { get; set; } = new List<ProductSupplier>();
}
