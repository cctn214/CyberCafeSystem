using System;
using System.Collections.Generic;

namespace CCDatabase.Models;

public partial class Unit
{
    public int UnitId { get; set; }

    public string Type { get; set; } = null!;

    public decimal Rate { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<Rent> Rents { get; set; } = new List<Rent>();
}
