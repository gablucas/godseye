
 window._hlsInstances = window._hlsInstances || { };

    window.loadHlsVideo = async function (elementId, url, opts) {
        // opções padrão
        opts = Object.assign({
            checkIntervalMs: 1000,    // espera entre tentativas ao checar o manifest
            maxCheckAttempts: 20,     // tentativas antes de falhar
            hlsOptions: {             // opções passadas ao Hls()
                manifestLoadingMaxRetry: 6,
                manifestLoadingRetryDelay: 1000,
                manifestLoadingRetryOnFail: true
            }
        }, opts || {});

    // encode da URL (garante espaços e caracteres OK)
    try {url = decodeURIComponent(url); } catch(e) { }
    url = encodeURI(url);

    // espera o elemento existir no DOM
    let video = document.getElementById(elementId);
    const waitElement = async () => {
        for (let i = 0; i < 10 && !video; i++) {
        await new Promise(r => setTimeout(r, 100));
    video = document.getElementById(elementId);
        }
    return video;
    };
    video = await waitElement();
    if (!video) {
        console.error("loadHlsVideo: elemento não encontrado:", elementId);
    return;
    }

    // cancela instância anterior para esse elemento (se houver)
    if (window._hlsInstances[elementId]) {
        try {
        window._hlsInstances[elementId].destroy();
        } catch { }
    window._hlsInstances[elementId] = null;
    }

    // Função que checa se o manifest já existe (faz fetch)
    const manifestExists = async () => {
        try {
            const resp = await fetch(url, {method: 'GET', mode: 'cors', cache: 'no-store' });
    if (!resp.ok) return false;
    // opcional: checar content-type ou conteúdo curto
    const ct = resp.headers.get('content-type') || '';
    if (ct.includes('application/vnd.apple.mpegurl') || ct.includes('vnd.apple.mpegurl') || ct.includes('application/x-mpegURL') || ct.includes('audio/mpegurl') || ct.includes('text/plain')) {
                return true;
            }
    // fallback: se for texto e contiver #EXTM3U
    const txt = await resp.text();
    return txt.indexOf('#EXTM3U') !== -1;
        } catch (err) {
            return false;
        }
    };

    // espera manifest aparecer com polling
    let attempt = 0;
    let ok = false;
    while (attempt < opts.maxCheckAttempts && !ok) {
        ok = await manifestExists();
    if (ok) break;
    attempt++;
        await new Promise(r => setTimeout(r, opts.checkIntervalMs));
    }

    if (!ok) {
        console.warn(`loadHlsVideo: manifest não encontrado após ${opts.maxCheckAttempts} tentativas: ${url}`);
        // ainda vamos tentar iniciar o Hls.js (ele fará retrys internos), mas avisamos no console
    }

    // Safari nativo?
    if (video.canPlayType('application/vnd.apple.mpegurl')) {
        video.src = url;
    try {await video.play(); } catch { }
    return;
    }

    // Instancia Hls
    if (typeof Hls === 'undefined') {
        console.error('Hls.js não encontrado. Adicione <script src="https://cdn.jsdelivr.net/npm/hls.js@latest"></script> em index.html');
    return;
    }

    const hls = new Hls(opts.hlsOptions);

    // guarda instância para cleanup futuro
    window._hlsInstances[elementId] = hls;

    hls.on(Hls.Events.ERROR, function (event, data) {
        console.warn('hls error', event, data);
    // em alguns erros, recarregar o manifest ajuda
    if (data && data.type === Hls.ErrorTypes.NETWORK_ERROR) {
        // tenta recarregar o manifest
        setTimeout(() => {
            try { hls.startLoad(); } catch (e) { }
        }, 1000);
        }
    });

    try {
        hls.loadSource(url);
    hls.attachMedia(video);
    } catch (err) {
        console.error('Erro ao iniciar Hls.js:', err);
    }

    // ao destruir (se necessário) você pode chamar: window._hlsInstances['camera-player'].destroy();
};

