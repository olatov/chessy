/**
 * Copies the provided text to the clipboard.
 *
 * @param {string} text - The text to copy.
 * @return {Promise<void>} A promise that resolves when the text is successfully copied.
 */
function copyTextToClipboard(text) {
    return navigator.clipboard.writeText(text);
}

/**
 * Scrolls the given element to the bottom of its content.
 *
 * @param {HTMLElement} element - The element to scroll.
 * @return {void} This function does not return a value.
 */
function scrollElementToBottom(element) {
    element.scrollTop = element.scrollHeight;
}
