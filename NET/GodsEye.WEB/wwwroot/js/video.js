
window.goto = function (seconds) {
    const video = document.getElementById("player");

    // garante que os metadados já carregaram
    if (video.readyState >= 1) {
        video.currentTime = seconds;
    video.play(); // opcional
    } else {
        video.addEventListener("loadedmetadata", () => {
            video.currentTime = seconds;
            video.play();
        }, { once: true });
    }
}
