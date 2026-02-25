document.addEventListener("DOMContentLoaded", () => {
    const messages = [
        "Carregando página...",
        "Aguarde um momento...",
        "Preparando tudo para você...",
        "Quase pronto..."
    ];

    const textEl = document.getElementById("loading-text");
    if (!textEl) {
        console.warn("Elemento #loading-text não encontrado.");
        return;
    }

    let index = 0;
    textEl.textContent = messages[index]; // garante a primeira mensagem

    setInterval(() => {
        index = (index + 1) % messages.length;
        textEl.textContent = messages[index];
    }, 1000);
});