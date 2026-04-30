namespace GodsEye.Shared.Response.Person
{
    public class PersonResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? ImagePath { get; set; }
        public int Active { get; set; }

        public LookupResponse Sector { get; set; }
        public LookupResponse AccessLevel { get; set; }
    }
}
