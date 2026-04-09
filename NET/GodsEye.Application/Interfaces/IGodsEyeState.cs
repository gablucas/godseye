using GodsEye.Application.DTOs.Model;

namespace GodsEye.Application.Interfaces
{
    public interface IGodsEyeState
    {
        List<PersonCache> GetPersons();
        List<CameraCache> GetCameras();
        CameraCache? GetCameraById(int cameraId);
        PersonCache GetPersonById(int personId);
        void UpdateLastSeen(int personId, int cameraId, DateTime identifiedAt);
        void UpsertPerson(PersonCache person);
        void RemovePerson(int personId);
        void UpsertCamera(CameraCache camera);
        bool TryUpdateDetection(int personId, int cameraId, DateTime identifiedAt);

    }
}
