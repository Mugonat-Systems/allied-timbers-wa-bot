const fileInput = document.querySelector('#file-input');
const chooseFileBtn = document.querySelector('#choose-file-btn');
const fileList = document.querySelector('#file-list');
let files = [];

// Add click event listener to the Choose File button
chooseFileBtn.addEventListener('click', function () {
    fileInput.click();
});

// Add event listener to the file input
fileInput.addEventListener('change', function () {
    const selectedFiles = fileInput.files;
    for (let i = 0; i < selectedFiles.length; i++) {
        const selectedFile = selectedFiles[i];
        if (selectedFile) {
            // Add the selected file to the files array and display its name in the file list
            files.push(selectedFile);
            const listItem = document.createElement('div');
            listItem.innerHTML = `
        <span>${selectedFile.name}</span>
        <button class="cancel-btn">Cancel</button>
      `;
            const cancelBtn = listItem.querySelector('.cancel-btn');
            cancelBtn.addEventListener('click', function () {
                // Remove the corresponding file from the files array
                const fileIndex = files.findIndex(file => file.name === selectedFile.name);
                if (fileIndex >= 0) {
                    files.splice(fileIndex, 1);
                }
                // Remove the list item from the list
                listItem.remove();
                // If no files are left, hide the upload button
                if (files.length === 0) {
                    uploadBtn.style.display = 'none';
                }
            });
            fileList.appendChild(listItem);
        }
    }
});