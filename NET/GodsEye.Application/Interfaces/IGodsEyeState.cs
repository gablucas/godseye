using GodsEye.Application.DTOs.Model;

namespace GodsEye.Application.Interfaces
{
    public interface IGodsEyeState
    {
        Task InitializeAsync();
        List<PersonCache> GetPersons();
        List<CameraCache> GetCameras();
        List<AccessLevelCache> GetAccessLevel();

        CameraCache? GetCameraById(int cameraId);
        PersonCache? GetPersonById(int personId);
        AccessLevelCache? GetAccessLevelById(int id);

        void UpserPerson(PersonCache person);
        void UpsertCamera(CameraCache camera);
        void UpserAccessLevel(AccessLevelCache accessLevel);

        void RemovePerson(int id);
        void RemoveCamera(int id);
        void RemoveAccessLevel(int id);

        bool TryUpdateDetection(int personId, int cameraId, DateTime identifiedAt);

    }
}
