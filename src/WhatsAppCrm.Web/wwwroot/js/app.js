// JS Interop functions for Blazor components

window.scrollToBottom = (elementId) => {
    const el = document.getElementById(elementId);
    if (el) {
        el.scrollTop = el.scrollHeight;
    }
};

window.autoResizeTextarea = (element) => {
    if (element) {
        element.style.height = 'auto';
        element.style.height = Math.min(element.scrollHeight, 120) + 'px';
    }
};

window.focusElement = (elementId) => {
    const el = document.getElementById(elementId);
    if (el) el.focus();
};
