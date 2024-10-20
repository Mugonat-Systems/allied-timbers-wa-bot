function chatsComponent() {
    return toAlpine({
        data: {
            suggestions: false,
            history: false,
            withNewMessages: false,
            chats: []
        },
        methods: {
            init() {
                chatEvents.on('open', (event) => {
                    this.history = false
                    this.withNewMessages = false
                    this.chats = Array.from(new Set([event.detail.phone, ...this.chats])).slice(0, 3)
                })
                chatEvents.on('history', (event) => this.history = !!event.detail)
                chatEvents.on('suggestions', (event) => this.suggestions = !!event.detail)
                chatEvents.on('refresh', (event) => this.getMessages(!!event.detail))

                let start = true

                window.__interval = setCustomAsyncInterval(async () => {
                    await this.getMessages(start);
                    start = false
                }, 7000, true)

                imageSpotlight()
            },
            async getMessages(showStatus = true) {
                async function fetchAllChats() {
                    if (showStatus) chatEvents.dispatch(`loading:list`, true)

                    const chats = await ChatManager.chats()

                    if (!Utils.arrayEquals(window['chats:list'] || [], chats)) {
                        chatEvents.dispatch('list', window['chats:list'] = chats)
                        this.withNewMessages = chats.some(c => c.new_messages_count > 0)

                        if (this.withNewMessages) ChatManager.playAudio()
                    }

                    if (showStatus) chatEvents.dispatch(`loading:list`, false)
                }

                async function fetchIndividualChats() {
                    for (const key of this.chats) {
                        if (showStatus) chatEvents.dispatch(`loading:${key}`, true)
                        const personChats = await ChatManager.chat(key);

                        if (!Utils.arrayEquals(window[`chats:${key}`] || [], personChats)) {
                            chatEvents.dispatch(key, window[`chats:${key}`] = personChats)
                        }
                        if (showStatus) chatEvents.dispatch(`loading:${key}`, false)
                    }
                }

                await Promise.all([
                    fetchAllChats.call(this),
                    fetchIndividualChats.call(this)
                ])
            },
        },
    })
}