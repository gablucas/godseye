namespace GodsEye.WEB.Model.Forms
{
    public class CreateCameraForm
    {
        public string Name { get; set; }
        public string Connection { get; set; }
        public int? SectorId { get; set; } = null;
        public IEnumerable<int> Features { get; set; } = new List<int>();
    }
}
