namespace GodsEye.WEB.Model.Forms
{
    public class CreatePersonForm
    {
        public string Name { get; set; }
        public string Photo { get; set; }
        public int? SectorId { get; set; } = null;
        public int? AcessLevelId { get; set; } = null;
    }
}
