# godseye

Dependencias externas

// Redis
winget install Redis.Redis

// Visualizar o banco Redis
redis.io/insight

// Configurações no arquivo windows
// No windows o arquivo fica em Programs Files/Redis/redis.windows-service.conf

// save "tempo" "quantidade de mudanças para salvar"
save 900 1
save 300 10
save 60 1000 

appendonly yes // Toda vez que um job é criado/atualizado/deletado, o Redis anota em um arquivo de log. Se o servidor cair, ele relê esse log e recupera tudo. É a proteção mais importante.

appendfsync everysec // Com que frequência grava o log no disco, everysec => Grava a cada 1 segundo


appendonly yes
appendfsync everysec

// MediaMTX
//RabbitMQ

// PUBLICANDO O PROJETO PARA TESTES

// Entrar na pasta API e pelo CMD executar
dotnet publish -c Release --self-contained true -r win-x64 -o ./publish/api

// Entrar na pasta WEB Blazor e pelo CMD executar
dotnet publish -c Release -o ./publish/blazor

// Executar via CMD para poder rodar o projeto WEB Blazor
dotnet tool install -g dotnet-serve

// Ir na pasta do projeto e executar
dotnet serve -d ./wwwroot