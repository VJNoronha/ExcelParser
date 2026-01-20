class ExcelParserApp {
    constructor() {
        this.apiBaseUrl = '/api';
        this.init();
    }

    init() {
        this.cacheElements();
        this.attachEventListeners();
        this.loadSummary(); // Load on startup
        this.refreshDashboard(); // Load dashboard on startup
        this.startAutoRefresh();
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
        this.fileLabel.style.borderColor = '#2563eb';
        this.fileLabel.style.backgroundColor = 'rgba(37, 99, 235, 0.05)';
    }

    handleDragLeave(event) {
        event.preventDefault();
        event.stopPropagation();
        this.fileLabel.style.borderColor = '#e5e7eb';
        this.fileLabel.style.backgroundColor = 'rgba(37, 99, 235, 0.02)';
    }

    handleDrop(event) {
        event.preventDefault();
        event.stopPropagation();
        this.fileLabel.style.borderColor = '#e5e7eb';
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
                const result = await response.json();
                this.showStatus(`✓ ${file.name} uploaded successfully! Processing...`, 'success');
                this.resetForm();
                
                // Reload summary and dashboard after a short delay
                setTimeout(() => {
                    this.loadSummary();
                    this.refreshDashboard();
                }, 1500);
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

    async fetchData(url) {
        try {
            const response = await fetch(url);
            if (!response.ok) {
                throw new Error(`HTTP ${response.status}`);
            }
            return await response.json();
        } catch (error) {
            console.error('Error fetching data:', error);
            return null;
        }
    }

    async loadSummary() {
        try {
            console.log('Loading summary...');
            const response = await fetch(`${this.apiBaseUrl}/records/summary`);
            
            if (!response.ok) {
                throw new Error(`HTTP ${response.status}`);
            }
            
            const data = await response.json();
            console.log('Summary data:', data);
            this.updateSummaryDisplay(data);
        } catch (error) {
            console.error('Error loading summary:', error);
        }
    }

    updateSummaryDisplay(data) {
        // Remove placeholder
        const placeholder = this.statusContainer.querySelector('.status-placeholder');
        if (placeholder) {
            placeholder.remove();
        }

        // Remove old summary if exists
        const oldSummary = this.statusContainer.querySelector('.summary-stats');
        if (oldSummary) {
            oldSummary.remove();
        }

        // Create new summary
        const summaryDiv = document.createElement('div');
        summaryDiv.className = 'summary-stats';
        summaryDiv.innerHTML = `
            <div class="stat-item success-stat">
                <span class="stat-label">Complete Records</span>
                <span class="stat-value">${data.completeRecords || 0}</span>
            </div>
            <div class="stat-item error-stat">
                <span class="stat-label">Incomplete Records</span>
                <span class="stat-value">${data.incompleteRecords || 0}</span>
            </div>
        `;

        this.statusContainer.insertBefore(summaryDiv, this.statusContainer.firstChild);
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

        this.statusContainer.insertBefore(statusDiv, this.statusContainer.firstChild);

        setTimeout(() => {
            statusDiv.style.opacity = '0';
            statusDiv.style.transition = 'opacity 0.3s';
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

    startAutoRefresh() {
        setInterval(() => {
            this.loadSummary();
            this.refreshDashboard();
        }, 5000);
    }

    async refreshDashboard() {
        const summary = await this.fetchData(`${this.apiBaseUrl}/records/summary`);
        if (summary) {
            document.getElementById("completeCount").textContent = summary.completeRecords || 0;
            document.getElementById("incompleteCount").textContent = summary.incompleteRecords || 0;
        }

        this.loadTable("complete", "completeTable");
        this.loadTable("incomplete", "incompleteTable");
    }

    async loadTable(type, elId) {
        const data = await this.fetchData(`${this.apiBaseUrl}/records/${type}`);
        const element = document.getElementById(elId);

        if (!data || !data.length) {
            element.innerHTML = `<p>No records yet. Upload a file to see ${type} records.</p>`;
            return;
        }

        let rows = data.map(r => `
            <tr>
                <td>${r.name}</td>
                <td>${r.email}</td>
                <td>${type === "complete" ? "Valid" : (r.reason || "Invalid")}</td>
            </tr>
        `).join("");

        element.innerHTML = `
            <table>
                <tr><th>Name</th><th>Email</th><th>Status</th></tr>
                ${rows}
            </table>
        `;
    }

    switchTab(tabName, event) {
        // Remove active class from all tabs
        document.querySelectorAll('.tab-btn').forEach(btn => btn.classList.remove('active'));
        document.querySelectorAll('.tab-content').forEach(content => content.classList.remove('active'));

        // Add active class to clicked tab
        event.target.classList.add('active');
        document.getElementById(tabName).classList.add('active');
    }
}

document.addEventListener('DOMContentLoaded', () => {
    window.app = new ExcelParserApp();
});

function switchTab(tabName, event) {
    window.app.switchTab(tabName, event);
}