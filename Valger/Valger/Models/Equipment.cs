using System;
using System.Collections.Generic;

namespace Valger.Models;

public partial class Equipment
{
    public int EquipmentId { get; set; }

    public string Article { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public int AvailableQuantity { get; set; }

    public string? RentalUnit { get; set; }

    public decimal RentalCost { get; set; }

    public decimal? Discount { get; set; }

    public string? Photo { get; set; }

    public int? ManufacturerId { get; set; }

    public int? SupplierId { get; set; }

    public int? TypeId { get; set; }

    public virtual Manufacturer? Manufacturer { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual Supplier? Supplier { get; set; }

    public virtual EquipmentType? Type { get; set; }
}
