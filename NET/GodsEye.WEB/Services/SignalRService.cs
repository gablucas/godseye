using Microsoft.AspNetCore.SignalR.Client;

namespace GodsEye.WEB.Services
{
    public class SignalRService
    {
        public HubConnection? Connection { get; private set; }

        public void Create(string hubUrl)
        {
            if (Connection != null)
                return;

            Connection = new HubConnectionBuilder()
                .WithUrl(hubUrl)
                .WithAutomaticReconnect()
                .Build();
        }

        public void On<T>(string methodName, Action<T> handler)
        {
            if (Connection == null)
                throw new InvalidOperationException("Connection não criada");

            Connection.On(methodName, handler);
        }

        public async Task StartAsync()
        {
            if (Connection == null)
                throw new InvalidOperationException("Connection não criada");

            if (Connection.State == HubConnectionState.Disconnected)
            {
                await Connection.StartAsync();
                Console.WriteLine("✅ SignalR conectado");
            }
        }
    }
}
