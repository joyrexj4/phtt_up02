using System;
using System.Collections.Generic;

namespace WpfDemoExam02.Models;

public partial class Manufacturer
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<ProductList> ProductLists { get; set; } = new List<ProductList>();
}
