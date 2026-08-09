using System;
using System.Collections.Generic;

namespace CCDatabase.Models;

public partial class Rent
{
    public int RentId { get; set; }

    public int UserId { get; set; }

    public int UnitId { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public int? Duration { get; set; }

    public decimal? TotalCost { get; set; }

    public virtual Unit Unit { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
