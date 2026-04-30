using GodsEye.Shared.Interfaces;

namespace GodsEye.Shared.Response
{
    public class LookupResponse : IJsonType, IJSonTypeList
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
