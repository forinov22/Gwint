function setupCardAdjustments() {
    function adjustCardPositions() {
        const gameAction = document.querySelector('.game-action');
        if (!gameAction) return;

        const unitRows = gameAction.querySelectorAll('.unit-row');

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

    adjustCardPositions();
    
    window.addEventListener('resize', adjustCardPositions);
}

function cleanupCardAdjustments() {
    window.removeEventListener('resize', adjustCardPositions);
}

function scrollElement(element, delta, direction) {
    delta = direction ? delta : -delta;
    if (element) {
        element.scrollLeft += delta;
    }
}

export { setupCardAdjustments, cleanupCardAdjustments, scrollElement };
