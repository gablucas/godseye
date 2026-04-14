using GodsEye.Application.DTOs.Model;
using GodsEye.Application.Interfaces;
using GodsEye.Application.Interfaces.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace GodsEye.Infrastructure.Services
{
    public class GodsEyeState : IGodsEyeState
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private GodsEyeCache _cache = new();
        private readonly object _lock = new();

        public GodsEyeState(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
            Initialize().GetAwaiter().GetResult();
        }

        public List<PersonCache> GetPersons()
        {
            lock (_lock)
            {
                // Retornamos uma cópia (.ToList) para que quem lê não trave quem escreve
                return _cache.Persons.ToList();
            }
        }

        public List<CameraCache> GetCameras()
        {
            lock (_lock)
            {
                return _cache.Cameras.ToList();
            }
        }

        public CameraCache? GetCameraById(int cameraId)
        {
            lock (_lock)
            {
                return _cache.Cameras.FirstOrDefault(x => x.Id == cameraId);
            }
        }

        public PersonCache GetPersonById(int personId)
        {
            lock (_lock)
            {
                return _cache.Persons.First(x => x.Id == personId);
            }
        }

        private async Task Initialize()
        {
            using var scope = _scopeFactory.CreateScope();

            var personQuery = scope.ServiceProvider.GetRequiredService<IPersonQuerie>();
            var cameraQuery = scope.ServiceProvider.GetRequiredService<ICameraQuerie>();

            var persons = await personQuery.GetAllCache(CancellationToken.None);
            var cameras = await cameraQuery.GetAllCache(CancellationToken.None);

            lock(_lock)
            {
                _cache.Persons = persons.ToList();
                _cache.Cameras = cameras.ToList();
            }
        }

        public void UpdateLastSeen(int personId, int cameraId, DateTime identifiedAt)
        {
            lock (_lock)
            {
                var person = _cache.Persons.FirstOrDefault(p => p.Id == personId);
                if (person != null)
                {
                    person.LastSeen = identifiedAt;
                    person.LastCameraId = cameraId;
                }
            }
        }

        public void UpsertPerson(PersonCache person)
        {
            lock ( _lock)
            {
                var index = _cache.Persons.FindIndex(p => p.Id == person.Id);
                if (index != -1) _cache.Persons[index] = person;
                else _cache.Persons.Add(person);
            }
        }

        public void RemovePerson(int personId)
        {
            lock (_lock) 
            {
                _cache.Persons.RemoveAll(p => p.Id == personId);
            
            }
        }

        public void UpsertCamera(CameraCache camera)
        {
            lock (_lock)
            {
                var index = _cache.Cameras.FindIndex(c => c.Id == camera.Id);
                if (index != -1) _cache.Cameras[index] = camera;
                else _cache.Cameras.Add(camera);
            }
        }

        public bool TryUpdateDetection(int personId, int cameraId, DateTime identifiedAt)
        {
            lock (_lock)
            {
                var person = _cache.Persons.FirstOrDefault(p => p.Id == personId);
                var currentCamera = _cache.Cameras.FirstOrDefault(c => c.Id == cameraId);

                if (person == null || currentCamera == null) return false;

                var currentSector = currentCamera.SectorId;

                if (person.LastSeen == null)
                {
                    person.LastSeen = identifiedAt;
                    person.LastCameraId = cameraId;
                    return true;
                }

                var lastCamera = _cache.Cameras.FirstOrDefault(c => c.Id == person.LastCameraId);
                var lastSector = lastCamera?.SectorId;

                if (identifiedAt < person.LastSeen)
                {
                    if (lastSector == currentSector)
                        return false;

                    return true;
                }

                if (lastSector == currentSector)
                {
                    return false;
                }

                person.LastSeen = identifiedAt;
                person.LastCameraId = cameraId;

                return true;
            }
        }
    }
}
