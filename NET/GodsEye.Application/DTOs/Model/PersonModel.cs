namespace GodsEye.Application.DTOs.Model
{
    public class PersonModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImagePath { get; set; }
        public int Active { get; set; }

        public SectorDTO Sector { get; set; }
        public AccessLevelModel AccessLevel { get; set; }
    }

    public class SectorDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
