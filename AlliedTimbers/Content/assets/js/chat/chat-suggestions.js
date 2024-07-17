const suggestionCrud = {
    data: [],
    api: '/api/responses',
    read(id = '', force = false) {
        if (!this.data.length || force)
            return fetch(`${this.api}/${id}`).then(r => r.json()).then(d => this.data = d)

        return Promise.resolve(this.data)
    },
    create(trigger, message) {
        return fetch(this.api, {
            method: "post",
            headers: new Headers({'Content-Type': 'application/json'}),
            body: JSON.stringify({
                key: trigger.replace(/\//g, ''),
                value: message
            })
        }).then(r => r.json())
    },
    update(id, {trigger, message}) {
        return fetch(`${this.api}/${id}`, {
            method: "put",
            headers: new Headers({'Content-Type': 'application/json'}),
            body: JSON.stringify({
                key: trigger.replace(/\//g, ''),
                value: message
            })
        }).then(r => r.json())
    },
    delete(id) {
        return fetch(`${this.api}/DeleteChatQuickResponse/${id}`, {
            method: "delete",
            headers: new Headers({'Content-Type': 'application/json'}),
        })
    }
}

function suggestionsComponent() {
    return toAlpine({
        data: {
            suggestionsList: this.$persist(suggestionCrud.data).as('suggestions'),
            suggestion: {
                trigger: '',
                message: ''
            },
        },
        methods: {
            init() {
                return this.getSuggestions()
            },
            async getSuggestions() {
                this.suggestionsList = await this.status.subscribe(suggestionCrud.read('', true))
            },
            async saveSuggestion() {
                await this.status.subscribe(suggestionCrud.create(
                    this.suggestion.trigger,
                    this.suggestion.message
                ), 'save')

                this.suggestion = {
                    trigger: '',
                    message: ''
                }

                await this.getSuggestions()
            },
            updateSuggestion() {
            },
            async deleteSuggestion(id) {
                await suggestionCrud.delete(id)

                await this.getSuggestions()
            },
        }
    });
}