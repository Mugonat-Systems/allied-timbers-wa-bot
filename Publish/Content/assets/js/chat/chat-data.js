// Chats data
const ChatManager = {
    _sortList(chats) {
        return chats.slice().sort((a, b) => {
            if (a.Timestamp && b.Timestamp) return Date.parse(b.Timestamp) - Date.parse(a.Timestamp)
            if (a.last_replied_on) return Date.parse(b.last_replied_on) - Date.parse(a.last_replied_on)
            return 0
        })
    },
    _sortChats(chats) {
        return chats.slice().sort((a, b) => {
            if (a.Timestamp && b.Timestamp) return Date.parse(a.Timestamp) - Date.parse(b.Timestamp)
            if (a.last_replied_on) return Date.parse(a.last_replied_on) - Date.parse(b.last_replied_on)
            return 0
        })
    },
    chats() {
        return fetch('/api/whatsapp-web/snapshot')
            .then(r => r.json())
            .then(chats => this._sortList(chats))
    },
    chat(phone) {
        if (phone) return fetch(`/api/whatsapp-web/${phone}`)
            .then(r => r.json())
            .then(chats => this._sortChats(chats))
        else return Promise.resolve([])
    },
    historyDates(phone) {
        if (phone) return fetch(`/api/whatsapp-web/dates/${phone}`)
            .then(r => r.json())
            .then(d => d.sort((a, b) => new Date(b.date) - new Date(a.date)))
        else return Promise.resolve([])
    },
    historyChats(phone, date) {
        if (phone) return fetch(`/api/whatsapp-web/history/${phone}?date=${date}`).then(r => r.json())
        else return Promise.resolve([])
    },
    playAudio(debugSource = 'none') {
        $('#not-audio').get(0).play()

        console.log(`[Audio] ${debugSource}`)
    },
    visibility: (function () {
        let stateKey, eventKey, keys = {
            hidden: "visibilitychange",
            webkitHidden: "webkitvisibilitychange",
            mozHidden: "mozvisibilitychange",
            msHidden: "msvisibilitychange"
        };

        for (stateKey in keys) {
            if (stateKey in document) {
                eventKey = keys[stateKey];
                break;
            }
        }

        return function (c) {
            if (c) document.addEventListener(eventKey, c);
            return !document[stateKey];
        }
    })()
};

(function () {
    "use strict";
    
    window.visibilityCount = 0;
    window.visibilityCounter = undefined;
    window.visibilityTimeout = 320;

    /// When user has been away for {window.visibilityTimeout} seconds or more then reload page
    ChatManager.visibility(function () {
        const visibility = ChatManager.visibility();

        if (visibility) {
            if (window.visibilityCounter) clearInterval(window.visibilityCounter)
            if (window.visibilityCount > window.visibilityTimeout) location.reload()

            window.visibilityCount = 0;
        } else {
            window.visibilityCounter = setInterval(() => {
                window.visibilityCount++;

                if (window.visibilityCount > window.visibilityTimeout) {
                    if (window.hasOwnProperty('__interval')) {
                        window.__interval.cancel()
                    }
                }
            }, 1000)
        }
    })
})()

