using System;
using System.Collections.Generic;

namespace WpfDemoExam02.Models;

public partial class House
{
    public int Id { get; set; }

    public int StreetId { get; set; }

    public int? HouseNumber { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual Street Street { get; set; } = null!;
}
