using GodsEye.Application.DTOs.Model;

namespace GodsEye.WEB.Model.Forms
{
    public class PersonForm
    {
        public PersonForm()
        {

        }

        public PersonForm(PersonModel person)
        {
            Id = person.Id;
            Name = person.Name;
            SectorId = person.Sector.Id;
            AccessLevelId = person.AccessLevel.Id;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int? SectorId { get; set; } = null;
        public int? AccessLevelId { get; set; } = null;
    }
}
