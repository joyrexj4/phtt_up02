using System;
using System.Collections.Generic;

namespace WpfDemoExam02.Models;

public partial class Zipcode
{
    public int Id { get; set; }

    public int Name { get; set; }

    public virtual ICollection<Street> Streets { get; set; } = new List<Street>();
}
