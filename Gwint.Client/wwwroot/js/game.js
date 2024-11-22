function setupCardAdjustments() {
    function adjustCardPositions(wrapper) {
        const gameAction = document.querySelector('.game-action');
        if (!gameAction) return; // Ensure the target element exists

        // const unitRows = gameAction.querySelectorAll('.unit-row');
        const unitRows = document.querySelectorAll(wrapper);

        unitRows.forEach(unitRow => {
            const cards = unitRow.querySelectorAll('.unit-card-wrapper');
            
            if (cards.length > 0) {
                const totalCardsWidth = cards.length * cards[0].offsetWidth;
                const availableWidth = gameAction.offsetWidth;

                if (totalCardsWidth > availableWidth) {
                    const overlapOffset = (totalCardsWidth - availableWidth) / (cards.length - 1);
                    cards.forEach((card, index) => {
                        card.style.transform = `translateX(${-index * overlapOffset}px)`;
                    });
                }
            }
        });
    }

    adjustCardPositions('.unit-row');
    adjustCardPositions('.card-selection');
    
    window.addEventListener('resize', () => {
        adjustCardPositions('.unit-row');
        adjustCardPositions('.card-selection');
    });
}

function cleanupCardAdjustments() {
    // window.removeEventListener('resize', adjustCardPositions);
}

export { setupCardAdjustments, cleanupCardAdjustments };
