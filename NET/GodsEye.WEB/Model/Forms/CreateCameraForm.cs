using GodsEye.WEB.Validation;

namespace GodsEye.WEB.Model.Forms
{
    public class CreateCameraForm
    {
        public string Name { get; set; }
        public string Connection { get; set; }
        public string SectorId { get; set; }
        public IEnumerable<int> Features { get; set; } = new List<int>();
    }
}
