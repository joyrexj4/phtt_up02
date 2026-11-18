using System;
using System.Collections.Generic;

namespace WpfDemoExam02.Models;

public partial class Street
{
    public int Id { get; set; }

    public int ZipcodeId { get; set; }

    public int CityId { get; set; }

    public string StreetName { get; set; } = null!;

    public virtual City City { get; set; } = null!;

    public virtual ICollection<House> Houses { get; set; } = new List<House>();

    public virtual Zipcode Zipcode { get; set; } = null!;
}
