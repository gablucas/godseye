using GodsEye.API.DTO;

namespace GodsEye.API.Interfaces
{
    public interface IFaceMatcherService
    {
        (int, float) FindMatch(float[] extractedVector, List<PersonCache> persons, float threshold = 0.65f);
    }
}
