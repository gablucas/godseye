using GodsEye.API.DTO;
using GodsEye.API.Interfaces;
using System.Collections.Concurrent;

namespace GodsEye.API.Services
{
    public class GodsEyeState : IGodsEyeState
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ConcurrentDictionary<int, PersonCache> _persons = new();
        private readonly ConcurrentDictionary<int, CameraCache> _cameras = new();
        private readonly ConcurrentDictionary<int, AccessLevelCache> _accessLevels = new();

        public GodsEyeState(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public async Task InitializeAsync()
        {
            using var scope = _scopeFactory.CreateScope();

            var personQuery = scope.ServiceProvider.GetRequiredService<IPersonQuerie>();
            var cameraQuery = scope.ServiceProvider.GetRequiredService<ICameraQuerie>();
            var accessLevelQuery = scope.ServiceProvider.GetRequiredService<IAccessLevelQuerie>();

            // Agora rodará livremente sem travar threads
            var persons = await personQuery.GetAllCache(CancellationToken.None);
            var cameras = await cameraQuery.GetAllCache(CancellationToken.None);
            var accessLevels = await accessLevelQuery.GetAllCache(CancellationToken.None);

            foreach (var p in persons) _persons.TryAdd(p.Id, p);
            foreach (var c in cameras) _cameras.TryAdd(c.Id, c);
            foreach (var a in accessLevels) _accessLevels.TryAdd(a.Id, a);
        }

        private T? GetItem<T>(ConcurrentDictionary<int, T> dictionary, int id) where T : IGodsEyeCache
        {
            dictionary.TryGetValue(id, out var item);
            return item;
        }

        public PersonCache? GetPersonById(int id) => GetItem(_persons, id);
        public CameraCache? GetCameraById(int id) => GetItem(_cameras, id);
        public AccessLevelCache? GetAccessLevelById(int id) => GetItem(_accessLevels, id);

        private List<T> GetList<T>(ConcurrentDictionary<int, T> dictionary) where T : IGodsEyeCache
        {
            return dictionary.Values.ToList();
        }

        public List<PersonCache> GetPersons() => GetList(_persons);
        public List<CameraCache> GetCameras() => GetList(_cameras);
        public List<AccessLevelCache> GetAccessLevel() => GetList(_accessLevels);


        private void Upsert<T>(ConcurrentDictionary<int, T> dictionary, T value) where T : IGodsEyeCache
        {
            dictionary.AddOrUpdate(value.Id, value, (id, oldValue) => value);
        }

        public void UpserPerson(PersonCache person) => Upsert(_persons, person);
        public void UpsertCamera(CameraCache camera) => Upsert(_cameras, camera);
        public void UpserAccessLevel(AccessLevelCache accessLevel) => Upsert(_accessLevels, accessLevel);


        private void Remove<T>(ConcurrentDictionary<int, T> dictionary, int id) where T : IGodsEyeCache
        {
            dictionary.TryRemove(id, out _);
        }

        public void RemovePerson(int id) => Remove(_persons, id);
        public void RemoveCamera(int id) => Remove(_cameras, id);
        public void RemoveAccessLevel(int id) => Remove(_accessLevels, id);

        public bool TryUpdateDetection(int personId, int cameraId, DateTime identifiedAt)
        {
            if (!_persons.TryGetValue(personId, out var person)) return false;
            if (!_cameras.TryGetValue(cameraId, out var currentCamera)) return false;
            
            lock (person.SyncRoot)
            {
                var currentSector = currentCamera.SectorId;

                // Se nunca foi vista, atualiza e retorna true
                if (person.LastSeen == null)
                {
                    person.LastSeen = identifiedAt;
                    person.LastCameraId = cameraId;
                    return true;
                }

                // Busca a última câmera para comparar setores
                _cameras.TryGetValue(person.LastCameraId ?? 0, out var lastCamera);
                var lastSector = lastCamera?.SectorId;

                // Lógica de descarte por setor (se for o mesmo setor, ignora a detecção)
                if (lastSector == currentSector)
                {
                    // Opcional: mesmo sendo o mesmo setor, se a data for muito superior, 
                    // você pode querer atualizar o LastSeen. Caso contrário, apenas ignora.
                    return false;
                }

                // Se chegou aqui, mudou de setor
                person.LastSeen = identifiedAt;
                person.LastCameraId = cameraId;
                return true;
            }
        }
    }
}
