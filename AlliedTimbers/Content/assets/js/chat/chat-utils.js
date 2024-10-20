// Utils 
const Utils = {
    arrayEquals(array1, array2) {
        // Get the value type
        let type = Object.prototype.toString.call(array1);

        // If the two objects are not the same type, return false
        if (type !== Object.prototype.toString.call(array2)) return false;

        // If items are not an object or array, return false
        if (['[object Array]', '[object Object]'].indexOf(type) < 0) return false;

        // Compare the length of the length of the two items
        let valueLen = type === '[object Array]' ? array1.length : Object.keys(array1).length;
        let otherLen = type === '[object Array]' ? array2.length : Object.keys(array2).length;
        if (valueLen !== otherLen) return false;

        // Compare two items
        let compare = function (item1, item2) {

            // Get the object type
            let itemType = Object.prototype.toString.call(item1);

            // If an object or array, compare recursively
            if (['[object Array]', '[object Object]'].indexOf(itemType) >= 0) {
                if (!Utils.arrayEquals(item1, item2)) return false;
            }

            // Otherwise, do a simple comparison
            else {

                // If the two items are not the same type, return false
                if (itemType !== Object.prototype.toString.call(item2)) return false;

                // Else if it's a function, convert to a string and compare
                // Otherwise, just compare
                if (itemType === '[object Function]') {
                    if (item1.toString() !== item2.toString()) return false;
                } else {
                    if (item1 !== item2) return false;
                }

            }
        };

        // Compare properties
        if (type === '[object Array]') {
            for (let i = 0; i < valueLen; i++) {
                if (compare(array1[i], array2[i]) === false) return false;
            }
        } else {
            for (let key in array1) {
                if (array1.hasOwnProperty(key)) {
                    if (compare(array1[key], array2[key]) === false) return false;
                }
            }
        }

        // If nothing failed, return true
        return true;
    },
    objectEquals(object1, object2) {
        if (object1 === null || object1 === undefined || object2 === null || object2 === undefined) {
            return object1 === object2;
        }
        // after this just checking type of one would be enough
        if (object1.constructor !== object2.constructor) {
            return false;
        }
        // if they are functions, they should exactly refer to same one (because of closures)
        if (object1 instanceof Function) {
            return object1 === object2;
        }
        // if they are regexps, they should exactly refer to same one (it is hard to better equality check on current ES)
        if (object1 instanceof RegExp) {
            return object1 === object2;
        }
        if (object1 === object2 || object1.valueOf() === object2.valueOf()) {
            return true;
        }
        if (Array.isArray(object1) && object1.length !== object2.length) {
            return false;
        }

        // if they are dates, they must had equal valueOf
        if (object1 instanceof Date) {
            return false;
        }

        // if they are strictly equal, they both need to be object at least
        if (!(object1 instanceof Object)) {
            return false;
        }
        if (!(object2 instanceof Object)) {
            return false;
        }

        // recursive object equality check
        let p = Object.keys(object1);
        return Object.keys(object2).every(function (i) {
                return p.indexOf(i) !== -1;
            }) &&
            p.every(function (i) {
                return Utils.objectEquals(object1[i], object2[i]);
            });
    },
    arrayGroupByKey(array, key) {
        return array.reduce(function (rv, x) {
            (rv[x[key]] = rv[x[key]] || []).push(x);
            return rv;
        }, {});
    },
    /**
     * @param list
     * @param keyGetter
     * @returns {Map<any, any>}
     */
    arrayGroupBy(list, keyGetter) {
        const map = new Map();
        for (const item of list) {
            const key = keyGetter(item);
            const collection = map.get(key);

            if (!collection) map.set(key, [item]);
            else collection.push(item);
        }
        return map;
    }
}

function dispatch(event, detail = {}) {
    document.dispatchEvent(new CustomEvent(event, {detail}))
}

function on(event, callback) {
    document.addEventListener(event, callback)
}

function off(event, callback) {
    document.removeEventListener(event, callback)
}

function getMomentFromDayDifference(secondsFromEpoch) {
    const oneDay = 24 * 60 * 60 * 1000;
    let secDiff = secondsFromEpoch / 1000;
    let dayDiff = Math.round(Math.abs(secondsFromEpoch / oneDay))
    let weekDiff = Math.ceil(dayDiff / 7)

    if (dayDiff < 0 || dayDiff > 365) return 'a long time ago'
    if (dayDiff === 0 && secDiff < 60) return "just now";
    else if (dayDiff === 0 && secDiff < 120) return "1 minute ago";
    else if (dayDiff === 0 && secDiff < 3600) return `${Math.floor(secDiff / 60)} minutes ago`;
    else if (dayDiff === 0 && secDiff < 7200) return "1 hour ago";
    else if (dayDiff === 0 && secDiff < 86400) return `${Math.floor(secDiff / 3600)} hours ago`;
    else if (dayDiff === 1) return "yesterday";
    else if (dayDiff < 7) return `${dayDiff} days ago`
    else if (dayDiff === 7) return 'a week ago'
    else if (weekDiff === 2) return `last week`
    else if (weekDiff > 1 && weekDiff <= 4) return `${weekDiff} weeks ago`
    else if (weekDiff === 4) return 'a month ago'

    return null
}

function getMoment(timestamp, defaultTo = 'toDateString') {
    if (!timestamp) return 'a long time ago'

    return getMomentFromDayDifference(Date.now() - Date.parse(timestamp)) || new Date(timestamp)[defaultTo]()
}

const rxStatus = Object.preventExtensions({
    default: 0,
    loading: 1,
    loadingMore: 4,
    success: 2,
    empty: 3,
    error: 5,
})

const chatEvents = Object.preventExtensions({
    dispatch: (event, detail = {}) => dispatch(`chat:${event}`, detail),
    on: (event, callback) => on(`chat:${event}`, callback),
    off: (event, callback) => off(`chat:${event}`, callback),
})

function toChatObject(chat) {
    chat.lastAvailableOn = chat['lastAvailableOn'];
    chat.lastRepliedOn = chat['lastRepliedOn'];
    
    let x = Math.abs(new Date(chat.lastAvailableOn) - new Date()) / (60 * 60 * 1000);

    return ({
        ...chat,
        actualTime: chat.lastAvailableOn ? new Date(chat.lastAvailableOn).toLocaleTimeString() : '',
        lastAvailableTime: window.getMoment(chat.lastAvailableOn),
        lastRepliedTime: window.getMoment(chat.lastRepliedOn, 'toLocaleTimeString'),
        timeToExpire: Math.round(24 - x)
    });
}

function toChatMessage({messageType, platform, fromName, fromPhone, isClosed, messageText, sentTimeStamp, toName, toPhone, status, messageUrl}) {
    const isToday = (someDate) => {
        const today = new Date()
        return someDate.getDate() === today.getDate() &&
            someDate.getMonth() === today.getMonth() &&
            someDate.getFullYear() === today.getFullYear()
    }
    const parsedDate = new Date(sentTimeStamp)

    return {
        phone: fromPhone,
        sender: fromName || fromPhone,
        receiver: toName || toPhone,
        timestamp: parsedDate,
        moment: isToday(parsedDate) ? getMoment(sentTimeStamp) : parsedDate.toLocaleTimeString(),
        message: messageText,
        url: messageUrl || messageText,
        type: messageType,
        closed: isClosed,
        time: parsedDate.toLocaleTimeString(),
        status: status,
        platform: platform,

    };
}

function toMessageContent(chat) {
    console.log(chat)
    switch (chat.type) {
        case 'image' :
            return `<img class="img-fluid" src="${chat.url}" alt="Image"><br>${chat.message || 'Image'}`
        case 'audio' :
            return `<audio src="${chat.url}" controls><br>Audio`
        case 'video' :
            return `<video style="height: 250px; width: 30rem" controls class="img-fluid card" src="${chat.url}" ><br>${chat.message || 'Video'}`
        case 'file' :
            return `<a class="text-black-50" href="${chat.url}" >Download ${chat.url.split('.').reverse()[0].toUpperCase()} File<br>${chat.message || ''}</a>`
        default:
            chat.message = chat.message || 'Empty message'
            const htmlFormat = [
                {symbol: '*', tag: 'b'},
                {symbol: '_', tag: 'em'},
                {symbol: '~', tag: 'del'},
                {symbol: '`', tag: 'code'},
                {symbol: '\n', tag: 'br'},
            ];

            htmlFormat.forEach(({symbol, tag}) => {
                if (!chat.message) return;

                const regex = new RegExp(`\\${symbol}([^${symbol}]*)\\${symbol}`, 'gm');
                const match = chat.message.match(regex);
                if (!match) return;

                match.forEach(m => {
                    let formatted = m;
                    for (let i = 0; i < 2; i++) {
                        formatted = formatted.replace(symbol, `<${i > 0 ? '/' : ''}${tag}>`);
                    }
                    chat.message = chat.message.replace(m, formatted);
                });
            });

            return chat.message.replace('\n', '<br />')
    }
}

function toAlpine({data, methods}) {

    const status = () => ({
        __state: new Map(),
        __status: (() => rxStatus)(),
        get(state = 'def') {
            return this.__state.has(state)
                ? this.__state.get(state)
                : false;
        },
        set(status, state = 'def') {
            this.__state.set(state, status)
        },
        is(status, state = 'def') {
            return this.get(state) === status
        },
        /**
         * @param {Promise<*>} promise
         * @param {string} state
         * @returns {Promise<*>}
         */
        async subscribe(promise, state = 'def') {
            this.set(rxStatus.loading, state)

            try {
                const result = await promise
                this.set(result ? this.__status.success : this.__status.empty, state)
                return result
            } catch (e) {
                this.set(this.__status.error = e, state)
            }
        },
    })
    const _data = {}

    for (const dataKey in data) {
        _data[dataKey] = data[dataKey]
    }

    return {...data, ...methods, status: status()};
}

function setCustomAsyncInterval(asyncHandler, timeout, executeNow = false) {
    if (!window.hasOwnProperty('__timers')) window.__timers = []

    const id = Date.now();
    const context = this;
    let timer = 0;

    const actualHandler = async () => {
        await asyncHandler.call(context)

        if (__timers.includes(id)) timer = setTimeout(actualHandler, timeout)
    }

    timer = setTimeout(actualHandler, executeNow ? 0 : timeout)

    window.__timers.push(id)
    
    return {
        id,
        context,
        cancel() {
            clearTimeout(timer)
            window.__timers = window.__timers.filter(t => t !== id)
        }
    }
}

function query() {
    return new Proxy(new URLSearchParams(window.location.search), {
        get: (searchParams, prop) => searchParams.get(prop),
    });
}

function imageSpotlight() {
    on('click', function (event) {
        if (event.target && event.target.tagName === 'IMG') {
            const modal = document.querySelector('.image-spotlight')

            if (modal.classList.contains('active')) return

            const imagesContainer = modal.querySelector('.images');
            const image = modal.querySelector('.image');
            const close = () => {
                modal.classList.remove('active');
                document.removeEventListener('keydown', onKey)
            }

            image.src = event.target.src
            modal.classList.add('active')
            imagesContainer.innerHTML = Array.from(event.target.parentElement.parentElement.parentElement.parentElement.querySelectorAll('img.img-fluid')).map(e => e.outerHTML).join()

            const images = Array.from(imagesContainer.querySelectorAll('img.img-fluid'));
            const markActive = () => {
                const activeImage = images.find(e => e.src === image.src)
                images.forEach(d => d.classList.remove('active'))
                if (activeImage) {
                    activeImage.classList.add('active')
                    activeImage.scrollIntoView()
                }
            }
            const onKey = (event) => {
                const activeImage = images.findIndex(e => e.src === image.src)

                if (event.keyCode === 37 && activeImage !== 0) image.src = images[activeImage - 1].src
                if (event.keyCode === 39) image.src = images[activeImage + 1].src
                if (event.key === "Escape") close()

                markActive()
            }

            images.forEach(e => e.addEventListener('click', () => {
                image.src = e.src
                markActive()
            }))

            document.addEventListener('keydown', onKey)
            modal.querySelector('.image-close').addEventListener('click', close)

            setTimeout(() => markActive(), 600)
        }
    })
}
