let canvas;
let ctx;

let startX = 0;
let startY = 0;
let isDrawing = false;
let isSynced = false;
let drawingEnabled = false;

let rect = null;

export function initRoiCanvas() {
    canvas = document.getElementById("roiCanvas");
    ctx = canvas.getContext("2d");

    canvas.addEventListener("mousedown", onMouseDown);
    canvas.addEventListener("mousemove", onMouseMove);
    canvas.addEventListener("mouseup", onMouseUp);

    disableDrawing()
}

function onMouseDown(e) {
    if (!drawingEnabled) return;

    const pos = getMousePos(e);

    isDrawing = true;
    startX = pos.x;
    startY = pos.y;
}

function onMouseMove(e) {
    if (!isDrawing || !drawingEnabled) return;

    const clamp = (v, min, max) => Math.max(min, Math.min(max, v));
    const pos = getMousePos(e);

    const endX = clamp(pos.x, 0, canvas.width);
    const endY = clamp(pos.y, 0, canvas.height);

    const x = Math.min(startX, endX);
    const y = Math.min(startY, endY);
    const width = Math.abs(endX - startX);
    const height = Math.abs(endY - startY);

    ctx.clearRect(0, 0, canvas.width, canvas.height);

    ctx.strokeStyle = "#00ff00";
    ctx.lineWidth = 2;
    ctx.strokeRect(x, y, width, height);

    // ⬇️ salva RELATIVO
    rect = {
        x: x / canvas.width,
        y: y / canvas.height,
        width: width / canvas.width,
        height: height / canvas.height
    };
}

export function setStrokeRect(rect) {
    if (!rect || !ctx || !canvas) return;

    ctx.clearRect(0, 0, canvas.width, canvas.height);

    ctx.strokeStyle = "#00ff00";
    ctx.lineWidth = 2;

    ctx.strokeRect(
        rect.x * canvas.width,
        rect.y * canvas.height,
        rect.width * canvas.width,
        rect.height * canvas.height
    );
}

function getMousePos(e) {
    const bounds = canvas.getBoundingClientRect();

    const scaleX = canvas.width / bounds.width;
    const scaleY = canvas.height / bounds.height;

    return {
        x: (e.clientX - bounds.left) * scaleX,
        y: (e.clientY - bounds.top) * scaleY
    };
}

function onMouseUp() {
    if (!drawingEnabled) return;
    isDrawing = false;
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

export function syncCanvasWithVideo(videoId) {
    const video = document.getElementById(videoId);
    if (!video || !canvas) return;

    const sync = () => {
        canvas.width = video.videoWidth;
        canvas.height = video.videoHeight;

        ctx.clearRect(0, 0, canvas.width, canvas.height);
        isSynced = true;
    };

    video.addEventListener("loadedmetadata", sync);
    sync();
}
