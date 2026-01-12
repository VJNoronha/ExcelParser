class ExcelParserApp {
    constructor() {
        this.apiBaseUrl = '/api';
        this.init();
    }

    init() {
        this.cacheElements();
        this.attachEventListeners();
    }

    cacheElements() {
        this.fileInput = document.getElementById('fileInput');
        this.uploadForm = document.getElementById('uploadForm');
        this.fileLabel = document.querySelector('.file-label');
        this.fileName = document.getElementById('fileName');
        this.fileInfo = document.getElementById('fileInfo');
        this.selectedFileName = document.getElementById('selectedFileName');
        this.selectedFileSize = document.getElementById('selectedFileSize');
        this.submitBtn = document.getElementById('submitBtn');
        this.btnText = document.getElementById('btnText');
        this.btnSpinner = document.getElementById('btnSpinner');
        this.statusContainer = document.getElementById('statusContainer');
    }

    attachEventListeners() {
       
        this.fileInput.addEventListener('change', (e) => this.handleFileSelect(e));

        this.fileLabel.addEventListener('dragover', (e) => this.handleDragOver(e));
        this.fileLabel.addEventListener('dragleave', (e) => this.handleDragLeave(e));
        this.fileLabel.addEventListener('drop', (e) => this.handleDrop(e));

        
        this.uploadForm.addEventListener('submit', (e) => this.handleSubmit(e));
    }

    handleFileSelect(event) {
        const file = event.target.files[0];
        if (file) {
            this.selectFile(file);
        }
    }

    handleDragOver(event) {
        event.preventDefault();
        event.stopPropagation();
        this.fileLabel.style.borderColor = 'var(--primary-color)';
        this.fileLabel.style.backgroundColor = 'var(--primary-light)';
    }

    handleDragLeave(event) {
        event.preventDefault();
        event.stopPropagation();
        this.fileLabel.style.borderColor = 'var(--border-color)';
        this.fileLabel.style.backgroundColor = 'rgba(37, 99, 235, 0.02)';
    }

    handleDrop(event) {
        event.preventDefault();
        event.stopPropagation();
        this.fileLabel.style.borderColor = 'var(--border-color)';
        this.fileLabel.style.backgroundColor = 'rgba(37, 99, 235, 0.02)';

        const files = event.dataTransfer.files;
        if (files.length > 0) {
            const file = files[0];
            if (this.isValidFile(file)) {
                this.fileInput.files = files;
                this.selectFile(file);
            } else {
                this.showStatus('Invalid file type. Please upload an Excel or CSV file.', 'error');
            }
        }
    }

    selectFile(file) {
        if (!this.isValidFile(file)) {
            this.showStatus('Invalid file type. Supported: .xlsx, .xls, .csv', 'error');
            this.fileInput.value = '';
            return;
        }

        this.selectedFileName.textContent = file.name;
        this.selectedFileSize.textContent = this.formatFileSize(file.size);
        this.fileInfo.classList.remove('hidden');
        this.fileName.textContent = file.name;
    }

    isValidFile(file) {
        const validTypes = ['.xlsx', '.xls', '.csv'];
        const fileName = file.name.toLowerCase();
        return validTypes.some(type => fileName.endsWith(type));
    }

    formatFileSize(bytes) {
        if (bytes === 0) return '0 Bytes';
        const k = 1024;
        const sizes = ['Bytes', 'KB', 'MB', 'GB'];
        const i = Math.floor(Math.log(bytes) / Math.log(k));
        return Math.round(bytes / Math.pow(k, i) * 100) / 100 + ' ' + sizes[i];
    }

    async handleSubmit(event) {
        event.preventDefault();

        const file = this.fileInput.files[0];
        if (!file) {
            this.showStatus('Please select a file', 'error');
            return;
        }

        await this.uploadFile(file);
    }

    async uploadFile(file) {
        this.setLoadingState(true);

        try {
            const formData = new FormData();
            formData.append('file', file);

            const response = await fetch(`${this.apiBaseUrl}/upload`, {
                method: 'POST',
                body: formData
            });

            if (response.ok) {
                const result = await response.text();
                this.showStatus(`✓ ${file.name} uploaded successfully!`, 'success');
                this.resetForm();
            } else if (response.status === 400) {
                this.showStatus('Invalid file or empty file selected', 'error');
            } else {
                this.showStatus(`Upload failed with status ${response.status}`, 'error');
            }
        } catch (error) {
            console.error('Upload error:', error);
            this.showStatus(`Error uploading file: ${error.message}`, 'error');
        } finally {
            this.setLoadingState(false);
        }
    }

    showStatus(message, type) {
        const statusDiv = document.createElement('div');
        statusDiv.className = `status-message ${type}`;

        const iconMap = {
            success: '✓',
            error: '✕',
            info: 'ℹ'
        };

        statusDiv.innerHTML = `
            <span class="status-icon">${iconMap[type]}</span>
            <span>${message}</span>
        `;

     
        const placeholder = this.statusContainer.querySelector('.status-placeholder');
        if (placeholder) {
            placeholder.remove();
        }

        this.statusContainer.insertBefore(statusDiv, this.statusContainer.firstChild);

       
        setTimeout(() => {
            statusDiv.style.animation = 'slideIn 0.3s ease reverse';
            setTimeout(() => statusDiv.remove(), 300);
        }, 6000);
    }

    setLoadingState(isLoading) {
        this.submitBtn.disabled = isLoading;
        if (isLoading) {
            this.btnText.classList.add('hidden');
            this.btnSpinner.classList.remove('hidden');
        } else {
            this.btnText.classList.remove('hidden');
            this.btnSpinner.classList.add('hidden');
        }
    }

    resetForm() {
        this.uploadForm.reset();
        this.fileInfo.classList.add('hidden');
        this.fileName.textContent = 'Click to select or drag and drop';
        this.fileInput.value = '';
    }
}


document.addEventListener('DOMContentLoaded', () => {
    new ExcelParserApp();
});
