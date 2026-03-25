using System;
using System.Collections.Generic;

namespace Valger.Models;

public partial class PickupPoint
{
    public int PointId { get; set; }

    public string Address { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
