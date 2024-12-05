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

function setupCardAdjustments() {


    adjustCardPositions();

    window.addEventListener('resize', adjustCardPositions);
}

function cleanupCardAdjustments() {
    window.removeEventListener('resize', adjustCardPositions);
}

function rotateOwnCards() {
    const own_cards = document.querySelectorAll('.own');
    const totalCards = own_cards.length;
    if (totalCards != 1) {
        const minRotate = -40;
        const maxRotate = 40;

        own_cards.forEach((card, index) => {
            const angle = minRotate + (maxRotate - minRotate) * (index / (totalCards - 1));

            card.style.transform = `rotateX(15deg) rotate(${angle}deg)`;
        });
    }
    else {
        own_cards[0].style.transform = 'rotateX(15deg) rotate(0)';
    }
}
function rotateOpponentCards() {
    const opponent_cards = document.querySelectorAll('.opponent');
    const totalCards = opponent_cards.length;
    if (totalCards != 1) {
        const minRotate = 220;
        const maxRotate = 140;

        opponent_cards.forEach((card, index) => {
            const angle = minRotate + (maxRotate - minRotate) * (index / (totalCards - 1));

            card.style.transform = `rotateX(40deg) rotate(${angle}deg)`;
        });
    }
    else {
        opponent_cards[0].style.transform = 'rotateX(40deg) rotate(180deg)';
    }
}

var timeout = null;

function initializeCardEvents() {
    $(document).off('click', '.field');
    $(document).off('click', '.own');
    $(document).on('click', '.own', function () {
        $(this).toggleClass('flipped');
    });
    $(document).on('click', '.field', function () {
        $(this).toggleClass('flipped');
    });
}

function allowDrop(event) {
    event.preventDefault();
}

let offsetX = 0;
let offsetY = 0;
let card = null;

function dragStart(e, cardId) {
    card = document.getElementById('card-' + cardId);

    const rect = card.getBoundingClientRect();
    offsetX = e.clientX - rect.left;
    offsetY = e.clientY - rect.top;
    card.style.height = '140px';
    //card.style.width = '70px';
    card.style.margin = '0';
    card.style.setProperty('top', (e.clientY - offsetY) + 'px', 'important');
    card.style.setProperty('left', (e.clientX - offsetX) + 'px', 'important');
    card.style.setProperty('position', 'fixed', 'important');
}

function drag(e, cardId) {
    card.style.setProperty('top', (e.clientY - offsetY) + 'px', 'important');
    card.style.setProperty('left', (e.clientX - offsetX) + 'px', 'important');
}

function drop(event, gameRoomInstance) {
    const cardId = (card.id).substring(5);
    console.log(card.style.top)
    card.style.height = '';
    card.style.width = '';
    card.style.margin = '';
    card.style.top = ''; 
    card.style.left = '';
    card.style.position = '';
    // Затем вызываем MoveCard
    gameRoomInstance.invokeMethodAsync('MoveCard', cardId)
        .then(() => {
            console.log("Card moved successfully.");
        })
        .catch(error => {
            console.error("Failed to move card:", error);
        });
}




function scrollElement(element, delta, direction) {
    delta = direction ? delta : -delta;
    if (element) {
        element.scrollLeft += delta;
    }
}

window.getBoundingClientRect = (element) => {
    if (!element) {
        console.error("Element is null in getBoundingClientRect");
        return null;
    }
    const rect = element.getBoundingClientRect();
    return {
        left: rect.left,
        top: rect.top,
        width: rect.width,
        height: rect.height
    };
};

function changeFavicon(newIconPath) {
    var link = document.querySelector("link[rel='icon']") || document.createElement('link');
    link.type = 'image/png';
    link.rel = 'icon';
    link.href = newIconPath;
    document.head.appendChild(link);
}

function initializeDragAndDrop(cardPrefix, playZoneId, deckZoneId) {
    const playZone = document.getElementById(playZoneId);
    const deckZone = document.getElementById(deckZoneId);

    document.querySelectorAll(`[id^=${cardPrefix}]`).forEach(card => {
        let isDragging = false;

        const drag = (e) => {
            if (!isDragging) return;
            card.style.position = 'absolute';
            card.style.top = `${e.clientY - card.offsetHeight / 2}px`;
            card.style.left = `${e.clientX - card.offsetWidth / 2}px`;
        };

        const mouseUp = (e) => {
            isDragging = false;
            card.style.cursor = 'grab';

            const cardRect = card.getBoundingClientRect();
            const playZoneRect = playZone.getBoundingClientRect();
            const deckZoneRect = deckZone.getBoundingClientRect();

            if (isOverZone(cardRect, playZoneRect)) {
                console.log('Card played!');
                card.classList.add('played');
            } else if (isOverZone(cardRect, deckZoneRect)) {
                console.log('Card returned to deck!');
                card.classList.add('return');
            } else {
                resetCardPosition(card);
            }

            window.removeEventListener('mousemove', drag);
        };

        const mouseDown = () => {
            isDragging = true;
            card.style.cursor = 'grabbing';
            card.classList.remove('played', 'return');
            window.addEventListener('mousemove', drag);
        };

        card.addEventListener('mousedown', mouseDown);
        window.addEventListener('mouseup', mouseUp);
    });

    const isOverZone = (cardRect, zoneRect) => {
        return (
            cardRect.top >= zoneRect.top &&
            cardRect.bottom <= zoneRect.bottom &&
            cardRect.left >= zoneRect.left &&
            cardRect.right <= zoneRect.right
        );
    };

    const resetCardPosition = (card) => {
        card.style.top = '';
        card.style.left = '';
        card.style.transform = '';
    };
}

document.addEventListener('DOMContentLoaded', () => {
    const onDragStart = (event) => {
        event.dataTransfer.setData('text/plain', event.target.id);
    };

    const onDragOver = (event) => {
        event.preventDefault(); // Разрешает сброс
    };

    const onDrop = (event) => {
        event.preventDefault();
        const cardId = event.dataTransfer.getData('text/plain');
        const draggedElement = document.getElementById(cardId);
        const dropZone = event.currentTarget;

        if (dropZone && draggedElement) {
            dropZone.appendChild(draggedElement);

            // Notify Blazor
            const dotNetRef = dropZone.getAttribute('data-dotnet-ref');
            if (dotNetRef) {
                DotNet.invokeMethodAsync('YourNamespace', 'CardDropped', cardId, dropZone.id);
            }
        }
    };

    window.onDragStart = onDragStart;
    window.onDragOver = onDragOver;
    window.onDrop = onDrop;
});

const onDrop = (event) => {
    event.preventDefault();
    const cardId = event.dataTransfer.getData('text/plain');
    const dropZone = event.currentTarget;

    if (dropZone && cardId) {
        const dotNetRef = dropZone.getAttribute('data-dotnet-ref');
        if (dotNetRef) {
            // Вызов C# метода с cardId и targetRowId
            DotNet.invokeMethodAsync('YourNamespace', 'CardDropped', cardId, dropZone.id);
        }
    }
};


export { setupCardAdjustments, cleanupCardAdjustments, scrollElement, initializeDragAndDrop, onDrop, rotateOpponentCards, rotateOwnCards, initializeCardEvents, changeFavicon, drag, drop, dragStart };
