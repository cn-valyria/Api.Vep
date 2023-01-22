using System.Collections.Generic;

namespace Repository.DTO;

public class AidListDetail
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Subject { get; set; }
    public string Message { get; set; }
    public List<ListRecipientDetail> Recipients { get; set; }
}