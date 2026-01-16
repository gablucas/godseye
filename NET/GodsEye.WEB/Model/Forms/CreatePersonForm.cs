namespace GodsEye.WEB.Model.Forms
{
    public class CreatePersonForm
    {
        public string Name { get; set; }
        public string Photo { get; set; }
        public IEnumerable<string> Sectors { get; set; }
    }
}
