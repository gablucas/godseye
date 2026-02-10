let canvas, ctx, videoElement;
let isActive = false;
let currentMode = null; // 'rect' ou 'polygon'

// Estado interno
let isDragging = false;
let points = [];
let rectStart = null;
let rectCurrent = null;

// Cores
const drawColor = "#00FFCE";
const fillColor = "rgba(0, 255, 206, 0.2)";
const nodeColor = "#FFFFFF";

export function initRoiCanvas(videoId, canvasId) {
    videoElement = document.getElementById(videoId);
    canvas = document.getElementById(canvasId);

    if (!videoElement || !canvas) return;

    ctx = canvas.getContext('2d');

    const resizeObserver = new ResizeObserver(() => syncSize());
    resizeObserver.observe(videoElement);

    // Eventos globais (adicionados apenas uma vez)
    canvas.addEventListener('mousedown', onMouseDown);
    canvas.addEventListener('mousemove', onMouseMove);
    window.addEventListener('mouseup', onMouseUp);

    window.addEventListener('keydown', (e) => {
        if (!isActive) return;
        if (e.key === 'z' && (e.ctrlKey || e.metaKey)) undo();
        if (e.key === 'Enter' && currentMode === 'polygon') closePolygon();
        if (e.key === 'Escape') cancelDrawing();
    });

    syncSize();
}

function syncSize() {
    if (videoElement && canvas) {
        const width = videoElement.clientWidth;
        const height = videoElement.clientHeight;
        if (canvas.width !== width || canvas.height !== height) {
            canvas.width = width;
            canvas.height = height;
            redraw();
        }
    }
}

// --- ZERAR TUDO (Função auxiliar interna) ---
function resetState() {
    points = [];
    rectStart = null;
    rectCurrent = null;
    isDragging = false;
    // Não alteramos isActive ou currentMode aqui, pois isso depende do contexto
}

// --- API Pública ---

export function startDrawing(mode) {
    resetState(); // <--- O SEGREDO: Limpa lixo da memória anterior

    isActive = true;
    currentMode = mode;
    canvas.style.cursor = 'crosshair';
    redraw();
}

export function stopDrawing() {
    isActive = false;
    isDragging = false;
    currentMode = null;
    canvas.style.cursor = 'default';
    redraw();
}

export function clearCanvas() {
    resetState();
    redraw();
}

export function undo() {
    if (currentMode === 'polygon' && points.length > 0) {
        points.pop();
        redraw();
    } else if (currentMode === 'rect') {
        rectStart = null;
        rectCurrent = null;
        redraw();
    }
}

export function getShapeData() {
    // Pega as dimensões ATUAIS do canvas (que está sincronizado com o vídeo)
    const w = canvas.width;
    const h = canvas.height;

    let result = {
        width: 0, height: 0, points: []
    };

    if (w === 0 || h === 0) return result; // Proteção contra divisão por zero

    // --- MODO RETÂNGULO (Face) ---
    if (currentMode === 'rect' && rectStart && rectCurrent) {
        // 1. Calcula em PIXELS
        let pixelX = rectCurrent.w < 0 ? rectStart.x + rectCurrent.w : rectStart.x;
        let pixelY = rectCurrent.h < 0 ? rectStart.y + rectCurrent.h : rectStart.y;
        let pixelW = Math.abs(rectCurrent.w);
        let pixelH = Math.abs(rectCurrent.h);

        // 2. Converte para RELATIVO (0.0 a 1.0)
        result.width = pixelW / w;
        result.height = pixelH / h;

        // Salva Ponto inicial normalizado
        result.points = [{
            x: pixelX / w,
            y: pixelY / h
        }];
    }
    // --- MODO POLÍGONO (Ambiente) ---
    else if (currentMode === 'polygon' && points.length > 0) {
        let minX = Infinity, minY = Infinity, maxX = -Infinity, maxY = -Infinity;

        // Mapeia todos os pontos para relativo
        const relativePoints = points.map(p => {
            if (p.x < minX) minX = p.x;
            if (p.y < minY) minY = p.y;
            if (p.x > maxX) maxX = p.x;
            if (p.y > maxY) maxY = p.y;

            return {
                x: p.x / w,
                y: p.y / h
            };
        });

        // Salva bounding box relativa e pontos relativos
        result.width = (maxX - minX) / w;
        result.height = (maxY - minY) / h;
        result.points = relativePoints;
    }

    return result;
}

export function renderExistingShape(data, mode) {
    resetState();

    const w = canvas.width;
    const h = canvas.height;

    // Proteções contra nulos
    if (w === 0 || h === 0 || !data) return;

    // --- CARREGAR POLÍGONO ---
    if (mode === 'polygon' && data.points && data.points.length > 1) {
        points = data.points.map(p => ({
            x: p.x * w,
            y: p.y * h
        }));
        currentMode = 'polygon';
    }
    // --- CARREGAR RETÂNGULO ---
    // Verifica se tem pontos (onde guardamos X/Y) e largura
    else if (mode === 'rect' && data.points && data.points.length > 0) {
        // Recupera X/Y do primeiro ponto (normalizado 0-1) e converte para pixel
        let startX = data.points[0].x * w;
        let startY = data.points[0].y * h;

        let width = data.width * w;
        let height = data.height * h;

        rectStart = { x: startX, y: startY };
        rectCurrent = { w: width, h: height };
        currentMode = 'rect';
    }

    redraw();
}
// --- Eventos ---

function onMouseDown(e) {
    if (!isActive) return;
    e.preventDefault();
    const { x, y } = getMousePos(e);

    if (currentMode === 'rect') {
        isDragging = true;
        rectStart = { x, y };
        rectCurrent = { w: 0, h: 0 };
    }
    else if (currentMode === 'polygon') {
        // Fechar polígono
        if (points.length > 2) {
            const dist = Math.sqrt((points[0].x - x) ** 2 + (points[0].y - y) ** 2);
            if (dist < 10) {
                closePolygon();
                return;
            }
        }
        points.push({ x, y });
    }
    redraw();
}

function onMouseMove(e) {
    if (!isActive) return;

    if (currentMode === 'rect') {
        if (!isDragging || !rectStart) return;

        const { x, y } = getMousePos(e);
        rectCurrent.w = x - rectStart.x;
        rectCurrent.h = y - rectStart.y;

        requestAnimationFrame(redraw);
    }
}

function onMouseUp(e) {
    // Soltar o mouse para de arrastar, mas mantém isActive true (modo de edição ainda ligado)
    if (isDragging) {
        isDragging = false;
    }
}

function getMousePos(evt) {
    const rect = canvas.getBoundingClientRect();
    return {
        x: evt.clientX - rect.left,
        y: evt.clientY - rect.top
    };
}

function closePolygon() {
    redraw();
}

function cancelDrawing() {
    stopDrawing();
    clearCanvas();
}

function redraw() {
    if (!ctx) return;

    ctx.clearRect(0, 0, canvas.width, canvas.height);
    ctx.lineWidth = 3;
    ctx.strokeStyle = drawColor;
    ctx.fillStyle = fillColor;

    // Desenha Polígono (Somente se pontos existirem)
    if (points.length > 0) {
        ctx.beginPath();
        ctx.moveTo(points[0].x, points[0].y);
        for (let i = 1; i < points.length; i++) {
            ctx.lineTo(points[i].x, points[i].y);
        }
        if (points.length > 2) {
            ctx.closePath();
            ctx.fill();
        }
        ctx.stroke();

        points.forEach(p => {
            ctx.beginPath();
            ctx.arc(p.x, p.y, 4, 0, 2 * Math.PI);
            ctx.fillStyle = nodeColor;
            ctx.fill();
        });
    }

    // Desenha Retângulo (Somente se dados existirem)
    if (rectStart && rectCurrent) {
        ctx.beginPath();
        ctx.rect(rectStart.x, rectStart.y, rectCurrent.w, rectCurrent.h);
        ctx.fill();
        ctx.stroke();

        // Ponto de visualização no canto
        ctx.beginPath();
        ctx.arc(rectStart.x + rectCurrent.w, rectStart.y + rectCurrent.h, 3, 0, 2 * Math.PI);
        ctx.fillStyle = nodeColor;
        ctx.fill();
    }
}