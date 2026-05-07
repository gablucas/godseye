using GodsEye.API.DTO;

namespace GodsEye.API.Interfaces
{
    public interface IGodsEyeState
    {
        Task InitializeAsync();
        List<PersonCache> GetPersons();
        List<DeviceCache> GetCameras();
        List<AccessLevelCache> GetAccessLevel();

        DeviceCache? GetCameraById(int cameraId);
        PersonCache? GetPersonById(int personId);
        AccessLevelCache? GetAccessLevelById(int id);

        void UpserPerson(PersonCache person);
        void UpsertCamera(DeviceCache camera);
        void UpserAccessLevel(AccessLevelCache accessLevel);

        void RemovePerson(int id);
        void RemoveCamera(int id);
        void RemoveAccessLevel(int id);

        bool TryUpdateDetection(int personId, int cameraId, DateTime identifiedAt);

    }
}
