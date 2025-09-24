using System;
using System.Collections.Generic;

namespace Repositories.Model;

public partial class DealerType
{
    public int DealerTypeId { get; set; }

    public string TypeName { get; set; } = null!;

    public virtual ICollection<Dealer> Dealers { get; set; } = new List<Dealer>();
}
