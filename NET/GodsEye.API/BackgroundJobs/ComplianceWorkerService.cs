namespace GodsEye.API.BackgroundJobs
{
    public class ComplianceWorkerService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public ComplianceWorkerService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(TimeSpan.FromMinutes(1));

            while(await timer.WaitForNextTickAsync(stoppingToken))
            {
                using var scope = _scopeFactory.CreateScope();

                //var queryPerson = scope.ServiceProvider
                //.GetRequiredService<IQueryPerson>();
            }
        }
    }
}
