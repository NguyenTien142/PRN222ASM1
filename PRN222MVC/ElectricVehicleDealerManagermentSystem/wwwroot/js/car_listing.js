// Global variables
let currentCategory = 'all';
let currentPage = 1;
let itemsPerPage = 9;
let currentDataSet = [];
let allVehicleData = []; // Store all vehicle data from API

// Category configuration with mapping from API to UI
const categoryConfig = {
    all: { name: 'All Vehicles', icon: 'bi-grid-3x3-gap' },
    1: { name: 'Electric Cars', icon: 'bi-car-front', alias: 'car' },
    2: { name: 'Bikes', icon: 'bi-bicycle', alias: 'bike' },
    3: { name: 'Bicycles', icon: 'bi-bike', alias: 'bicycle' }
};

// API functions
async function fetchVehicleData() {
    try {
        console.log('🔄 Fetching vehicle data from API...');
        
        const response = await fetch('/vehicle/GetAdminVehicle');
        
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }
        
        const apiData = await response.json();
        console.log('📊 Raw API response:', apiData);
        
        // Transform API data to UI format
        allVehicleData = apiData.map(vehicle => ({
            id: vehicle.vehicleId,
            name: vehicle.model.toUpperCase(),
            subtitle: `${vehicle.version || 'Standard'} - ${vehicle.color}`,
            price: vehicle.price,
            quantity: vehicle.quantity,
            image: vehicle.image || '/images/default-vehicle.jpg',
            alt: `${vehicle.model} ${vehicle.color}`,
            category: vehicle.categoryId || 1, // Use categoryId from API
            categoryName: vehicle.categoryName,
            manufacturDate: vehicle.manufactureDate
        }));
        
        console.log('✅ Transformed vehicle data:', allVehicleData);
        
        // Update category counts
        updateCategorySelector();
        
        // Initialize with all data
        currentDataSet = allVehicleData;
        
        return allVehicleData;
    } catch (error) {
        console.error('❌ Error fetching vehicle data:', error);
        showNotification('Failed to load vehicle data. Please refresh the page.', 'error');
        
        // Fallback to empty array
        allVehicleData = [];
        currentDataSet = [];
        return [];
    }
}

// Function to update category selector with actual counts
function updateCategorySelector() {
    const categorySelect = document.getElementById('categorySelect');
    if (!categorySelect) return;
    
    // Count vehicles by category
    const categoryCounts = {};
    allVehicleData.forEach(vehicle => {
        const categoryId = vehicle.category;
        categoryCounts[categoryId] = (categoryCounts[categoryId] || 0) + 1;
    });
    
    const totalCount = allVehicleData.length;
    
    // Update selector options
    categorySelect.innerHTML = `
        <option value="all" selected>🌐 All Vehicles (${totalCount})</option>
        <option value="1">🚗 Electric Cars (${categoryCounts[1] || 0})</option>
        <option value="2">🏍️ Bikes (${categoryCounts[2] || 0})</option>
        <option value="3">🚲 Bicycles (${categoryCounts[3] || 0})</option>
    `;
}

// Function to filter by category
function filterByCategory(category) {
    currentCategory = category;
    
    if (category === 'all') {
        currentDataSet = allVehicleData;
    } else {
        // Filter by categoryId
        const categoryId = parseInt(category);
        currentDataSet = allVehicleData.filter(vehicle => vehicle.category === categoryId);
    }
    
    currentPage = 1; // Reset to first page
    renderVehicleListings();
    renderPagination();
    updatePaginationInfo();
    updateCategoryButtons();
    updatePageTitle();
    
    console.log(`🔍 Filtered by category: ${category} (${currentDataSet.length} items)`);
}

// Function to update category buttons
function updateCategoryButtons() {
    const categoryButtons = document.querySelectorAll('.category-btn');
    categoryButtons.forEach(btn => {
        const category = btn.getAttribute('data-category');
        if (category === currentCategory) {
            btn.classList.add('active');
        } else {
            btn.classList.remove('active');
        }
    });
}

// Function to update page title
function updatePageTitle() {
    const titleElement = document.querySelector('.page-title');
    if (titleElement) {
        const config = categoryConfig[currentCategory] || { name: 'Vehicles', icon: 'bi-grid-3x3-gap' };
        titleElement.innerHTML = `<i class="${config.icon} me-2"></i>${config.name}`;
    }
}

// Function to get paginated data
function getPaginatedData(data, page, itemsPerPage) {
    const startIndex = (page - 1) * itemsPerPage;
    const endIndex = startIndex + itemsPerPage;
    return data.slice(startIndex, endIndex);
}

// Function to calculate total pages
function getTotalPages(dataLength, itemsPerPage) {
    return Math.ceil(dataLength / itemsPerPage);
}

// Function to render pagination controls
function renderPagination() {
    const totalPages = getTotalPages(currentDataSet.length, itemsPerPage);
    const paginationContainer = document.querySelector('.pagination-container');
    
    if (!paginationContainer || totalPages <= 1) {
        if (paginationContainer) {
            paginationContainer.innerHTML = '';
        }
        return;
    }

    let paginationHTML = `
        <nav aria-label="Vehicle listings pagination">
            <ul class="pagination justify-content-center">
    `;

    // Previous button
    paginationHTML += `
        <li class="page-item ${currentPage === 1 ? 'disabled' : ''}">
            <a class="page-link" href="#" onclick="changePage(${currentPage - 1})" aria-label="Previous">
                <span aria-hidden="true">&laquo;</span>
            </a>
        </li>
    `;

    // Page numbers
    const maxVisiblePages = 5;
    let startPage = Math.max(1, currentPage - Math.floor(maxVisiblePages / 2));
    let endPage = Math.min(totalPages, startPage + maxVisiblePages - 1);

    if (endPage - startPage < maxVisiblePages - 1) {
        startPage = Math.max(1, endPage - maxVisiblePages + 1);
    }

    // First page and ellipsis
    if (startPage > 1) {
        paginationHTML += `
            <li class="page-item">
                <a class="page-link" href="#" onclick="changePage(1)">1</a>
            </li>
        `;
        if (startPage > 2) {
            paginationHTML += `<li class="page-item disabled"><span class="page-link">...</span></li>`;
        }
    }

    // Page numbers
    for (let i = startPage; i <= endPage; i++) {
        paginationHTML += `
            <li class="page-item ${i === currentPage ? 'active' : ''}">
                <a class="page-link" href="#" onclick="changePage(${i})">${i}</a>
            </li>
        `;
    }

    // Last page and ellipsis
    if (endPage < totalPages) {
        if (endPage < totalPages - 1) {
            paginationHTML += `<li class="page-item disabled"><span class="page-link">...</span></li>`;
        }
        paginationHTML += `
            <li class="page-item">
                <a class="page-link" href="#" onclick="changePage(${totalPages})">${totalPages}</a>
            </li>
        `;
    }

    // Next button
    paginationHTML += `
        <li class="page-item ${currentPage === totalPages ? 'disabled' : ''}">
            <a class="page-link" href="#" onclick="changePage(${currentPage + 1})" aria-label="Next">
                <span aria-hidden="true">&raquo;</span>
            </a>
        </li>
    `;

    paginationHTML += `
            </ul>
        </nav>
    `;

    paginationContainer.innerHTML = paginationHTML;
}

// Function to change page
function changePage(page) {
    const totalPages = getTotalPages(currentDataSet.length, itemsPerPage);
    
    if (page < 1 || page > totalPages || page === currentPage) {
        return;
    }
    
    currentPage = page;
    renderVehicleListings();
    renderPagination();
    updatePaginationInfo();
    
    // Scroll to top of listings
    const vehicleListingsGrid = document.querySelector('.vehicle-listings-grid');
    if (vehicleListingsGrid) {
        vehicleListingsGrid.scrollIntoView({ behavior: 'smooth', block: 'start' });
    }
}

// Function to update pagination info - CHANGED: Import -> Add Vehicle
function updatePaginationInfo() {
    const totalItems = currentDataSet.length;
    const totalPages = getTotalPages(totalItems, itemsPerPage);
    const startItem = (currentPage - 1) * itemsPerPage + 1;
    const endItem = Math.min(currentPage * itemsPerPage, totalItems);
    
    const paginationInfo = document.querySelector('.pagination-info');
    if (paginationInfo) {
        paginationInfo.innerHTML = `
            <button class="btn btn-success btn-sm" onclick="showAddVehicleModal()" title="Add New Vehicle">
                <i class="bi bi-plus-circle me-1"></i>Add Vehicle
            </button>
        `;
    }
}

// ✅ NEW: Function to show add vehicle modal
function showAddVehicleModal() {
    console.log('🆕 Opening add vehicle modal');
    try {
        // Reset form
        resetAddVehicleForm();
        
        // Show the add vehicle modal
        const addModal = new bootstrap.Modal(document.getElementById('addVehicleModal'));
        addModal.show();
        
        console.log('✅ Add vehicle modal opened successfully');
    } catch (error) {
        console.error('❌ Error opening add vehicle modal:', error);
        showNotification('Error opening add modal', 'error');
    }
}

// ✅ NEW: Function to reset add vehicle form
function resetAddVehicleForm() {
    try {
        // Reset all form fields
        const form = document.getElementById('addVehicleForm');
        if (form) {
            form.reset();
        }
        
        // Set default values
        const today = new Date().toISOString().split('T')[0];
        const elements = {
            'addVehicleModel': '',
            'addVehicleColor': '',
            'addVehiclePrice': '',
            'addVehicleQuantity': '1',
            'addVehicleCategory': '1',
            'addVehicleVersion': '',
            'addVehicleImage': '',
            'addVehicleManufactureDate': today
        };
        
        // Set values safely
        Object.entries(elements).forEach(([id, value]) => {
            const element = document.getElementById(id);
            if (element) {
                element.value = value;
            }
        });
        
        // Reset validation states
        if (form) {
            form.classList.remove('was-validated');
            const invalidInputs = form.querySelectorAll('.is-invalid');
            invalidInputs.forEach(input => input.classList.remove('is-invalid'));
        }
        
        // Hide image preview
        const imagePreviewSection = document.getElementById('imagePreviewSection');
        if (imagePreviewSection) {
            imagePreviewSection.style.display = 'none';
        }
        
        console.log('🔄 Add vehicle form reset completed');
    } catch (error) {
        console.error('❌ Error resetting form:', error);
    }
}

// ✅ NEW: Function to add new vehicle
async function addNewVehicle() {
    console.log('🆕 addNewVehicle function called');
    
    try {
        // Get form data safely
        const getElementValue = (id) => {
            const element = document.getElementById(id);
            return element ? element.value.trim() : '';
        };
        
        const vehicleData = {
            CategoryId: parseInt(getElementValue('addVehicleCategory'), 10) || 1,
            Model: getElementValue('addVehicleModel'),
            Color: getElementValue('addVehicleColor'),
            Price: parseFloat(getElementValue('addVehiclePrice')) || 0,
            Quantity: parseInt(getElementValue('addVehicleQuantity'), 10) || 0,
            Version: getElementValue('addVehicleVersion') || null,
            Image: getElementValue('addVehicleImage') || null,
            ManufactureDate: getElementValue('addVehicleManufactureDate')
        };

        console.log('📊 Vehicle data to add:', vehicleData);

        // Validate data
        if (!validateAddVehicleData(vehicleData)) {
            return;
        }

        // Show loading state
        const addButton = document.querySelector('#addVehicleModal .btn-primary');
        if (addButton) {
            addButton.innerHTML = '<i class="spinner-border spinner-border-sm me-2"></i>Adding...';
            addButton.disabled = true;
        }

        console.log('🚀 Sending POST request to /vehicle/AddVehicleWithInventory');

        // Make API call with timeout
        const controller = new AbortController();
        const timeoutId = setTimeout(() => controller.abort(), 30000);

        try {
            const response = await fetch('/vehicle/AddVehicleWithInventory', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(vehicleData),
                signal: controller.signal
            });

            clearTimeout(timeoutId);

            console.log('📡 API Response status:', response.status);

            if (response.ok) {
                const result = await response.json();
                console.log('✅ Vehicle added successfully:', result);
                showNotification(result.message || 'Vehicle added successfully!', 'success');
                
                // Close the modal
                const addModal = bootstrap.Modal.getInstance(document.getElementById('addVehicleModal'));
                if (addModal) {
                    addModal.hide();
                }
                
                // Refresh the vehicle list
                await refreshData();
                
            } else {
                let errorMessage = 'Failed to add vehicle';
                try {
                    const errorData = await response.json();
                    errorMessage = errorData.message || errorMessage;
                } catch (e) {
                    errorMessage = `HTTP ${response.status}: ${response.statusText}`;
                }
                
                console.error('❌ Add failed:', errorMessage);
                showNotification(`Error adding vehicle: ${errorMessage}`, 'error');
            }
        } catch (error) {
            clearTimeout(timeoutId);
            throw error;
        }
    } catch (error) {
        console.error('❌ Exception in addNewVehicle:', error);
        
        let message = 'An error occurred while adding the vehicle';
        if (error.name === 'AbortError') {
            message = 'Request timeout. Please try again.';
        } else {
            message = error.message;
        }
        
        showNotification(message, 'error');
    } finally {
        // Reset button state
        const addButton = document.querySelector('#addVehicleModal .btn-primary');
        if (addButton) {
            addButton.innerHTML = '<i class="bi bi-plus-circle me-1"></i>Add Vehicle';
            addButton.disabled = false;
        }
    }
}

// ✅ NEW: Function to validate add vehicle data
function validateAddVehicleData(vehicle) {
    const errors = [];
    
    if (!vehicle.Model || vehicle.Model.length < 2) {
        errors.push('Vehicle model must be at least 2 characters long');
    }
    
    if (!vehicle.Color || vehicle.Color.length < 2) {
        errors.push('Vehicle color must be at least 2 characters long');
    }
    
    if (!vehicle.Price || vehicle.Price <= 0 || vehicle.Price > 999999) {
        errors.push('Price must be between 1 and 999,999');
    }
    
    if (vehicle.Quantity < 0 || vehicle.Quantity > 9999) {
        errors.push('Quantity must be between 0 and 9999');
    }
    
    if (!vehicle.CategoryId || vehicle.CategoryId < 1) {
        errors.push('Please select a valid category');
    }
    
    if (!vehicle.ManufactureDate) {
        errors.push('Manufacture date is required');
    }
    
    if (errors.length > 0) {
        showNotification(`Validation errors:\n${errors.join('\n')}`, 'error');
        return false;
    }
    
    return true;
}

// ✅ NEW: Function to cancel add vehicle
function cancelAddVehicle() {
    const addModal = bootstrap.Modal.getInstance(document.getElementById('addVehicleModal'));
    if (addModal) {
        addModal.hide();
    }
    console.log('🚫 Add vehicle cancelled');
}

// Function to refresh data from API
async function refreshData() {
    console.log('🔄 Refreshing data...');
    
    // Show loading state
    const vehicleListingsGrid = document.querySelector('.vehicle-listings-grid');
    if (vehicleListingsGrid) {
        vehicleListingsGrid.innerHTML = `
            <div class="loading-placeholder text-center py-5">
                <div class="spinner-border text-primary" role="status">
                    <span class="visually-hidden">Loading...</span>
                </div>
                <p class="mt-3 text-muted">Refreshing vehicle data...</p>
            </div>
        `;
    }
    
    // Fetch fresh data
    await fetchVehicleData();
    
    // Re-apply current filter
    filterByCategory(currentCategory);
    
    showNotification('Vehicle data refreshed successfully!', 'success');
}

// Function to render vehicle listings with pagination
function renderVehicleListings() {
    const vehicleListingsGrid = document.querySelector('.vehicle-listings-grid');
    
    if (!vehicleListingsGrid) {
        console.error('Vehicle listings grid not found');
        return;
    }

    // Get paginated data
    const paginatedData = getPaginatedData(currentDataSet, currentPage, itemsPerPage);
    
    // Clear existing content
    vehicleListingsGrid.innerHTML = '';
    
    // Show no results if no data
    if (paginatedData.length === 0) {
        const categoryName = (categoryConfig[currentCategory]?.name || 'vehicles').toLowerCase();
        vehicleListingsGrid.innerHTML = `
            <div class="no-results text-center py-5">
                <i class="${categoryConfig[currentCategory]?.icon || 'bi-grid-3x3-gap'}" style="font-size: 3rem; color: #6c757d;"></i>
                <h5 class="mt-3 text-muted">No ${categoryName} found</h5>
                <p class="text-muted">Try selecting a different category or refreshing the data</p>
                <button class="btn btn-primary" onclick="refreshData()">
                    <i class="bi bi-arrow-clockwise me-1"></i>Refresh Data
                </button>
            </div>
        `;
        return;
    }
    
    // Generate HTML for each vehicle
    paginatedData.forEach(vehicle => {
        const vehicleCard = createVehicleCard(vehicle);
        vehicleListingsGrid.appendChild(vehicleCard);
    });
    
    console.log(`✅ Rendered ${paginatedData.length} vehicles (Page ${currentPage})`);
}

// Function to create a single vehicle card
function createVehicleCard(vehicle) {
    const vehicleCard = document.createElement('div');
    vehicleCard.className = 'vehicle-card';
    vehicleCard.setAttribute('data-vehicle-id', vehicle.id);
    vehicleCard.setAttribute('data-category', vehicle.category);
    
    // Format price
    const formattedPrice = vehicle.price.toLocaleString('en-US', {
        minimumFractionDigits: 0,
        maximumFractionDigits: 2
    });
    
    vehicleCard.innerHTML = `
        <div class="vehicle-image">
            <img src="${vehicle.image}" alt="${vehicle.alt}" onerror="handleImageError(this)">
            <div class="vehicle-status available">Quantity: ${vehicle.quantity}</div>
        </div>
        <div class="vehicle-content">
            <h6 class="vehicle-title">${vehicle.name}</h6>
            <p class="vehicle-subtitle">${vehicle.subtitle}</p>
            <div class="vehicle-price">
                <div class="price">
                    <strong>€ ${formattedPrice}</strong>
                </div>
                <small class="text-muted">${vehicle.categoryName}</small>
            </div>
        </div>
        <div class="vehicle-actions">
            <button class="action-btn edit-btn" onclick="editVehicle(${vehicle.id})">
                <i class="bi bi-pencil"></i>
                Edit
            </button>
            <button class="action-btn delete-btn" onclick="deleteVehicle(${vehicle.id})">
                <i class="bi bi-trash"></i>
                Delete
            </button>
        </div>
    `;
    
    return vehicleCard;
}

// Function to handle image loading errors
function handleImageError(img) {
    // Check if we've already tried to load the default image
    if (!img.hasAttribute('data-fallback-attempted')) {
        // Mark that we've attempted fallback
        img.setAttribute('data-fallback-attempted', 'true');
        
        // Try to load default image
        img.src = '/images/default-vehicle.jpg';
    } else {
        // If default image also failed, use a placeholder or base64 image
        img.style.display = 'none';
        
        // Create a placeholder div
        const placeholder = document.createElement('div');
        placeholder.className = 'image-placeholder';
        placeholder.style.cssText = `
            width: 100%;
            height: 200px;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            display: flex;
            align-items: center;
            justify-content: center;
            color: white;
            font-size: 14px;
            font-weight: 500;
            text-align: center;
            border-radius: 8px;
        `;
        placeholder.innerHTML = `
            <div>
                <i class="bi bi-image" style="font-size: 2rem; display: block; margin-bottom: 8px;"></i>
                <div>No Image Available</div>
            </div>
        `;
        
        // Replace the image with placeholder
        img.parentNode.replaceChild(placeholder, img);
    }
}

// Alternative: Use base64 placeholder image
function getPlaceholderImage() {
    // Small 1x1 pixel transparent image in base64
    return 'data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iMzAwIiBoZWlnaHQ9IjIwMCIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIj4KICA8cmVjdCB3aWR0aD0iMzAwIiBoZWlnaHQ9IjIwMCIgZmlsbD0iIzY2N2VlYSIvPgogIDx0ZXh0IHg9IjE1MCIgeT0iMTAwIiBmb250LWZhbWlseT0iQXJpYWwiIGZvbnQtc2l6ZT0iMTQiIGZpbGw9IndoaXRlIiB0ZXh0LWFuY2hvcj0ibWlkZGxlIiBkeT0iLjNlbSI+Tm8gSW1hZ2UgQXZhaWxhYmxlPC90ZXh0Pgo8L3N2Zz4K';
}

// Better approach: Use placeholder in the initial HTML
function createVehicleCardImproved(vehicle) {
    const vehicleCard = document.createElement('div');
    vehicleCard.className = 'vehicle-card';
    vehicleCard.setAttribute('data-vehicle-id', vehicle.id);
    vehicleCard.setAttribute('data-category', vehicle.category);
    
    // Format price
    const formattedPrice = vehicle.price.toLocaleString('en-US', {
        minimumFractionDigits: 0,
        maximumFractionDigits: 2
    });
    
    // Use placeholder if no image or invalid image
    const imageUrl = vehicle.image && vehicle.image !== 'NULL' && vehicle.image.trim() !== '' 
        ? vehicle.image 
        : getPlaceholderImage();
    
    vehicleCard.innerHTML = `
        <div class="vehicle-image">
            <img src="${imageUrl}" alt="${vehicle.alt}" onerror="handleImageError(this)">
            <div class="vehicle-status available">Quantity: ${vehicle.quantity}</div>
        </div>
        <div class="vehicle-content">
            <h6 class="vehicle-title">${vehicle.name}</h6>
            <p class="vehicle-subtitle">${vehicle.subtitle}</p>
            <div class="vehicle-price">
                <div class="price">
                    <strong>€ ${formattedPrice}</strong>
                </div>
                <small class="text-muted">${vehicle.categoryName}</small>
            </div>
        </div>
        <div class="vehicle-actions">
            <button class="action-btn edit-btn" onclick="editVehicle(${vehicle.id})">
                <i class="bi bi-pencil"></i>
                Edit
            </button>
            <button class="action-btn delete-btn" onclick="deleteVehicle(${vehicle.id})">
                <i class="bi bi-trash"></i>
                Delete
            </button>
        </div>
    `;
    
    return vehicleCard;
}

// Make handleImageError globally accessible
window.handleImageError = handleImageError;

// Updated functions for vehicle management
function editVehicle(vehicleId) {
    const vehicle = allVehicleData.find(v => v.id === vehicleId);
    if (vehicle) {
        console.log(`🔧 Editing vehicle: ${vehicle.name} (ID: ${vehicleId})`);
        showEditModal(vehicle);
    }
}

// Updated function to call the DELETE API with correct method
async function deleteVehicle(vehicleId) {
    const vehicle = allVehicleData.find(v => v.id === vehicleId);
    if (!vehicle) {
        showNotification('Vehicle not found', 'error');
        return;
    }

    // Confirm deletion
    const confirmDelete = confirm(`Are you sure you want to delete ${vehicle.name}?\n\nThis action cannot be undone.`);
    if (!confirmDelete) {
        return;
    }

    try {
        console.log(`🗑️ Deleting vehicle: ${vehicle.name} (ID: ${vehicleId})`);
        
        // Show loading notification immediately
        showNotification('Deleting vehicle...', 'info');
        
        // Set timeout for API call to prevent hanging
        const controller = new AbortController();
        const timeoutId = setTimeout(() => controller.abort(), 5000); // 5 second timeout
        
        // FIXED: Use DELETE method to match your Controller
        const response = await fetch(`/vehicle/DeleteVehicleInventory/${vehicleId}`, {
            method: 'DELETE', // Correct method
            headers: {
                'Content-Type': 'application/json'
            },
            signal: controller.signal
        });

        clearTimeout(timeoutId); // Clear timeout if request completes

        console.log('📡 Delete API Response status:', response.status);
        console.log('📡 Delete API Response ok:', response.ok);

        if (response.ok) {
            console.log('✅ Vehicle deleted successfully');
            showNotification(`Successfully deleted ${vehicle.name}`, 'success');
            
            // OPTIMIZED: Update UI immediately without waiting
            updateUIAfterDelete(vehicleId);
            
        } else {
            // Try to get error message from response
            let errorMessage = 'Failed to delete vehicle';
            try {
                const errorData = await response.json();
                errorMessage = errorData.message || errorMessage;
            } catch (e) {
                errorMessage = `HTTP ${response.status}: ${response.statusText}`;
            }
            
            console.error('❌ Delete failed:', errorMessage);
            showNotification(`Error deleting vehicle: ${errorMessage}`, 'error');
        }
    } catch (error) {
        if (error.name === 'AbortError') {
            console.error('❌ Delete request timeout');
            showNotification('Request timeout. Please try again.', 'error');
        } else {
            console.error('❌ Exception during delete:', error);
            showNotification(`Network error: ${error.message}`, 'error');
        }
    }
}

// OPTIMIZED: Separate function to update UI quickly
function updateUIAfterDelete(vehicleId) {
    // Remove vehicle from local data
    const vehicleIndex = allVehicleData.findIndex(v => v.id === vehicleId);
    if (vehicleIndex > -1) {
        allVehicleData.splice(vehicleIndex, 1);
    }
    
    // Update current dataset
    const currentIndex = currentDataSet.findIndex(v => v.id === vehicleId);
    if (currentIndex > -1) {
        currentDataSet.splice(currentIndex, 1);
    }
    
    // Use requestAnimationFrame for smoother UI updates
    requestAnimationFrame(() => {
        renderVehicleListings();
        renderPagination();
        updatePaginationInfo();
        updateCategorySelector();
    });
}

// Search function with pagination reset
function searchVehicles(searchTerm) {
    if (!searchTerm.trim()) {
        filterByCategory(currentCategory); // Reset to current category
    } else {
        currentDataSet = allVehicleData.filter(vehicle => {
            const matchesSearch = vehicle.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
                                vehicle.subtitle.toLowerCase().includes(searchTerm.toLowerCase()) ||
                                vehicle.categoryName.toLowerCase().includes(searchTerm.toLowerCase());
            const matchesCategory = currentCategory === 'all' || vehicle.category == currentCategory;
            return matchesSearch && matchesCategory;
        });
        
        currentPage = 1;
        renderVehicleListings();
        renderPagination();
        updatePaginationInfo();
    }
}

// Function to show edit modal - sử dụng modal có sẵn
function showEditModal(vehicle) {
    console.log('🔧 Showing edit modal for vehicle:', vehicle);
    
    // Kiểm tra modal có tồn tại không
    const modalElement = document.getElementById('editCarModal');
    if (!modalElement) {
        console.error('❌ Edit modal not found in DOM');
        showNotification('Edit modal not available', 'error');
        return;
    }
    
    try {
        // Populate form fields với modal có sẵn
        document.getElementById('editCarId').value = vehicle.id;
        document.getElementById('editCarName').value = vehicle.name;
        
        // Parse subtitle để lấy version và color
        const subtitleParts = vehicle.subtitle.split(' - ');
        const version = subtitleParts[0] || '';
        const color = subtitleParts[1] || vehicle.subtitle;
        
        document.getElementById('editCarSubtitle').value = `${version} - ${color}`;
        document.getElementById('editCarPrice').value = vehicle.price;
        document.getElementById('editCarQuantity').value = vehicle.quantity;
        document.getElementById('editCarImage').value = vehicle.image !== '/images/default-vehicle.jpg' ? vehicle.image : '';
        document.getElementById('editCarAlt').value = vehicle.alt || `${vehicle.name} ${color}`;
        
        // Show modal có sẵn
        const editModal = new bootstrap.Modal(modalElement);
        editModal.show();
        
        console.log('✅ Edit modal opened successfully');
    } catch (error) {
        console.error('❌ Error opening edit modal:', error);
        showNotification('Error opening edit modal', 'error');
    }
}

// Function to save edited car - cập nhật để phù hợp với API
async function saveEditedCar() {
    try {
        const vehicleId = parseInt(document.getElementById('editCarId').value);
        
        // Get form data
        const updatedVehicle = {
            vehicleId: vehicleId,
            model: document.getElementById('editCarName').value.trim(),
            subtitle: document.getElementById('editCarSubtitle').value.trim(),
            price: parseFloat(document.getElementById('editCarPrice').value),
            quantity: parseInt(document.getElementById('editCarQuantity').value),
            image: document.getElementById('editCarImage').value.trim(),
            alt: document.getElementById('editCarAlt').value.trim()
        };
        
        // Validate data
        if (!validateVehicleData(updatedVehicle)) {
            return;
        }
        
        // Show loading state
        const saveBtn = document.querySelector('#editCarModal .btn-primary');
        const originalText = saveBtn.innerHTML;
        saveBtn.innerHTML = '<i class="spinner-border spinner-border-sm me-1"></i>Saving...';
        saveBtn.disabled = true;
        
        // TODO: Call API to update vehicle
        // const response = await fetch(`/api/vehicle/update/${vehicleId}`, {
        //     method: 'PUT',
        //     headers: { 'Content-Type': 'application/json' },
        //     body: JSON.stringify(updatedVehicle)
        // });
        
        // For now, update local data
        const vehicleIndex = allVehicleData.findIndex(v => v.id === vehicleId);
        if (vehicleIndex > -1) {
            allVehicleData[vehicleIndex] = {
                ...allVehicleData[vehicleIndex],
                name: updatedVehicle.model.toUpperCase(),
                subtitle: updatedVehicle.subtitle,
                price: updatedVehicle.price,
                quantity: updatedVehicle.quantity,
                image: updatedVehicle.image || '/images/default-vehicle.jpg',
                alt: updatedVehicle.alt
            };
        }
        
        // Update current dataset and refresh display
        filterByCategory(currentCategory);
        
        // Hide modal
        const editModal = bootstrap.Modal.getInstance(document.getElementById('editCarModal'));
        editModal.hide();
        
        showNotification(`Successfully updated ${updatedVehicle.model}`, 'success');
        console.log(`✅ Updated vehicle: ${updatedVehicle.model} (ID: ${vehicleId})`);
        
    } catch (error) {
        console.error('❌ Error updating vehicle:', error);
        showNotification(`Error updating vehicle: ${error.message}`, 'error');
    } finally {
        // Reset button state
        const saveBtn = document.querySelector('#editCarModal .btn-primary');
        if (saveBtn) {
            saveBtn.innerHTML = '<i class="bi bi-check-circle me-1"></i>Save Changes';
            saveBtn.disabled = false;
        }
    }
}

// Function to validate vehicle data
function validateVehicleData(vehicle) {
    const errors = [];
    
    if (!vehicle.model || vehicle.model.length < 2) {
        errors.push('Vehicle name must be at least 2 characters long');
    }
    
    if (!vehicle.subtitle || vehicle.subtitle.length < 2) {
        errors.push('Vehicle subtitle must be at least 2 characters long');
    }
    
    if (!vehicle.price || vehicle.price <= 0 || vehicle.price > 10000) {
        errors.push('Price must be between 1 and 10,000');
    }
    
    if (vehicle.quantity < 0 || vehicle.quantity > 999) {
        errors.push('Quantity must be between 0 and 999');
    }
    
    if (errors.length > 0) {
        showNotification(`Validation errors:\n${errors.join('\n')}`, 'error');
        return false;
    }
    
    return true;
}

// Function to cancel edit
function cancelEdit() {
    const editModal = bootstrap.Modal.getInstance(document.getElementById('editCarModal'));
    if (editModal) {
        editModal.hide();
    }
    console.log('🚫 Edit cancelled');
}

// Utility functions
function showNotification(message, type = 'info') {
    const notificationContainer = document.getElementById('notificationContainer') || createNotificationContainer();
    const notification = document.createElement('div');
    notification.className = `alert alert-${type === 'error' ? 'danger' : type === 'success' ? 'success' : 'info'} alert-dismissible fade show notification-item`;
    notification.innerHTML = `
        ${message.replace(/\n/g, '<br>')}
        <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
    `;
    
    notificationContainer.appendChild(notification);
    
    setTimeout(() => {
        if (notification.parentNode) {
            notification.remove();
        }
    }, 5000);
}

function createNotificationContainer() {
    const container = document.createElement('div');
    container.id = 'notificationContainer';
    container.className = 'position-fixed top-0 end-0 p-3';
    container.style.zIndex = '9999';
    document.body.appendChild(container);
    return container;
}

// Add image preview functionality
document.addEventListener('DOMContentLoaded', function() {
    // Initialize vehicle manager
    console.log('🚗 Vehicle Data Manager initialized with API integration');
    
    // Show loading state
    const vehicleListingsGrid = document.querySelector('.vehicle-listings-grid');
    if (vehicleListingsGrid) {
        vehicleListingsGrid.innerHTML = `
            <div class="loading-placeholder text-center py-5">
                <div class="spinner-border text-primary" role="status">
                    <span class="visually-hidden">Loading...</span>
                </div>
                <p class="mt-3 text-muted">Loading vehicle data...</p>
            </div>
        `;
    }
    
    // Fetch data from API
    fetchVehicleData().then(() => {
        // Initialize display
        filterByCategory('all');
    });
    
    // Add image preview functionality
    const imageInput = document.getElementById('addVehicleImage');
    const imagePreview = document.getElementById('addVehicleImagePreview');
    const imagePreviewSection = document.getElementById('imagePreviewSection');
    
    if (imageInput) {
        imageInput.addEventListener('input', function() {
            const imageUrl = this.value.trim();
            if (imageUrl && isValidUrl(imageUrl)) {
                if (imagePreview) {
                    imagePreview.src = imageUrl;
                    imagePreview.onload = function() {
                        if (imagePreviewSection) {
                            imagePreviewSection.style.display = 'block';
                        }
                    };
                    imagePreview.onerror = function() {
                        if (imagePreviewSection) {
                            imagePreviewSection.style.display = 'none';
                        }
                    };
                }
            } else {
                if (imagePreviewSection) {
                    imagePreviewSection.style.display = 'none';
                }
            }
        });
    }
});

// Helper function to validate URL
function isValidUrl(string) {
    try {
        new URL(string);
        return true;
    } catch (_) {
        return false;
    }
}

// Export all functions for global use
window.showAddVehicleModal = showAddVehicleModal;
window.addNewVehicle = addNewVehicle;
window.cancelAddVehicle = cancelAddVehicle;
window.resetAddVehicleForm = resetAddVehicleForm;
window.validateAddVehicleData = validateAddVehicleData;
window.refreshData = refreshData;
window.filterByCategory = filterByCategory;
window.searchVehicles = searchVehicles;
window.changePage = changePage;
window.saveEditedCar = saveEditedCar;
window.cancelEdit = cancelEdit;
window.editVehicle = editVehicle;
window.deleteVehicle = deleteVehicle;
window.handleImageError = handleImageError;
window.isValidUrl = isValidUrl;

// Export functions for vehicle manager
window.vehicleManager = {
    filterByCategory,
    searchVehicles,
    editVehicle,
    deleteVehicle,
    changePage,
    refreshData,
    allVehicleData,
    showAddVehicleModal,
    addNewVehicle
};