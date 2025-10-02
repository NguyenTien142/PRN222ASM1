/**
 * Import Modal Functions
 * Handles file import, export, and template functionality
 */

// Import Modal Functions
function showImportModal() {
    const importModal = new bootstrap.Modal(document.getElementById('importModal'));
    importModal.show();
    resetImportModal();
}

function resetImportModal() {
    document.getElementById('fileInput').value = '';
    document.getElementById('fileInfoSection').style.display = 'none';
    document.getElementById('progressSection').style.display = 'none';
    document.getElementById('importBtn').disabled = true;
    document.getElementById('importProgress').style.width = '0%';
    document.getElementById('progressText').textContent = 'Preparing import...';
}

function downloadTemplate() {
    // Create sample template data
    const templateData = [
        ['Name', 'Subtitle', 'Price', 'Quantity', 'Category', 'Image URL', 'Alt Text'],
        ['TESLA MODEL 3', 'Standard Range Plus', '450', '10', 'car', 'https://example.com/tesla.jpg', 'Tesla Model 3'],
        ['HONDA CB500F', 'Naked Sport Bike', '180', '5', 'bike', 'https://example.com/honda.jpg', 'Honda CB500F'],
        ['TREK FX 3', 'Hybrid Fitness Bike', '75', '15', 'bicycle', 'https://example.com/trek.jpg', 'Trek FX 3']
    ];

    // Convert to CSV
    const csvContent = templateData.map(row => row.join(',')).join('\n');
    
    // Create and download file
    const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
    const link = document.createElement('a');
    const url = URL.createObjectURL(blob);
    link.setAttribute('href', url);
    link.setAttribute('download', 'vehicle_import_template.csv');
    link.style.visibility = 'hidden';
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    
    showNotification('Template downloaded successfully!', 'success');
}

function exportData() {
    // Export current data to CSV
    const headers = ['Name', 'Subtitle', 'Price', 'Quantity', 'Category', 'Image URL', 'Alt Text'];
    const csvData = [headers];
    
    sampleVehicleData.forEach(vehicle => {
        csvData.push([
            vehicle.name,
            vehicle.subtitle,
            vehicle.price,
            vehicle.quantity,
            vehicle.category,
            vehicle.image,
            vehicle.alt
        ]);
    });
    
    const csvContent = csvData.map(row => row.join(',')).join('\n');
    const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
    const link = document.createElement('a');
    const url = URL.createObjectURL(blob);
    link.setAttribute('href', url);
    link.setAttribute('download', `vehicle_export_${new Date().toISOString().split('T')[0]}.csv`);
    link.style.visibility = 'hidden';
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    
    showNotification('Data exported successfully!', 'success');
}

function showAddModal() {
    // Show modal to add new vehicle
    showNotification('Add New Vehicle feature coming soon!', 'info');
}

// File handling utilities
function formatFileSize(bytes) {
    if (bytes === 0) return '0 Bytes';
    const k = 1024;
    const sizes = ['Bytes', 'KB', 'MB', 'GB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i];
}

function handleFileSelect(file) {
    // Validate file
    const allowedTypes = ['.csv', '.xlsx', '.xls'];
    const fileExtension = '.' + file.name.split('.').pop().toLowerCase();
    
    if (!allowedTypes.includes(fileExtension)) {
        showNotification('Please select a valid file format (CSV or Excel)', 'error');
        return;
    }

    if (file.size > 10 * 1024 * 1024) { // 10MB limit
        showNotification('File size must be less than 10MB', 'error');
        return;
    }

    // Display file info
    document.getElementById('fileName').textContent = file.name;
    document.getElementById('fileSize').textContent = formatFileSize(file.size);
    document.getElementById('fileInfoSection').style.display = 'block';
    document.getElementById('importBtn').disabled = false;

    // Store file for import
    window.selectedFile = file;
}

function clearSelectedFile() {
    document.getElementById('fileInput').value = '';
    document.getElementById('fileInfoSection').style.display = 'none';
    document.getElementById('importBtn').disabled = true;
    window.selectedFile = null;
}

function startImport() {
    if (!window.selectedFile) {
        showNotification('Please select a file first', 'error');
        return;
    }

    const progressSection = document.getElementById('progressSection');
    const progressBar = document.getElementById('importProgress');
    const progressText = document.getElementById('progressText');
    const importBtn = document.getElementById('importBtn');

    // Show progress
    progressSection.style.display = 'block';
    importBtn.disabled = true;
    
    // Simulate import process
    let progress = 0;
    const progressInterval = setInterval(() => {
        progress += Math.random() * 15;
        if (progress > 100) progress = 100;
        
        progressBar.style.width = progress + '%';
        
        if (progress < 30) {
            progressText.textContent = 'Reading file...';
        } else if (progress < 60) {
            progressText.textContent = 'Validating data...';
        } else if (progress < 90) {
            progressText.textContent = 'Importing records...';
        } else {
            progressText.textContent = 'Finalizing import...';
        }
        
        if (progress >= 100) {
            clearInterval(progressInterval);
            setTimeout(() => {
                // Hide modal and show success
                const importModal = bootstrap.Modal.getInstance(document.getElementById('importModal'));
                importModal.hide();
                showNotification('Import completed successfully! 15 records imported.', 'success');
                
                // Refresh data
                if (typeof filterByCategory === 'function' && typeof currentCategory !== 'undefined') {
                    filterByCategory(currentCategory);
                }
            }, 500);
        }
    }, 200);
}

// Search functionality
function performSearch() {
    const searchInput = document.getElementById('searchInput');
    if (searchInput && window.vehicleManager) {
        const searchTerm = searchInput.value;
        window.vehicleManager.searchVehicles(searchTerm);
    }
}

// Reset filters
function resetFilters() {
    const searchInput = document.getElementById('searchInput');
    if (searchInput) {
        searchInput.value = '';
    }
    if (window.vehicleManager) {
        window.vehicleManager.filterByCategory('all');
    }
}

// Initialize file upload functionality
function initializeFileUpload() {
    const uploadArea = document.getElementById('uploadArea');
    const fileInput = document.getElementById('fileInput');
    const fileInfoSection = document.getElementById('fileInfoSection');
    const importBtn = document.getElementById('importBtn');

    if (!uploadArea || !fileInput) return;

    // Drag and drop functionality
    uploadArea.addEventListener('dragover', function(e) {
        e.preventDefault();
        uploadArea.classList.add('dragover');
    });

    uploadArea.addEventListener('dragleave', function(e) {
        e.preventDefault();
        uploadArea.classList.remove('dragover');
    });

    uploadArea.addEventListener('drop', function(e) {
        e.preventDefault();
        uploadArea.classList.remove('dragover');
        const files = e.dataTransfer.files;
        if (files.length > 0) {
            handleFileSelect(files[0]);
        }
    });

    uploadArea.addEventListener('click', function() {
        fileInput.click();
    });

    fileInput.addEventListener('change', function(e) {
        if (e.target.files.length > 0) {
            handleFileSelect(e.target.files[0]);
        }
    });
}

// Initialize search functionality
function initializeSearch() {
    const searchInput = document.getElementById('searchInput');
    if (searchInput) {
        searchInput.addEventListener('keypress', function(e) {
            if (e.key === 'Enter') {
                performSearch();
            }
        });
    }
}

// Main initialization function
function initializeImportModal() {
    console.log('🔧 Import Modal initialized');
    initializeFileUpload();
    initializeSearch();
}

// Initialize when DOM is loaded
document.addEventListener('DOMContentLoaded', function() {
    initializeImportModal();
});

// Export functions for global use
window.importModal = {
    showImportModal,
    resetImportModal,
    downloadTemplate,
    exportData,
    showAddModal,
    clearSelectedFile,
    startImport,
    performSearch,
    resetFilters
};

// Make functions globally accessible
window.showImportModal = showImportModal;
window.downloadTemplate = downloadTemplate;
window.exportData = exportData;
window.showAddModal = showAddModal;
window.clearSelectedFile = clearSelectedFile;
window.startImport = startImport;
window.performSearch = performSearch;
window.resetFilters = resetFilters;