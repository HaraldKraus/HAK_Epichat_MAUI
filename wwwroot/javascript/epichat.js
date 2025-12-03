window.scrollToBottom = (element) => {
    if (element) {
        element.scrollTop = element.scrollHeight; // Scrollt das Div ganz nach unten (ChatMessages)
    }
};

window.openModal = (element) => {
    if (element && element.showModal) {
        element.showModal();  // ÷ffnet das Modal (native API)
    }
};

window.closeModal = (element) => {
    if (element && element.close) {
        element.close();  // Schlieﬂt das Modal
    }
};

window.closePicoDropdowns = () => {
    // Schlieﬂt alle offenen <details> (wird von pico.css f¸r Dropdowns genutzt)
    document.querySelectorAll('details[open]').forEach(details => {
        details.removeAttribute('open');
    });
}