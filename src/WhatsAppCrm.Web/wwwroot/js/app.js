// WhatsApp CRM - JS Interop helpers for Blazor Server

/**
 * Scroll an element to the bottom (for chat messages auto-scroll)
 */
window.scrollToBottom = (elementId) => {
    const el = document.getElementById(elementId);
    if (el) {
        el.scrollTop = el.scrollHeight;
    }
};

/**
 * Auto-resize a textarea based on its content
 */
window.autoResizeTextarea = (element) => {
    if (element) {
        element.style.height = 'auto';
        element.style.height = Math.min(element.scrollHeight, 120) + 'px';
    }
};

/**
 * Focus an element by its ID
 */
window.focusElement = (elementId) => {
    const el = document.getElementById(elementId);
    if (el) el.focus();
};

/**
 * Copy text to clipboard
 */
window.copyToClipboard = async (text) => {
    try {
        await navigator.clipboard.writeText(text);
        return true;
    } catch {
        return false;
    }
};
