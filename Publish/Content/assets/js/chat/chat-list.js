function listComponent() {
    return toAlpine({
        data: {
            list: new Map,
            chats: [],
        },
        methods: {
            init() {
                chatEvents.on('list', event => this.onData(event.detail))
                chatEvents.on('loading:list', event => this.status.set(event.detail ? rxStatus.loading : rxStatus.success))
                
                const input = document.querySelector('#search-input')
                if(input) input.addEventListener('keydown', () => this.onSearch(input.value))

            },
            onSearch(filter) {
                const groups = Object.entries(Object.fromEntries(this.list.entries()));

                this.chats = (filter && filter.length > 1) ? groups
                    .map(([key, chats]) => [key, chats.filter(c => JSON.stringify(Object.values(c)).toLowerCase().includes(filter.toLowerCase()))])
                    .filter(([_, chats]) => chats.length) : groups;
            },
            onData(chats) {
                const fnGrouping = c => {
                    const date = new Date();
                    const date1 = new Date(c.last_available_on);

                    const hours = date.getHours();
                    const hours1 = date1.getHours();

                    return c.is_closed
                        ? Infinity
                        : (date.getDate() > date1.getDate())
                            ? ((hours + 24) - hours1)
                            : (hours - hours1)
                };

                this.list = new Map([...Utils.arrayGroupBy(chats, fnGrouping).entries()]
                    .sort((a, b) => a[0] - b[0])
                    .map(([key, value]) => {
                        let kp = {key, value}

                        kp.value = value.sort((x, z) => {
                            let d1 = 0, d2 = 0
                            if (x.last_available_on) d1 = Date.parse(x.last_available_on)
                            if (z.last_available_on) d2 = Date.parse(z.last_available_on)

                            return d2 - d1
                        })

                        if (key < 1) kp.key = `Sent just now`
                        else if (key === 1) kp.key = `Sent an hour ago`
                        else if (key === Infinity) kp.key = `Closed`
                        else kp.key = `Sent ${key} hours ago`

                        return [kp.key, kp.value]
                    }))

                this.onSearch('')
            },
            refresh() {
                chatEvents.dispatch('refresh')
            },
        }
    });
}