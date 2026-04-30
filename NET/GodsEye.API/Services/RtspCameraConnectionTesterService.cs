using GodsEye.API.Interfaces;
using System.Net.Sockets;
using System.Text;

namespace GodsEye.API.Services
{
    public class RtspCameraConnectionTesterService : ICameraConnectionTesterService
    {
        public async Task<(bool IsSuccess, string Message)> TestConnectionAsync(string rtspUrl)
        {
            try
            {
                // 1. Parsear a URL para pegar IP, Porta e Credenciais
                var uri = new Uri(rtspUrl);
                var host = uri.Host;
                var port = uri.Port > 0 ? uri.Port : 554; // 554 é a porta padrão RTSP

                using var client = new TcpClient();

                // Timeout de 3 segundos para não travar o usuário
                var connectTask = client.ConnectAsync(host, port);
                if (await Task.WhenAny(connectTask, Task.Delay(3000)) != connectTask)
                {
                    return (false, "Tempo limite esgotado. A câmera não respondeu.");
                }

                // 2. Enviar comando OPTIONS (O "Ping" do RTSP)
                // Esse comando pergunta quais métodos o servidor aceita sem precisar baixar vídeo
                var stream = client.GetStream();
                var command = $"OPTIONS {rtspUrl} RTSP/1.0\r\nCSeq: 1\r\nUser-Agent: CheckCamera\r\n\r\n";
                var bytes = Encoding.UTF8.GetBytes(command);

                await stream.WriteAsync(bytes, 0, bytes.Length);

                // 3. Ler a resposta
                var buffer = new byte[1024];

                // Dá um tempinho para ler a resposta
                var readTask = stream.ReadAsync(buffer, 0, buffer.Length);
                if (await Task.WhenAny(readTask, Task.Delay(3000)) != readTask)
                {
                    return (false, "Conectou, mas a câmera não respondeu ao comando RTSP.");
                }

                int bytesRead = await readTask;
                var response = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                // 4. Analisar Resposta
                // O padrão é responder "RTSP/1.0 200 OK"
                if (response.Contains("200 OK"))
                {
                    return (true, "Conexão estabelecida com sucesso!");
                }
                else if (response.Contains("401 Unauthorized"))
                {
                    // Se der 401, significa que o IP/Porta estão certos e é uma câmera,
                    // mas a senha/usuário na URL podem estar errados.
                    return (false, "A câmera foi encontrada, mas a senha ou usuário estão incorretos.");
                }

                return (false, $"Resposta inesperada da câmera: {response.Split('\r')[0]}");
            }
            catch (SocketException)
            {
                return (false, "Não foi possível conectar ao IP/Porta informados.");
            }
            catch (Exception ex)
            {
                return (false, $"Erro ao testar conexão, contate um administrador.");
            }
        }
    }
}
