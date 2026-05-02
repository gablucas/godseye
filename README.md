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