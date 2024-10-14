document.addEventListener('alpine:init', function () {
    Alpine.data('list', listComponent)
    Alpine.data('inbox', inboxComponent)
    Alpine.data('history', historyComponent)
    Alpine.data('suggestions', suggestionsComponent)
    Alpine.data('chats', chatsComponent)
})
