function historyComponent() {
    return toAlpine({
        data: {
            messages: [],
            dates: [],
            chat: {},
        },
        methods: {
            init: function () {
                const params = query()

                chatEvents.on('history', async event => {
                    this.chat = event.detail;
                    if(this.chat) await this.getDates()
                })

                if (params.tab && params.chat) {
                    history.pushState(null, null, window.location.href.split('?')[0]);
                    chatEvents.dispatch('history', {
                        phone: params.chat,
                        name: params.name,
                        acc_number: params.account,
                    })
                }
            },
            scrollChatsToBottom() {
                this.$nextTick(() => {
                    this.$refs.chat.scrollTo(0, this.$refs.chat.scrollHeight)
                })
            },
            scrollChatsToTop() {
                this.$nextTick(() => {
                    this.$refs.chat.scrollTo(0, 0)
                })
            },
            async getDates() {
                this.chats = []
                this.dates = await this.status.subscribe(ChatManager.historyDates(this.chat.phone), 'history')
            },
            async getChats(date) {
                this.messages = (await this.status.subscribe(ChatManager.historyChats(this.chat.phone, date), 'chats')).map(toChatMessage)
            },
        }
    });
}