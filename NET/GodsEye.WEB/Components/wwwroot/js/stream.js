window.streamFunctions = {
    // Armazena as conexões ativas para poder fechar depois
    activeConnections: {},

    start: async function (videoId, streamUrl) {
        const videoElement = document.getElementById(videoId);
        if (!videoElement) {
            console.error("Elemento de vídeo não encontrado:", videoId);
            return;
        }

        // 1. Limpeza: Se já existir uma stream rodando nesse elemento, pare ela antes
        if (this.activeConnections[videoId]) {
            this.activeConnections[videoId].close();
            delete this.activeConnections[videoId];
        }

        // 2. Configuração do WebRTC (PeerConnection)
        const pc = new RTCPeerConnection({
            iceServers: [{ urls: 'stun:stun.l.google.com:19302' }]
        });

        // Guarda a referência para fechar depois
        this.activeConnections[videoId] = pc;

        // Quando o MediaMTX mandar a stream (track), joga no vídeo
        pc.ontrack = (event) => {
            videoElement.srcObject = event.streams[0];
        };

        // 3. Cria a "Oferta" (Dizendo: "Eu quero receber vídeo/audio apenas")
        // O 'recvonly' é importante para players
        pc.addTransceiver('video', { direction: 'recvonly' });
        pc.addTransceiver('audio', { direction: 'recvonly' });

        const offer = await pc.createOffer();
        await pc.setLocalDescription(offer);

        // 4. Manda a oferta para o MediaMTX (Handshake WHEP)
        // O MediaMTX espera um POST com o SDP da oferta
        try {
            const response = await fetch(streamUrl, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/sdp'
                },
                body: offer.sdp
            });

            if (!response.ok) {
                throw new Error(`Erro ao conectar no MediaMTX: ${response.statusText}`);
            }

            // 5. Recebe a resposta (Answer) e finaliza a conexão
            const answerSdp = await response.text();
            await pc.setRemoteDescription(new RTCSessionDescription({
                type: 'answer',
                sdp: answerSdp
            }));

        } catch (err) {
            console.error("Falha na negociação WebRTC:", err);
            pc.close();
        }
    },

    stop: function (videoId) {
        const videoElement = document.getElementById(videoId);

        if (this.activeConnections[videoId]) {
            this.activeConnections[videoId].close();
            delete this.activeConnections[videoId];
        }

        if (videoElement) {
            videoElement.srcObject = null;
        }
    }
};