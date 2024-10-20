document.addEventListener('alpine:init', function () {
    const userName = prompt('Whats your name?')
    const userPhone = prompt('Whats your phone number?')


    Alpine.data('inbox', function () {
        return {
            userName,
            userPhone,
            messages: [],
            message: '',
            loading: false,
            dates: [],
            init() {

            },
            getThread(date) {
            },
            load() {

            },
            send() {
                fetch('/api/whatsapp?isTesting=true', {
                    headers: new Headers({
                        'Content-Type': 'application/json'
                    }),
                    method: 'post',
                    body: JSON.stringify({
                        "app": "Health263",
                        "timestamp": Date.now(),
                        "version": 2,
                        "type": "message",
                        "payload": {
                            "id": "ABEGkYaYVSEEAhAL3SLAWwHKeKrt6s3FKB0c",
                            "source": '263' + this.userPhone,
                            "type": "text",
                            "payload": {
                                "text": this.message
                            },
                            "sender": {
                                "phone": '263' + this.userPhone,
                                "name": this.userName,
                                "country_code": "263",
                                "dial_code": "263"
                            }
                        }
                    })
                })
            }
        }
    })
})