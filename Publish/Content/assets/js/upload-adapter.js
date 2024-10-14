function UploadAdapterPlugin(editor) {
    editor.plugins.get('FileRepository').createUploadAdapter = (loader) => {
        return new UploadAdapter(loader);
    };
}

const upload = async (type, file) => {
    let data = new FormData();
    data.append("File", file);
    data.append("Type", type);

    let request = await fetch('/files/upload', {
        method: 'POST',
        body: data
    });

    let response = await request.json();

    return (await response).createdAt;
};

class UploadAdapter {

    constructor(loader) {
        this.loader = loader;
    }

    upload() {
        return this.reader = new Promise((resolve, reject) => {

            const _accepted = (upload) => {
                resolve({default: upload});
            };

            const _error = (err) => {
                reject(err);
            };

            this.loader.file.then(file => {
                upload("editor-files", file)
                    .then(_accepted)
                    .catch(_error)
            });
        });
    }

    abort() {
        this.reader.abort();
    }
}
