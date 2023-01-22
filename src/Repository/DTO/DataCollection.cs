using System.Collections.Generic;

namespace Repository.DTO;

public class DataCollection<T>
{
    public int TotalCount { get; set; }
    public IEnumerable<T> Results { get; set; }
}