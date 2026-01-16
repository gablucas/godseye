window.cameraFunctions = {
    startCamera: async function () {
        const video = document.getElementById("video");
        if (navigator.mediaDevices.getUserMedia) {
            const stream = await navigator.mediaDevices.getUserMedia({ video: true });
            video.srcObject = stream;
        }
    },

    capturePhoto: function () {
        const video = document.getElementById("video");
        const canvas = document.getElementById("canvas");

        // ajusta tamanho
        canvas.width = video.videoWidth;
        canvas.height = video.videoHeight;

        const ctx = canvas.getContext("2d");
        ctx.drawImage(video, 0, 0, canvas.width, canvas.height);

        // retorna base64
        return canvas.toDataURL("image/png");
    }
}