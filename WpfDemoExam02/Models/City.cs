using System;
using System.Collections.Generic;

namespace WpfDemoExam02.Models;

public partial class City
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Street> Streets { get; set; } = new List<Street>();
}
