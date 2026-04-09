using GodsEye.Application.DTOs.Model;

namespace GodsEye.Application.Interfaces
{
    public interface IFaceMatcherService
    {
        (int, float) FindMatch(float[] extractedVector, List<PersonCache> persons, float threshold = 0.65f);
    }
}
