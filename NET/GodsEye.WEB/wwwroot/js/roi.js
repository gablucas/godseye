let canvas;
let ctx;
let videoElement;

let startX = 0;
let startY = 0;
let isDrawing = false;
let isSynced = false;
let drawingEnabled = false;

// Armazena o retângulo final
let rect = null;

export function initRoiCanvas() {
    canvas = document.getElementById("roiCanvas");
    if (!canvas) return; // Segurança extra

    ctx = canvas.getContext("2d");

    canvas.addEventListener("mousedown", onMouseDown);
    canvas.addEventListener("mousemove", onMouseMove);
    canvas.addEventListener("mouseup", onMouseUp);

    // Tratamento para quando o mouse sai do canvas enquanto arrasta
    canvas.addEventListener("mouseleave", onMouseUp);

    canvas.addEventListener("contextmenu", e => e.preventDefault());

    disableDrawing();
}

export function syncCanvasWithVideo(videoId) {
    videoElement = document.getElementById(videoId);
    if (!videoElement || !canvas) return;

    const sync = () => {
        if (videoElement.videoWidth > 0 && videoElement.videoHeight > 0) {
            canvas.width = videoElement.videoWidth;
            canvas.height = videoElement.videoHeight;
        }

        ctx.clearRect(0, 0, canvas.width, canvas.height);
        isSynced = true;

        // Redesenha o retângulo existente na nova escala
        if (rect) setStrokeRect(rect);
    };

    if (videoElement.readyState >= 1) {
        sync();
    } else {
        videoElement.addEventListener("loadedmetadata", sync);
    }

    const resizeObserver = new ResizeObserver(() => {
        // Redesenha para garantir que o canvas não estique visualmente errado
        if (rect) setStrokeRect(rect);
    });

    resizeObserver.observe(videoElement);
}

function onMouseDown(e) {
    if (!drawingEnabled || !isSynced) return;
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

    // Garante limites do canvas
    const currentX = Math.max(0, Math.min(canvas.width, pos.x));
    const currentY = Math.max(0, Math.min(canvas.height, pos.y));

    // Cálculo que permite arrastar para trás (valores negativos temporários)
    const width = currentX - startX;
    const height = currentY - startY;

    ctx.clearRect(0, 0, canvas.width, canvas.height);

    ctx.strokeStyle = "#00ff00";
    ctx.lineWidth = 3;
    ctx.strokeRect(startX, startY, width, height);
}

function onMouseUp(e) {
    if (!drawingEnabled || !isDrawing) return;
    isDrawing = false;

    const pos = getMousePos(e);

    // Normalização (Matemática para garantir X/Y no topo-esquerdo e Width/Height positivos)
    // Isso corrige o problema de arrastar da direita para esquerda
    const currentX = Math.max(0, Math.min(canvas.width, pos.x));
    const currentY = Math.max(0, Math.min(canvas.height, pos.y));

    const finalX = Math.min(startX, currentX);
    const finalY = Math.min(startY, currentY);
    const finalW = Math.abs(currentX - startX);
    const finalH = Math.abs(currentY - startY);

    if (finalW < 5 || finalH < 5) {
        rect = null;
        ctx.clearRect(0, 0, canvas.width, canvas.height);
        return;
    }

    // Cria o objeto unificado
    updateGlobalRect(finalX, finalY, finalW, finalH);

    // Redesenha limpo usando a função centralizada
    setStrokeRect(rect);
}

function getMousePos(e) {
    const bounds = canvas.getBoundingClientRect();

    // Proteção contra divisão por zero se o elemento estiver oculto
    if (bounds.width === 0 || bounds.height === 0) return { x: 0, y: 0 };

    const scaleX = canvas.width / bounds.width;
    const scaleY = canvas.height / bounds.height;

    return {
        x: (e.clientX - bounds.left) * scaleX,
        y: (e.clientY - bounds.top) * scaleY
    };
}

// Helper para manter o objeto rect sempre consistente
function updateGlobalRect(x, y, w, h) {
    rect = {
        x: Math.round(x),
        y: Math.round(y),
        width: Math.round(w),
        height: Math.round(h),
        // Recalcula sempre os relativos para garantir consistência
        relativeX: x / canvas.width,
        relativeY: y / canvas.height,
        relativeWidth: w / canvas.width,
        relativeHeight: h / canvas.height
    };
}

export function setStrokeRect(inputRect) {
    if (!inputRect || !ctx || !canvas) return;

    ctx.clearRect(0, 0, canvas.width, canvas.height);
    ctx.strokeStyle = "#00ff00";
    ctx.lineWidth = 3;

    let rX = inputRect.x;
    let rY = inputRect.y;
    let rW = inputRect.width;
    let rH = inputRect.height;

    // Detecta se veio em porcentagem (ex: do banco de dados)
    // Checagem mais robusta: se for menor que 1 E não for 0 pixels intencionalmente
    const isPercentage = (inputRect.width <= 1 && inputRect.width > 0) && (inputRect.x <= 1);

    if (isPercentage) {
        rX *= canvas.width;
        rY *= canvas.height;
        rW *= canvas.width;
        rH *= canvas.height;
    }

    ctx.strokeRect(rX, rY, rW, rH);

    // ATENÇÃO: Atualiza o global rect com os valores calculados E os relativos
    updateGlobalRect(rX, rY, rW, rH);
}

export function clearStrokeRect() {
    if (!ctx || !canvas) return;
    ctx.clearRect(0, 0, canvas.width, canvas.height);
    rect = null; // Importante limpar a referência lógica também
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
    return rect;
}