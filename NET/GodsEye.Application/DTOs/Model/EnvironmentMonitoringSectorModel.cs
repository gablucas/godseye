using System.Text.Json;

namespace GodsEye.Application.DTOs.Model
{
    public class EnvironmentMonitoringSectorModel
    {

        public int SectorId { get; set; }
        public string SectorName { get; set; }
        public int TotalPerson { get; set; }
        public string? PersonJSON { get; set;  }

        public List<EnvironmentMonitoringSectorLog> PersonLog { get; set; } = new();

        public void ParsePersons()
        {
            PersonLog = string.IsNullOrWhiteSpace(PersonJSON)
                ? new List<EnvironmentMonitoringSectorLog>()
                : JsonSerializer.Deserialize<List<EnvironmentMonitoringSectorLog>>(PersonJSON);
        }
    }

    public class EnvironmentMonitoringSectorLog
    {
        public int PersonId { get; set; }
        public string PersonName { get; set; }
        public string PersonPhoto { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
