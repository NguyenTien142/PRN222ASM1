/**
 * Enhanced Import Modal Functions with Resource Management
 * Handles file import, export, and template functionality
 */

// Global variables for state management
let currentUploadRequest = null;
let uploadTimeout = null;
let isUploading = false;

// ✅ FIXED: Add the missing cancelCurrentUpload function
function cancelCurrentUpload() {
    console.log('🛑 Cancelling current upload...');
    
    if (currentUploadRequest) {
        console.log('🛑 Aborting current upload request');
        currentUploadRequest.abort();
        currentUploadRequest = null;
    }
    
    if (uploadTimeout) {
        clearTimeout(uploadTimeout);
        uploadTimeout = null;
    }
    
    isUploading = false;
    
    // Reset UI if needed
    const progressSection = document.getElementById('progressSection');
    const importBtn = document.getElementById('importBtn');
    
    if (progressSection) {
        progressSection.style.display = 'none';
    }
    
    if (importBtn) {
        importBtn.disabled = false;
        importBtn.innerHTML = '<i class="bi bi-upload me-1"></i>Start Import';
    }
    
    console.log('✅ Upload cancellation completed');
}

// ✅ FIXED: Separate function for file import modal only
// ✅ FIXED: Separate function for file import modal only
// ✅ FIXED: Separate function for file import modal only
// ✅ FIXED: Separate function for file import modal only
function showFileImportModal() {
    try {
        // Cancel any ongoing upload before opening modal
        cancelCurrentUpload();
        
        // Reset import modal state
        resetImportModal();
        
        // Show the import modal
        const importModal = new bootstrap.Modal(document.getElementById('importModal'));
        importModal.show();
        
        console.log('✅ File import modal opened successfully');
    } catch (error) {
        console.error('❌ Error opening file import modal:', error);
        alert('Error opening import modal');
    }
}

// ✅ NEW: Separate function for edit vehicle modal only
function showEditVehicleModal(vehicleId, model, color, price, version, image, quantity) {
    try {
        // Check if edit modal exists
        const modalElement = document.getElementById('editCarModal');
        if (!modalElement) {
            console.error('❌ Edit modal not found');
            alert('Edit modal not available');
            return;
        }
        
        // Populate the editCarModal's input fields with the vehicle data
        document.getElementById('editCarId').value = vehicleId;
        document.getElementById('editCarName').value = model;
        document.getElementById('editCarSubtitle').value = `${version} - ${color}`;
        document.getElementById('editCarPrice').value = price;
        document.getElementById('editCarQuantity').value = quantity;
        document.getElementById('editCarImage').value = image && image !== '/images/default-vehicle.jpg' ? image : '';
        document.getElementById('editCarAlt').value = `${model} ${color}`;

        // Show the editCarModal
        const editModal = new bootstrap.Modal(modalElement);
        editModal.show();
        
        console.log('✅ Edit vehicle modal opened successfully');
    } catch (error) {
        console.error('❌ Error opening edit vehicle modal:', error);
        alert('Error opening edit modal');
    }
}

// ✅ FIXED: Add the missing showImportModal function for backward compatibility
function showImportModal(vehicleId, model, color, price, version, image, quantity) {
    // If called with parameters, it's for editing vehicle
    if (arguments.length > 0 && vehicleId) {
        showEditVehicleModal(vehicleId, model, color, price, version, image, quantity);
    } else {
        // If called without parameters, it's for file import
        showFileImportModal();
    }
}

// ✅ FIXED: Correct updateVehicle function with proper field IDs
async function updateVehicle() {
    console.log('🔄 updateVehicle function called');
    
    try {
        // ✅ FIXED: Use correct field IDs for edit modal
        const vehicleData = {
            VehicleId: parseInt(document.getElementById('editCarId').value, 10),
            Model: document.getElementById('editCarName').value,
            Color: document.getElementById('editCarSubtitle').value.split(' - ')[1] || 'Black',
            Price: parseFloat(document.getElementById('editCarPrice').value),
            Version: document.getElementById('editCarSubtitle').value.split(' - ')[0] || 'Standard',
            Quantity: parseInt(document.getElementById('editCarQuantity').value, 10),
            Image: document.getElementById('editCarImage').value
        };

        console.log('📊 Vehicle data to update:', vehicleData);

        // Validate data
        if (!vehicleData.VehicleId || vehicleData.VehicleId <= 0) {
            throw new Error('Invalid vehicle ID');
        }
        if (!vehicleData.Model || vehicleData.Model.trim() === '') {
            throw new Error('Model is required');
        }
        if (isNaN(vehicleData.Price) || vehicleData.Price <= 0) {
            throw new Error('Valid price is required');
        }
        if (isNaN(vehicleData.Quantity) || vehicleData.Quantity < 0) {
            throw new Error('Valid quantity is required');
        }

        // Show loading state
        const saveButton = document.querySelector('#editCarModal .btn-primary');
        if (saveButton) {
            saveButton.innerHTML = '<i class="spinner-border spinner-border-sm me-2"></i>Updating...';
            saveButton.disabled = true;
        }

        console.log('🚀 Sending PUT request to /vehicle/UpdateVehicle');

        // Make API call with timeout
        const controller = new AbortController();
        const timeoutId = setTimeout(() => controller.abort(), 30000);

        try {
            const response = await fetch('/vehicle/UpdateVehicle', {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(vehicleData),
                signal: controller.signal
            });

            clearTimeout(timeoutId);

            console.log('📡 API Response status:', response.status);

            if (response.ok) {
                console.log('✅ Vehicle updated successfully');
                showNotification('Vehicle updated successfully!', 'success');
                
                // Close the modal
                const editModal = bootstrap.Modal.getInstance(document.getElementById('editCarModal'));
                if (editModal) {
                    editModal.hide();
                }
                
                // Refresh the vehicle list
                if (typeof refreshData === 'function') {
                    await refreshData();
                }
            } else {
                let errorMessage = 'Failed to update vehicle';
                try {
                    const errorData = await response.json();
                    errorMessage = errorData.message || errorMessage;
                } catch (e) {
                    errorMessage = `HTTP ${response.status}: ${response.statusText}`;
                }
                
                console.error('❌ Update failed:', errorMessage);
                showNotification(`Error updating vehicle: ${errorMessage}`, 'error');
            }
        } catch (error) {
            clearTimeout(timeoutId);
            throw error;
        }
    } catch (error) {
        console.error('❌ Exception in updateVehicle:', error);
        
        let message = 'An error occurred while updating the vehicle';
        if (error.name === 'AbortError') {
            message = 'Request timeout. Please try again.';
        } else {
            message = error.message;
        }
        
        showNotification(message, 'error');
    } finally {
        // Reset button state
        const saveButton = document.querySelector('#editCarModal .btn-primary');
        if (saveButton) {
            saveButton.innerHTML = '<i class="bi bi-check-circle me-1"></i>Save Changes';
            saveButton.disabled = false;
        }
    }
}

function resetImportModal() {
    // Cancel any ongoing upload
    cancelCurrentUpload();
    
    // Clear file input and reset UI
    const fileInput = document.getElementById('fileInput');
    const fileInfoSection = document.getElementById('fileInfoSection');
    const progressSection = document.getElementById('progressSection');
    const importBtn = document.getElementById('importBtn');
    const progressBar = document.getElementById('importProgress');
    const progressText = document.getElementById('progressText');
    
    if (fileInput) fileInput.value = '';
    if (fileInfoSection) fileInfoSection.style.display = 'none';
    if (progressSection) progressSection.style.display = 'none';
    if (importBtn) importBtn.disabled = true;
    if (progressBar) progressBar.style.width = '0%';
    if (progressText) progressText.textContent = 'Preparing import...';
    
    // Clear stored file
    window.selectedFile = null;
    
    console.log('🔄 Import modal reset completed');
}

function downloadTemplate() {
    // Create sample template data
    const templateData = [
        ['Category', 'Color', 'Price', 'ManufactureDate', 'Model', 'Version', 'Image', 'Quantity'],
        ['2', 'Black', '399', '10/2/2025', '3000Z', '1', 'https://example.com/tesla.jpg', '30'],
        ['2', 'Red', '399', '10/2/2025', 'Cup 50', '1', 'https://example.com/tesla.jpg', '30']
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
    
    // Clean up blob URL
    setTimeout(() => URL.revokeObjectURL(url), 100);
    
    showNotification('Template downloaded successfully!', 'success');
}

function exportData() {
    // Export current data to CSV
    const headers = ['Category', 'Color', 'Price', 'ManufactureDate', 'Model', 'Version', 'Image', 'Quantity'];
    const csvData = [headers];
    
    // Only export if sampleVehicleData exists
    if (typeof sampleVehicleData !== 'undefined' && sampleVehicleData) {
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
    }
    
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
    
    // Clean up blob URL
    setTimeout(() => URL.revokeObjectURL(url), 100);
    
    showNotification('Data exported successfully!', 'success');
}

function showAddModal() {
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
    console.log('📁 File selected:', file.name);
    
    // Cancel any ongoing upload first
    cancelCurrentUpload();
    
    // Validate file
    const allowedTypes = ['.csv', '.xlsx', '.xls'];
    const fileExtension = '.' + file.name.split('.').pop().toLowerCase();
    
    if (!allowedTypes.includes(fileExtension)) {
        showNotification('Please select a valid file format (CSV or Excel)', 'error');
        return;
    }

    // Reduced file size limit to prevent crashes
    if (file.size > 2 * 1024 * 1024) { // 2MB limit
        showNotification('File size must be less than 2MB to prevent server crashes', 'error');
        return;
    }

    // Check if file is empty
    if (file.size === 0) {
        showNotification('Cannot upload empty files', 'error');
        return;
    }

    // Display file info
    const fileName = document.getElementById('fileName');
    const fileSize = document.getElementById('fileSize');
    const fileInfoSection = document.getElementById('fileInfoSection');
    const importBtn = document.getElementById('importBtn');
    
    if (fileName) fileName.textContent = file.name;
    if (fileSize) fileSize.textContent = formatFileSize(file.size);
    if (fileInfoSection) fileInfoSection.style.display = 'block';
    if (importBtn) importBtn.disabled = false;

    // Store file for import
    window.selectedFile = file;
    
    console.log('✅ File validation passed:', {
        name: file.name,
        size: file.size,
        type: file.type
    });
}

function clearSelectedFile() {
    cancelCurrentUpload();
    
    const fileInput = document.getElementById('fileInput');
    const fileInfoSection = document.getElementById('fileInfoSection');
    const importBtn = document.getElementById('importBtn');
    
    if (fileInput) fileInput.value = '';
    if (fileInfoSection) fileInfoSection.style.display = 'none';
    if (importBtn) importBtn.disabled = true;
    
    window.selectedFile = null;
    
    console.log('🗑️ Selected file cleared');
}

// Enhanced startImport function with proper resource management
async function startImport() {
    console.log('🔄 startImport called');
    
    // Prevent multiple uploads
    if (isUploading) {
        console.log('⚠️ Upload already in progress');
        showNotification('Upload already in progress. Please wait.', 'warning');
        return;
    }
    
    if (!window.selectedFile) {
        console.error('❌ No file selected');
        showNotification('Please select a file first', 'error');
        return;
    }

    // Additional file validation before upload
    if (window.selectedFile.size > 2 * 1024 * 1024) { // 2MB limit to prevent crashes
        console.error('❌ File too large');
        showNotification('File size must be less than 2MB to prevent server crashes', 'error');
        return;
    }

    // Check for common problematic file types
    const fileName = window.selectedFile.name.toLowerCase();
    if (!fileName.endsWith('.csv') && !fileName.endsWith('.xlsx') && !fileName.endsWith('.xls')) {
        console.error('❌ Invalid file type');
        showNotification('Only CSV and Excel files are allowed', 'error');
        return;
    }

    console.log(`📊 Starting upload for file: ${window.selectedFile.name} (${formatFileSize(window.selectedFile.size)})`);

    // Set upload state
    isUploading = true;

    const progressSection = document.getElementById('progressSection');
    const progressBar = document.getElementById('importProgress');
    const progressText = document.getElementById('progressText');
    const importBtn = document.getElementById('importBtn');

    try {
        // Show progress
        if (progressSection) progressSection.style.display = 'block';
        if (importBtn) {
            importBtn.disabled = true;
            importBtn.innerHTML = '<i class="spinner-border spinner-border-sm me-2"></i>Uploading...';
        }
        if (progressBar) progressBar.style.width = '10%';
        if (progressText) progressText.textContent = 'Preparing upload...';

        // Create AbortController for cancellation
        currentUploadRequest = new AbortController();
        
        // Set shorter timeout (30 seconds) to prevent crashes
        uploadTimeout = setTimeout(() => {
            if (currentUploadRequest) {
                console.log('⏰ Upload timeout reached, aborting request');
                currentUploadRequest.abort();
            }
        }, 30000);

        // Create FormData
        const formData = new FormData();
        formData.append('file', window.selectedFile);
        
        console.log('📦 FormData created for file:', window.selectedFile.name);

        if (progressBar) progressBar.style.width = '30%';
        if (progressText) progressText.textContent = 'Uploading file...';

        console.log('🎯 Making POST request to: /vehicle/ImportVehicles');

        // Make request
        const response = await fetch('/vehicle/ImportVehicles', {
            method: 'POST',
            body: formData,
            signal: currentUploadRequest.signal
        });

        // Clear timeout since request completed
        if (uploadTimeout) {
            clearTimeout(uploadTimeout);
            uploadTimeout = null;
        }

        console.log('📡 Response received:', {
            status: response.status,
            statusText: response.statusText,
            ok: response.ok
        });

        if (progressBar) progressBar.style.width = '80%';
        if (progressText) progressText.textContent = 'Processing response...';

        if (!response.ok) {
            let errorMessage = `HTTP ${response.status}: ${response.statusText}`;
            
            try {
                const contentType = response.headers.get('content-type');
                if (contentType && contentType.includes('application/json')) {
                    const errorData = await response.json();
                    errorMessage = errorData.message || errorMessage;
                } else {
                    const errorText = await response.text();
                    if (errorText) {
                        errorMessage = errorText;
                    }
                }
            } catch (e) {
                console.warn('Could not parse error response');
            }
            
            throw new Error(errorMessage);
        }

        // Parse successful response
        const result = await response.json();
        console.log('📊 Import result:', result);

        if (progressBar) progressBar.style.width = '100%';
        if (progressText) progressText.textContent = 'Import completed!';

        // Handle successful import
        setTimeout(() => {
            const importModal = bootstrap.Modal.getInstance(document.getElementById('importModal'));
            if (importModal) {
                importModal.hide();
            }

            if (result.success) {
                showNotification(result.message, 'success');
                console.log('✅ Import successful:', result);
            } else {
                showNotification(result.message || 'Import failed', 'error');
                console.log('⚠️ Import completed with errors:', result);
            }

            // Refresh data
            if (typeof refreshData === 'function') {
                refreshData();
            }
        }, 1000);

    } catch (error) {
        console.error('❌ Import error:', error);
        
        let userMessage = 'Import failed';
        
        if (error.name === 'AbortError') {
            userMessage = 'Upload was cancelled or timed out. Please try again with a smaller file.';
            console.log('⏰ Request was aborted due to timeout or cancellation');
        } else if (error.message.includes('Failed to fetch')) {
            userMessage = 'Cannot connect to server. Please check if the application is running and try again.';
            console.log('🌐 Network connectivity issue detected');
        } else if (error.message.includes('NetworkError')) {
            userMessage = 'Network error. Please check your internet connection and try again.';
            console.log('📡 Network error detected');
        } else if (error.message.includes('timeout')) {
            userMessage = 'Request timed out. Please try again with a smaller file.';
            console.log('⏰ Timeout error detected');
        } else {
            userMessage = error.message;
            console.log(`🔍 Specific error: ${error.message}`);
        }
        
        showNotification(userMessage, 'error');
        
    } finally {
        console.log('🧹 Cleaning up resources...');
        
        // Clean up resources
        isUploading = false;
        currentUploadRequest = null;
        
        if (uploadTimeout) {
            clearTimeout(uploadTimeout);
            uploadTimeout = null;
        }
        
        // Reset UI
        if (progressSection) progressSection.style.display = 'none';
        if (importBtn) {
            importBtn.disabled = false;
            importBtn.innerHTML = '<i class="bi bi-upload me-1"></i>Start Import';
        }
        if (progressBar) progressBar.style.width = '0%';
        if (progressText) progressText.textContent = 'Ready to import...';
        
        console.log('🧹 Upload cleanup completed');
    }
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

// Enhanced file upload initialization with proper cleanup
function initializeFileUpload() {
    console.log('🔧 Initializing file upload...');
    
    const uploadArea = document.getElementById('uploadArea');
    const fileInput = document.getElementById('fileInput');

    if (!uploadArea || !fileInput) {
        console.warn('⚠️ Upload area or file input not found');
        return;
    }

    // Prevent default drag behaviors
    ['dragenter', 'dragover', 'dragleave', 'drop'].forEach(eventName => {
        uploadArea.addEventListener(eventName, preventDefaults, false);
        document.body.addEventListener(eventName, preventDefaults, false);
    });

    // Highlight drop area when item is dragged over it
    ['dragenter', 'dragover'].forEach(eventName => {
        uploadArea.addEventListener(eventName, highlight, false);
    });

    ['dragleave', 'drop'].forEach(eventName => {
        uploadArea.addEventListener(eventName, unhighlight, false);
    });

    // Handle dropped files
    uploadArea.addEventListener('drop', handleDrop, false);
    
    // Handle click to upload
    uploadArea.addEventListener('click', function() {
        if (!isUploading) {
            fileInput.click();
        }
    });

    // Handle file input change
    fileInput.addEventListener('change', function(e) {
        if (e.target.files.length > 0) {
            handleFileSelect(e.target.files[0]);
        }
    });

    console.log('✅ File upload initialized successfully');

    // Helper functions
    function preventDefaults(e) {
        e.preventDefault();
        e.stopPropagation();
    }

    function highlight() {
        uploadArea.classList.add('dragover');
    }

    function unhighlight() {
        uploadArea.classList.remove('dragover');
    }

    function handleDrop(e) {
        const dt = e.dataTransfer;
        const files = dt.files;

        if (files.length > 1) {
            showNotification('Please select only one file at a time', 'error');
            return;
        }

        if (files.length > 0) {
            handleFileSelect(files[0]);
        }
    }
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

// Clean up on page unload
window.addEventListener('beforeunload', function() {
    cancelCurrentUpload();
});

// Export functions for global use
window.importModal = {
    showFileImportModal,
    showEditVehicleModal,
    showImportModal,
    resetImportModal,
    downloadTemplate,
    exportData,
    showAddModal,
    clearSelectedFile,
    startImport,
    performSearch,
    resetFilters,
    updateVehicle,
    cancelCurrentUpload
};

// Make functions globally accessible
window.showFileImportModal = showFileImportModal;
window.showEditVehicleModal = showEditVehicleModal;
window.showImportModal = showImportModal;
window.downloadTemplate = downloadTemplate;
window.exportData = exportData;
window.showAddModal = showAddModal;
window.clearSelectedFile = clearSelectedFile;
window.startImport = startImport;
window.performSearch = performSearch;
window.resetFilters = resetFilters;
window.updateVehicle = updateVehicle;
window.cancelCurrentUpload = cancelCurrentUpload;

// Add showNotification function if it doesn't exist
if (typeof showNotification === 'undefined') {
    window.showNotification = function(message, type = 'info') {
        console.log(`[${type.toUpperCase()}] ${message}`);
        
        // Create a simple notification
        const notification = document.createElement('div');
        notification.className = `alert alert-${type === 'error' ? 'danger' : type === 'success' ? 'success' : 'info'} alert-dismissible fade show`;
        notification.style.position = 'fixed';
        notification.style.top = '20px';
        notification.style.right = '20px';
        notification.style.zIndex = '9999';
        notification.style.minWidth = '300px';
        
        notification.innerHTML = `
            ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
        `;
        
        document.body.appendChild(notification);
        
        // Auto remove after 5 seconds
        setTimeout(() => {
            if (notification.parentNode) {
                notification.parentNode.removeChild(notification);
            }
        }, 5000);
    };
}

// Add event listener for image URL input
document.addEventListener('DOMContentLoaded', function() {
    const imageInput = document.getElementById('vehicleImage');
    const imagePreview = document.getElementById('vehicleImagePreview');
    const imagePlaceholder = document.getElementById('vehicleImagePlaceholder');
    
    if (imageInput) {
        imageInput.addEventListener('input', function() {                           
            const imageUrl = this.value.trim();
            if (imageUrl) {
                if (imagePreview) {
                    imagePreview.src = imageUrl;
                    imagePreview.style.display = 'block';
                }
                if (imagePlaceholder) {
                    imagePlaceholder.style.display = 'none';
                }
            } else {
                if (imagePreview) {
                    imagePreview.style.display = 'none';
                }
                if (imagePlaceholder) {
                    imagePlaceholder.style.display = 'block';
                }
            }
        });
    }
});