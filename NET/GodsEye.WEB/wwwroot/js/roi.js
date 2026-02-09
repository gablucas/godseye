let canvas;
let ctx;
let videoElement; // Guardar referência do vídeo

let startX = 0;
let startY = 0;
let isDrawing = false;
let isSynced = false;
let drawingEnabled = false;

// Armazena o retângulo final
let rect = null;

export function initRoiCanvas() {
    canvas = document.getElementById("roiCanvas");
    ctx = canvas.getContext("2d");

    // Adiciona suporte a Touch para mobile/tablet também
    canvas.addEventListener("mousedown", onMouseDown);
    canvas.addEventListener("mousemove", onMouseMove);
    canvas.addEventListener("mouseup", onMouseUp);

    // Opcional: prevenir menu de contexto ao clicar com botão direito
    canvas.addEventListener("contextmenu", e => e.preventDefault());

    disableDrawing();
}

export function syncCanvasWithVideo(videoId) {
    videoElement = document.getElementById(videoId);
    if (!videoElement || !canvas) return;

    const sync = () => {
        // 1. A Resolução Interna (buffer) deve ser a original do vídeo (ex: 800x600)
        if (videoElement.videoWidth > 0 && videoElement.videoHeight > 0) {
            canvas.width = videoElement.videoWidth;
            canvas.height = videoElement.videoHeight;
        }

        // Limpa e reseta
        ctx.clearRect(0, 0, canvas.width, canvas.height);
        isSynced = true;

        // Se já tiver um rect salvo, redesenha ele agora que o tamanho mudou
        if (rect) setStrokeRect(rect);
    };

    // Sincroniza metadados
    if (videoElement.readyState >= 1) {
        sync();
    } else {
        videoElement.addEventListener("loadedmetadata", sync);
    }

    // 2. ResizeObserver: Garante que o desenho não suma se a tela mudar
    const resizeObserver = new ResizeObserver(() => {
        // O CSS cuida do tamanho visual, mas precisamos garantir que o rect 
        // seja redesenhado na proporção correta se houver mudança drástica
        if (rect) setStrokeRect(rect);
    });

    resizeObserver.observe(videoElement);
}

function onMouseDown(e) {
    if (!drawingEnabled || !isSynced) return;

    // Evita arrastar a imagem se for tag img, etc.
    e.preventDefault();

    const pos = getMousePos(e);

    isDrawing = true;
    startX = pos.x;
    startY = pos.y;
}

function onMouseMove(e) {
    if (!isDrawing || !drawingEnabled) return;
    e.preventDefault();

    const pos = getMousePos(e);

    // Garante que não saia das bordas do vídeo (ex: -5px vira 0)
    const currentX = Math.max(0, Math.min(canvas.width, pos.x));
    const currentY = Math.max(0, Math.min(canvas.height, pos.y));

    const width = currentX - startX;
    const height = currentY - startY;

    // Limpa tudo
    ctx.clearRect(0, 0, canvas.width, canvas.height);

    // Desenha o novo quadrado
    ctx.strokeStyle = "#00ff00"; // Verde "Matrix"
    ctx.lineWidth = 3; // Linha um pouco mais grossa para visibilidade
    ctx.strokeRect(startX, startY, width, height);
}

function onMouseUp(e) {
    if (!drawingEnabled || !isDrawing) return;
    isDrawing = false;

    const pos = getMousePos(e);

    // Normaliza para garantir que width/height sejam positivos 
    // (caso o usuário arraste da direita para esquerda)
    const finalX = Math.min(startX, pos.x);
    const finalY = Math.min(startY, pos.y);
    const finalW = Math.abs(pos.x - startX);
    const finalH = Math.abs(pos.y - startY);

    // Evita cliques acidentais (tamanho zero)
    if (finalW < 5 || finalH < 5) {
        rect = null;
        ctx.clearRect(0, 0, canvas.width, canvas.height);
        return;
    }

    // ⬇️ SALVA O RECT FINAL
    // Aqui está o pulo do gato: salvamos JÁ em pixels reais para o FFmpeg
    // e também em relativo caso precise no front.
    rect = {
        x: Math.round(Math.max(0, finalX)),
        y: Math.round(Math.max(0, finalY)),
        width: Math.round(finalW),
        height: Math.round(finalH),

        // Se precisar da porcentagem (0.0 a 1.0)
        relativeX: finalX / canvas.width,
        relativeY: finalY / canvas.height,
        relativeWidth: finalW / canvas.width,
        relativeHeight: finalH / canvas.height
    };

    // Redesenha limpo e fixo
    setStrokeRect(rect);
}

function getMousePos(e) {
    // getBoundingClientRect pega o tamanho VISUAL do canvas na tela (ex: 400x300)
    const bounds = canvas.getBoundingClientRect();

    // canvas.width é o tamanho REAL do vídeo (ex: 800)
    // Se visual é 400 e real é 800, scaleX = 2.
    const scaleX = canvas.width / bounds.width;
    const scaleY = canvas.height / bounds.height;

    return {
        // Multiplicamos a posição do clique pela escala para achar o pixel real
        x: (e.clientX - bounds.left) * scaleX,
        y: (e.clientY - bounds.top) * scaleY
    };
}

// Função para desenhar programaticamente (ex: carregar do banco)
export function setStrokeRect(inputRect) {
    if (!inputRect || !ctx || !canvas) return;

    ctx.clearRect(0, 0, canvas.width, canvas.height);
    ctx.strokeStyle = "#00ff00";
    ctx.lineWidth = 3;

    // Verifica se o rect veio em porcentagem (menor que 1) ou pixels
    // Assume que se width for <= 1, é porcentagem.
    let rX = inputRect.x;
    let rY = inputRect.y;
    let rW = inputRect.width;
    let rH = inputRect.height;

    if (inputRect.width <= 1 && inputRect.x <= 1) {
        rX *= canvas.width;
        rY *= canvas.height;
        rW *= canvas.width;
        rH *= canvas.height;
    }

    ctx.strokeRect(rX, rY, rW, rH);

    // Atualiza a variavel global caso tenhamos setado externamente
    rect = { x: rX, y: rY, width: rW, height: rH };
}

export function enableDrawing() {
    if (!isSynced) return;
    drawingEnabled = true;
    canvas.style.pointerEvents = "auto";
    canvas.style.cursor = "crosshair";
}

export function disableDrawing() {
    drawingEnabled = false;
    isDrawing = false;
    canvas.style.pointerEvents = "none";
    canvas.style.cursor = "default";
}

export function getRect() {
    return rect; // Retorna o objeto com Pixels Reais E Relativos
}