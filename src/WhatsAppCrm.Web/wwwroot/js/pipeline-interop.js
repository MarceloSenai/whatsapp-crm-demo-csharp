// Pipeline Kanban — SortableJS interop for Blazor Server
// Enables drag-and-drop of deal cards between pipeline stages

/**
 * Initialize SortableJS on all kanban columns.
 * @param {DotNetObjectReference} dotNetHelper — Blazor component reference for callbacks
 */
window.initSortable = (dotNetHelper) => {
    // Clean up any previous instances
    window.destroySortable();

    const columns = document.querySelectorAll('.kanban-sortable');
    columns.forEach(column => {
        column._sortable = new Sortable(column, {
            group: 'pipeline',          // Allow dragging between all columns
            animation: 200,             // Smooth animation duration (ms)
            easing: 'cubic-bezier(0.25, 1, 0.5, 1)',
            ghostClass: 'sortable-ghost',
            chosenClass: 'sortable-chosen',
            dragClass: 'sortable-drag',
            delay: 50,                  // Small delay to prevent accidental drags
            delayOnTouchOnly: true,     // Delay only on touch devices
            touchStartThreshold: 3,     // Pixels of movement before drag starts (touch)
            fallbackOnBody: true,       // Append ghost to body for better z-index
            swapThreshold: 0.65,        // Threshold for swapping position

            onEnd: async (evt) => {
                const dealId = evt.item.dataset.dealId;
                const newStageId = evt.to.dataset.stageId;
                const oldStageId = evt.from.dataset.stageId;

                if (dealId && newStageId && newStageId !== oldStageId) {
                    try {
                        await dotNetHelper.invokeMethodAsync('OnDealMoved', dealId, newStageId);
                    } catch (err) {
                        console.error('[Pipeline] Failed to move deal:', err);
                        // Revert: move the element back to its original position
                        if (evt.from !== evt.to) {
                            evt.from.insertBefore(evt.item, evt.from.children[evt.oldIndex] || null);
                        }
                    }
                }
            }
        });
    });
};

/**
 * Destroy all SortableJS instances (cleanup on component dispose).
 */
window.destroySortable = () => {
    const columns = document.querySelectorAll('.kanban-sortable');
    columns.forEach(column => {
        if (column._sortable) {
            column._sortable.destroy();
            column._sortable = null;
        }
    });
};
