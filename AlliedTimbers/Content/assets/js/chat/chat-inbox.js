function inboxComponent() {
    return toAlpine({
        data: {
            files: [],
            dragging: false,
            chat: {},
            message: '',
            messages: [],
            dates: [],
            hasChat: false,
            suggestions: this.$persist(suggestionCrud.data).as('suggestions')
        },
        methods: {
            init() {
                chatEvents.on('open', event => {
                    const chat = event.detail

                    if (chat.phone !== this.chat.phone) {
                        if (this.chat.phone) {
                            chatEvents.off(this.chat.phone, window[this.chat.phone].data);
                            chatEvents.off(`loading:${this.chat.phone}`, window[this.chat.phone].loading);
                            localStorage.setItem('message-' + this.chat.phone, this.message)
                        }

                        if (chat) {
                            this.chat = chat
                            this.hasChat = true
                            this.message = localStorage.getItem('message-' + chat.phone) || ''
                            this.dates = []
                            this.messages = []

                            window[chat.phone] = {
                                data: (event) => this.onData(event.detail),
                                loading: (event) => this.status.set(event.detail ? rxStatus.loading : rxStatus.default)
                            }

                            chatEvents.on(chat.phone, window[chat.phone].data);
                            chatEvents.on(`loading:${chat.phone}`, window[chat.phone].loading);
                        }
                    }

                    this.onData(window[`chats:${chat.phone}`] || [])

                    chatEvents.dispatch('refresh')
                })
            },
            description() {

                if (!this.chat) return ''

                let desc = ['Phone: ' + this.chat.phone]

                if (this.chat.currentIssue) desc.push('Issue: ' + this.chat.currentIssue)
                if (this.chat.language) desc.push(', Preferred Language: ' + this.chat.language)

                return desc.join(', ')
            },
            grow() {
                const chatMessage = this.$refs.chatMessage
                if (chatMessage) {
                    chatMessage.style.transition = "height 0.4s ease-in-out"
                    chatMessage.style.maxHeight = "200px";
                    chatMessage.style.height = (chatMessage.scrollHeight) + "px";

                    if (!chatMessage.value) chatMessage.style.height = null
                }
            },
            suggestionList() {
                return this.suggestions
                    .filter(({Key, Value}) => `/${Key}`.startsWith(this.suggestionKey()))
                    .map(({Key, Value}) => ({trigger: Key, message: Value}))
            },
            suggestionKey() {
                return this.message.substring(this.message.indexOf('/'))
            },
            suggestionReplace(suggestion) {
                this.message = this.message.replace(this.suggestionKey(), suggestion.message)
            },
            async suggestionSave(thread) {
                if (!thread.type || thread.type === 'text') {
                    const message = thread.message
                    const trigger = prompt(`Save quick response\n Enter a trigger for message \n "${message}"`)
                    if (trigger) {
                        await suggestionCrud.create(trigger, message)
                        this.suggestions = await suggestionCrud.read('', true)
                    }
                }
            },
            scroll(toBottom = true) {
                const chatElement = this.$refs.chat;
                if (chatElement) this.$nextTick(() => chatElement.scrollTo(0, toBottom ? chatElement.scrollHeight : 0))
            },
            getThread(date) {
                return this.messages.filter(m => m.timestamp instanceof (Date) && m.timestamp.toLocaleDateString() === date)
            },
            dragStart() {
                this.dragging = true
            },
            dragDrop(input) {
                if (input.files.length) {
                    for (const file of input.files) {
                        if (file.type.startsWith("image")) {
                            let reader = new FileReader();
                            reader.onload = e => this.files.push({preview: e.target.result, file})
                            reader.onerror = e => this.files.push({file})
                            reader.readAsDataURL(file);
                        } else {
                            this.files.push({
                                preview: `https://placehold.jp/3d48e6/ffffff/200x100.jpg?text=${file.name}`,
                                file
                            })
                        }
                    }
                }

                input.value = null
            },
            dragStop() {
                setTimeout(() => this.dragging = false, 500)
            },
            removeAttachment(name) {
                this.files = this.files.filter(f => f.file.name !== name)
            },
            onData(messages) {
                this.messages = messages.map(toChatMessage)
                this.dates = [...new Set(this.messages.map(c => c.timestamp.toLocaleDateString()))]
                this.chat.platform = [...this.messages].reverse()[0].platform
                this.scroll()
            },
            async send(close = false) {
                if (close) {
                    if (!confirm('Are you sure you want to close this chat?')) {
                        return
                    }
                }

                const chatId = this.chat.phone
                const promise = async () => {
                    const person = { Platform: this.chat.platform,
                        Message: close ? 'xxx-yyy-zzz' : this.message || '',
                        MessageType: 'text',
                        MessageUrl: null,
                        From: chatId,
                        To: chatId,
                        IsClosed: close
                    };

                    if (this.files.length) {
                        const formData = new FormData()
                        
                        for (let file of this.files) {
                            formData.append('files', file.file)
                        }

                        const uploadRequest = await fetch('/media/upload', {
                            body: formData,
                            method: 'post'
                        });
                        const {messageUrl} = await uploadRequest.json()

                        person.MessageUrl = messageUrl
                    }

                    const response = await fetch(`/api/whatsapp-web/send?phone=${chatId}`, {
                        body: JSON.stringify(person),
                        headers: new Headers({"Content-Type": "application/json"}),
                        method: 'post', 
                        redirect: 'follow'
                    })

                    if (response.ok && !close) {
                        this.message = ''
                        this.files = []
                        await this.onData(await response.json())
                    }
                };

                await this.status.subscribe(promise(), close ? 'close' : 'send')

                if (close) this.hasChat = false
            },
        }
    })
}
