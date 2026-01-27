let canvas;
let ctx;

let startX = 0;
let startY = 0;
let isDrawing = false;
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

    const pos = getMousePos(e);

    const x = Math.min(startX, pos.x);
    const y = Math.min(startY, pos.y);
    const width = Math.abs(pos.x - startX);
    const height = Math.abs(pos.y - startY);

    ctx.clearRect(0, 0, canvas.width, canvas.height);

    ctx.strokeStyle = "#00ff00";
    ctx.lineWidth = 2;
    ctx.strokeRect(x, y, width, height);

    rect = { x, y, width, height };
}

export function setStrokeRect(rect) {
    if (!rect || !ctx || !canvas) return;

    ctx.clearRect(0, 0, canvas.width, canvas.height);

    ctx.strokeStyle = "#00ff00";
    ctx.lineWidth = 2;

    ctx.strokeRect(
        rect.x,
        rect.y,
        rect.width,
        rect.height
    );
}

function getMousePos(e) {
    const rect = canvas.getBoundingClientRect();

    const scaleX = canvas.width / rect.width;
    const scaleY = canvas.height / rect.height;

    return {
        x: (e.clientX - rect.left) * scaleX,
        y: (e.clientY - rect.top) * scaleY
    };
}

function onMouseUp() {
    if (!drawingEnabled) return;
    isDrawing = false;
}

export function enableDrawing() {
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
