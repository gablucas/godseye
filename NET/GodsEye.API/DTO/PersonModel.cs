namespace GodsEye.API.DTO
{
    public class PersonModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? ImagePath { get; set; }
        public int Active { get; set; }

        public SectorPersonDTO Sector { get; set; }
        public AccessLevelPersonDTO AccessLevel { get; set; }
    }

    public class SectorPersonDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class AccessLevelPersonDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

}
