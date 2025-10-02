// Sample data with categories - Updated to include Bicycle, Bike, Car
const sampleVehicleData = [
    // Cars
    {
        id: 1,
        name: "MERCEDES-BENZ EQA",
        subtitle: "Automatisch - Elektrisch tweedehands lease",
        price: 589,
        quantity: 25,
        image: "https://via.placeholder.com/300x200/2c3e50/ffffff?text=Mercedes+EQA",
        alt: "Mercedes EQA",
        category: "car"
    },
    {
        id: 2,
        name: "BMW SERIE 3 TOURING",
        subtitle: "320 e M Sport",
        price: 629,
        quantity: 18,
        image: "https://via.placeholder.com/300x200/34495e/ffffff?text=BMW+Serie+3",
        alt: "BMW Serie 3",
        category: "car"
    },
    {
        id: 3,
        name: "TESLA MODEL Y",
        subtitle: "AWD Long Range",
        price: 649,
        quantity: 12,
        image: "https://via.placeholder.com/300x200/e74c3c/ffffff?text=Tesla+Model+Y",
        alt: "Tesla Model Y",
        category: "car"
    },
    {
        id: 4,
        name: "AUDI Q5 SPORTBACK",
        subtitle: "40 TDI quattro S tronic",
        price: 729,
        quantity: 8,
        image: "https://via.placeholder.com/300x200/27ae60/ffffff?text=Audi+Q5",
        alt: "Audi Q5",
        category: "car"
    },
    {
        id: 5,
        name: "VOLVO XC90",
        subtitle: "T8 Twin Engine AWD Inscription",
        price: 945,
        quantity: 6,
        image: "https://via.placeholder.com/300x200/8e44ad/ffffff?text=Volvo+XC90",
        alt: "Volvo XC90",
        category: "car"
    },
    {
        id: 6,
        name: "PORSCHE TAYCAN",
        subtitle: "4S Sport Turismo",
        price: 1329,
        quantity: 3,
        image: "https://via.placeholder.com/300x200/f39c12/ffffff?text=Porsche+Taycan",
        alt: "Porsche Taycan",
        category: "car"
    },
    
    // Bikes (Motorcycles)
    {
        id: 7,
        name: "HARLEY DAVIDSON SPORTSTER",
        subtitle: "Iron 883 - Classic Cruiser",
        price: 289,
        quantity: 14,
        image: "https://via.placeholder.com/300x200/16a085/ffffff?text=Harley+Davidson",
        alt: "Harley Davidson Sportster",
        category: "bike"
    },
    {
        id: 8,
        name: "YAMAHA MT-07",
        subtitle: "689cc Naked Sport",
        price: 195,
        quantity: 22,
        image: "https://via.placeholder.com/300x200/2980b9/ffffff?text=Yamaha+MT07",
        alt: "Yamaha MT-07",
        category: "bike"
    },
    {
        id: 9,
        name: "KAWASAKI NINJA 650",
        subtitle: "Sport Touring - ABS",
        price: 225,
        quantity: 16,
        image: "https://via.placeholder.com/300x200/059669/ffffff?text=Kawasaki+Ninja",
        alt: "Kawasaki Ninja 650",
        category: "bike"
    },
    {
        id: 10,
        name: "HONDA CB650R",
        subtitle: "Neo Sports Cafe",
        price: 210,
        quantity: 19,
        image: "https://via.placeholder.com/300x200/dc2626/ffffff?text=Honda+CB650R",
        alt: "Honda CB650R",
        category: "bike"
    },
    {
        id: 11,
        name: "BMW R1250GS",
        subtitle: "Adventure Touring",
        price: 450,
        quantity: 8,
        image: "https://via.placeholder.com/300x200/7c3aed/ffffff?text=BMW+R1250GS",
        alt: "BMW R1250GS",
        category: "bike"
    },
    {
        id: 12,
        name: "DUCATI MONSTER 821",
        subtitle: "Naked Sport - Traction Control",
        price: 315,
        quantity: 12,
        image: "https://via.placeholder.com/300x200/ef4444/ffffff?text=Ducati+Monster",
        alt: "Ducati Monster 821",
        category: "bike"
    },
    
    // Bicycles
    {
        id: 13,
        name: "TREK DOMANE SL 7",
        subtitle: "Carbon Road Bike - 105 Di2",
        price: 89,
        quantity: 35,
        image: "https://via.placeholder.com/300x200/10b981/ffffff?text=Trek+Domane",
        alt: "Trek Domane SL 7",
        category: "bicycle"
    },
    {
        id: 14,
        name: "SPECIALIZED TARMAC SL7",
        subtitle: "Aero Road Bike - Ultegra",
        price: 125,
        quantity: 28,
        image: "https://via.placeholder.com/300x200/f59e0b/ffffff?text=Specialized+Tarmac",
        alt: "Specialized Tarmac SL7",
        category: "bicycle"
    },
    {
        id: 15,
        name: "GIANT DEFY ADVANCED",
        subtitle: "Endurance Road - Carbon Frame",
        price: 95,
        quantity: 42,
        image: "https://via.placeholder.com/300x200/6366f1/ffffff?text=Giant+Defy",
        alt: "Giant Defy Advanced",
        category: "bicycle"
    },
    {
        id: 16,
        name: "CANNONDALE SUPERSIX EVO",
        subtitle: "Race Bike - Shimano 105",
        price: 110,
        quantity: 31,
        image: "https://via.placeholder.com/300x200/ec4899/ffffff?text=Cannondale+SuperSix",
        alt: "Cannondale SuperSix Evo",
        category: "bicycle"
    },
    {
        id: 17,
        name: "SCOTT ADDICT RC",
        subtitle: "Climbing Bike - SRAM Rival",
        price: 98,
        quantity: 24,
        image: "https://via.placeholder.com/300x200/14b8a6/ffffff?text=Scott+Addict",
        alt: "Scott Addict RC",
        category: "bicycle"
    },
    {
        id: 18,
        name: "BIANCHI OLTRE XR4",
        subtitle: "Aero Road - Celeste Color",
        price: 145,
        quantity: 18,
        image: "https://via.placeholder.com/300x200/0891b2/ffffff?text=Bianchi+Oltre",
        alt: "Bianchi Oltre XR4",
        category: "bicycle"
    },
    {
        id: 19,
        name: "PINARELLO DOGMA F12",
        subtitle: "Pro Race Bike - Dura Ace Di2",
        price: 189,
        quantity: 12,
        image: "https://via.placeholder.com/300x200/7c2d12/ffffff?text=Pinarello+Dogma",
        alt: "Pinarello Dogma F12",
        category: "bicycle"
    },
    {
        id: 20,
        name: "CERVELO R5",
        subtitle: "Lightweight Climber",
        price: 135,
        quantity: 26,
        image: "https://via.placeholder.com/300x200/1f2937/ffffff?text=Cervelo+R5",
        alt: "Cervelo R5",
        category: "bicycle"
    }
];

// Filter state
let currentCategory = 'all';
let currentPage = 1;
let itemsPerPage = 9;
let currentDataSet = sampleVehicleData;

// Category configuration
const categoryConfig = {
    all: { name: 'All Vehicles', icon: 'bi-grid-3x3-gap' },
    car: { name: 'Cars', icon: 'bi-car-front' },
    bike: { name: 'Bikes', icon: 'bi-bicycle' },
    bicycle: { name: 'Bicycles', icon: 'bi-bike' }
};

// Function to filter by category
function filterByCategory(category) {
    currentCategory = category;
    
    if (category === 'all') {
        currentDataSet = sampleVehicleData;
    } else {
        currentDataSet = sampleVehicleData.filter(vehicle => vehicle.category === category);
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
        const config = categoryConfig[currentCategory];
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

// Function to update pagination info
function updatePaginationInfo() {
    const totalItems = currentDataSet.length;
    const totalPages = getTotalPages(totalItems, itemsPerPage);
    const startItem = (currentPage - 1) * itemsPerPage + 1;
    const endItem = Math.min(currentPage * itemsPerPage, totalItems);
    
    const paginationInfo = document.querySelector('.pagination-info');
    if (paginationInfo) {
        const categoryName = categoryConfig[currentCategory].name.toLowerCase();
        paginationInfo.innerHTML = `
         <button class="btn btn-success btn-sm" onclick="showImportModal()" title="Import Data">
                        <i class="bi bi-upload me-1"></i>Import
                      </button>
        `;
    }
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
        const categoryName = categoryConfig[currentCategory].name.toLowerCase();
        vehicleListingsGrid.innerHTML = `
            <div class="no-results text-center py-5">
                <i class="${categoryConfig[currentCategory].icon}" style="font-size: 3rem; color: #6c757d;"></i>
                <h5 class="mt-3 text-muted">No ${categoryName} found</h5>
                <p class="text-muted">Try selecting a different category or adjusting your filters</p>
            </div>
        `;
        return;
    }
    
    // Generate HTML for each vehicle
    paginatedData.forEach(vehicle => {
        const vehicleCard = createVehicleCard(vehicle);
        vehicleListingsGrid.appendChild(vehicleCard);
    });
    
    console.log(`✅ Rendered ${paginatedData.length} ${categoryConfig[currentCategory].name.toLowerCase()} (Page ${currentPage})`);
}

// Function to create a single vehicle card
function createVehicleCard(vehicle) {
    const vehicleCard = document.createElement('div');
    vehicleCard.className = 'vehicle-card';
    vehicleCard.setAttribute('data-vehicle-id', vehicle.id);
    vehicleCard.setAttribute('data-category', vehicle.category);
    
    // Determine price unit based on category
    let priceUnit = 'Per maand excl. BTW';
    if (vehicle.category === 'bicycle') {
        priceUnit = 'Per week excl. BTW';
    } else if (vehicle.category === 'bike') {
        priceUnit = 'Per maand excl. BTW';
    }
    
    vehicleCard.innerHTML = `
        <div class="vehicle-image">
            <img src="${vehicle.image}" alt="${vehicle.alt}">
            <div class="vehicle-status available">Quantity: ${vehicle.quantity}</div>
        </div>
        <div class="vehicle-content">
            <h6 class="vehicle-title">${vehicle.name}</h6>
            <p class="vehicle-subtitle">${vehicle.subtitle}</p>
            <div class="vehicle-price">
                <div class="price">
                    <strong>€ ${vehicle.price.toLocaleString()}</strong>
                    <small>${priceUnit}</small>
                </div>
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

// Updated functions for vehicle management
function editVehicle(vehicleId) {
    const vehicle = sampleVehicleData.find(v => v.id === vehicleId);
    if (vehicle) {
        console.log(`🔧 Editing vehicle: ${vehicle.name} (ID: ${vehicleId})`);
        showEditModal(vehicle);
    }
}

function deleteVehicle(vehicleId) {
    const vehicle = sampleVehicleData.find(v => v.id === vehicleId);
    if (vehicle) {
        const confirmDelete = confirm(`Are you sure you want to delete ${vehicle.name}?`);
        if (confirmDelete) {
            const index = sampleVehicleData.findIndex(v => v.id === vehicleId);
            if (index > -1) {
                sampleVehicleData.splice(index, 1);
                
                // Update current dataset if it's filtered
                const currentIndex = currentDataSet.findIndex(v => v.id === vehicleId);
                if (currentIndex > -1) {
                    currentDataSet.splice(currentIndex, 1);
                }
                
                // Adjust current page if necessary
                const totalPages = getTotalPages(currentDataSet.length, itemsPerPage);
                if (currentPage > totalPages && totalPages > 0) {
                    currentPage = totalPages;
                }
                
                renderVehicleListings();
                renderPagination();
                updatePaginationInfo();
                showNotification(`Successfully deleted ${vehicle.name}`, 'success');
                console.log(`🗑️ Deleted vehicle: ${vehicle.name} (ID: ${vehicleId})`);
            }
        }
    }
}

// Search function with pagination reset
function searchVehicles(searchTerm) {
    if (!searchTerm.trim()) {
        filterByCategory(currentCategory); // Reset to current category
    } else {
        currentDataSet = sampleVehicleData.filter(vehicle => {
            const matchesSearch = vehicle.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
                                vehicle.subtitle.toLowerCase().includes(searchTerm.toLowerCase());
            const matchesCategory = currentCategory === 'all' || vehicle.category === currentCategory;
            return matchesSearch && matchesCategory;
        });
        
        currentPage = 1;
        renderVehicleListings();
        renderPagination();
        updatePaginationInfo();
    }
}

// Function to show edit modal (keep existing implementation)
function showEditModal(vehicle) {
    document.getElementById('editCarId').value = vehicle.id;
    document.getElementById('editCarName').value = vehicle.name;
    document.getElementById('editCarSubtitle').value = vehicle.subtitle;
    document.getElementById('editCarPrice').value = vehicle.price;
    document.getElementById('editCarQuantity').value = vehicle.quantity;
    document.getElementById('editCarImage').value = vehicle.image;
    document.getElementById('editCarAlt').value = vehicle.alt;
    
    const editModal = new bootstrap.Modal(document.getElementById('editCarModal'));
    editModal.show();
}

// Keep existing modal functions...
function saveEditedCar() {
    const vehicleId = parseInt(document.getElementById('editCarId').value);
    const updatedVehicle = {
        id: vehicleId,
        name: document.getElementById('editCarName').value.trim(),
        subtitle: document.getElementById('editCarSubtitle').value.trim(),
        price: parseInt(document.getElementById('editCarPrice').value),
        quantity: parseInt(document.getElementById('editCarQuantity').value),
        image: document.getElementById('editCarImage').value.trim(),
        alt: document.getElementById('editCarAlt').value.trim(),
        category: sampleVehicleData.find(v => v.id === vehicleId)?.category || 'car'
    };
    
    if (!validateVehicleData(updatedVehicle)) {
        return;
    }
    
    const index = sampleVehicleData.findIndex(v => v.id === vehicleId);
    if (index > -1) {
        sampleVehicleData[index] = updatedVehicle;
        
        const currentIndex = currentDataSet.findIndex(v => v.id === vehicleId);
        if (currentIndex > -1) {
            currentDataSet[currentIndex] = updatedVehicle;
        }
        
        renderVehicleListings();
        
        const editModal = bootstrap.Modal.getInstance(document.getElementById('editCarModal'));
        editModal.hide();
        
        showNotification(`Successfully updated ${updatedVehicle.name}`, 'success');
        console.log(`📝 Updated vehicle: ${updatedVehicle.name} (ID: ${vehicleId})`);
    }
}

function validateVehicleData(vehicle) {
    const errors = [];
    
    if (!vehicle.name || vehicle.name.length < 2) {
        errors.push('Vehicle name must be at least 2 characters long');
    }
    
    if (!vehicle.subtitle || vehicle.subtitle.length < 2) {
        errors.push('Vehicle subtitle must be at least 2 characters long');
    }
    
    if (!vehicle.price || vehicle.price < 1 || vehicle.price > 10000) {
        errors.push('Price must be between 1 and 10,000');
    }
    
    if (!vehicle.quantity || vehicle.quantity < 0 || vehicle.quantity > 999) {
        errors.push('Quantity must be between 0 and 999');
    }
    
    if (errors.length > 0) {
        showNotification(`Validation errors:\n${errors.join('\n')}`, 'error');
        return false;
    }
    
    return true;
}

function showNotification(message, type = 'info') {
    const notificationContainer = document.getElementById('notificationContainer');
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

function cancelEdit() {
    const editModal = bootstrap.Modal.getInstance(document.getElementById('editCarModal'));
    editModal.hide();
}

// Initialize when DOM is loaded
document.addEventListener('DOMContentLoaded', function() {
    console.log('🚗 Vehicle Data Manager with Categories initialized');
    currentDataSet = sampleVehicleData;
    renderVehicleListings();
    renderPagination();
    updatePaginationInfo();
    updateCategoryButtons();
    updatePageTitle();
});

// Export functions for global use
window.vehicleManager = {
    filterByCategory,
    searchVehicles,
    editVehicle,
    deleteVehicle,
    changePage,
    saveEditedCar,
    cancelEdit,
    sampleVehicleData
};

// Make functions globally accessible
window.filterByCategory = filterByCategory;
window.searchVehicles = searchVehicles;
window.changePage = changePage;
window.saveEditedCar = saveEditedCar;
window.cancelEdit = cancelEdit;
window.editVehicle = editVehicle;
window.deleteVehicle = deleteVehicle;