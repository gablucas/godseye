namespace GodsEye.API.DTO
{
    public class PersonModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? ImagePath { get; set; }
        public int Active { get; set; }

        public SectorDTO Sector { get; set; }
        public AccessLevelDTO AccessLevel { get; set; }
    }

    public class SectorDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class AccessLevelDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

}
