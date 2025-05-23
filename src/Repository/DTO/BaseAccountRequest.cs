using System.Text.Json.Serialization;

namespace Repository.DTO;

public abstract class BaseAccountRequest
{
    public char Role { get; set; }
    public string UniqueCode { get; set; }
    public string Discord { get; set; }
    
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public long DiscordUniqueId { get; set; }
    public bool HasForeignMinistry { get; set; }
    public bool HasFederalAidCommission { get; set; }
    public bool HasDisasterReliefAgency { get; set; }
}