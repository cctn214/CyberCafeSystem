using System;
using System.Collections.Generic;

namespace CCDatabase.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Name { get; set; } = null!;

    public decimal Balance { get; set; }

    public virtual ICollection<Rent> Rents { get; set; } = new List<Rent>();
}
