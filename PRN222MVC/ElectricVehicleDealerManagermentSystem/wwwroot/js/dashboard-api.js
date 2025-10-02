// Dashboard API integration
class DashboardAPI {
    constructor() {
        this.baseUrl = '/Dealer';
    }

    // Utility function to format VND currency with K, M, B suffixes
    formatVND(amount) {
        if (amount === null || amount === undefined) return 'đ0';
        
        const num = parseFloat(amount);
        
        if (num >= 1000000000) { // >= 1 tỷ
            return (num / 1000000000).toFixed(1) + 'B VND';
        } else if (num >= 1000000) { // >= 1 triệu
            return (num / 1000000).toFixed(1) + 'M VND';
        } else if (num >= 100000) { // >= 100 nghìn
            return (num / 1000).toFixed(0) + 'K VND';
        } else {
            return new Intl.NumberFormat('vi-VN').format(num) + ' VND';
        }
    }

    // Utility function to format VND currency (full format)
    formatFullVND(amount) {
        if (amount === null || amount === undefined) return 'đ0';
        
        return new Intl.NumberFormat('vi-VN', {
            style: 'currency',
            currency: 'VND'
        }).format(amount);
    }

    async getDashboardStats() {
        try {
            console.log('🔄 Fetching dashboard stats from:', `${this.baseUrl}/Stats`);
            
            const response = await fetch(`${this.baseUrl}/Stats`);
            
            console.log('📡 Response status:', response.status);
            console.log('📡 Response ok:', response.ok);
            
            if (!response.ok) {
                if (response.status === 401) {
                    console.log('🔒 Unauthorized - redirecting to login');
                    window.location.href = '/Account/Login';
                    return;
                }
                const errorText = await response.text();
                console.error('❌ API Error:', response.status, errorText);
                throw new Error(`API Error: ${response.status} - ${errorText}`);
            }
            
            const data = await response.json();
            console.log('📊 Raw API response:', data);
            
            return data;
        } catch (error) {
            console.error('❌ Error fetching dashboard stats:', error);
            this.showErrorToast('Failed to load dashboard data. Please try refreshing the page.');
            return null;
        }
    }

    async getSuccessfulOrders() {
        try {
            console.log('🔄 Fetching successful orders from:', `${this.baseUrl}/GetSuccessfulOrders`);

            const response = await fetch(`${this.baseUrl}/GetSuccessfulOrders`);

            console.log('📡 Response status:', response.status);
            console.log('📡 Response ok:', response.ok);

            if (!response.ok) {
                if (response.status === 401) {
                    console.log('🔒 Unauthorized - redirecting to login');
                    window.location.href = '/Account/Login';
                    return;
                }
                const errorText = await response.text();
                console.error('❌ API Error:', response.status, errorText);
                throw new Error(`API Error: ${response.status} - ${errorText}`);
            }

            const data = await response.json();
            console.log('📊 Raw successful orders response:', data);

            return data;
        } catch (error) {
            console.error('❌ Error fetching successful orders:', error);

            // Show user-friendly error
            this.showErrorToast('Failed to load successful orders. Please try refreshing the page.');
            return null;
        }
    }

    showErrorToast(message) {
        // Create a simple error toast
        const toast = document.createElement('div');
        toast.style.cssText = `
            position: fixed;
            top: 20px;
            right: 20px;
            background: #dc3545;
            color: white;
            padding: 15px 20px;
            border-radius: 5px;
            z-index: 9999;
            font-family: Arial, sans-serif;
        `;
        toast.textContent = message;
        document.body.appendChild(toast);
        
        setTimeout(() => {
            toast.remove();
        }, 5000);
    }

    updateSuccessfulOrdersCard(data) {
        console.log('📈 Updating successful orders card with:', data);
        const countElement = document.getElementById('successful-count');
        const progressElement = document.getElementById('successful-progress');
        const percentageElement = document.getElementById('successful-percentage');
        
        if (data && countElement && progressElement && percentageElement) {
            const total = 500; // Target
            const percentage = Math.round((data.count / total) * 100);
            
            countElement.textContent = `${data.count} / ${total}`;
            progressElement.style.width = `${percentage}%`;
            percentageElement.textContent = `${percentage}% Done`;
            
            console.log(`✅ Updated successful orders: ${data.count}/${total} (${percentage}%)`);
        } else {
            console.warn('⚠️ Could not update successful orders card - missing elements or data');
        }
    }

    updatePendingOrdersCard(data) {
        console.log('📋 Updating pending orders card with:', data);
        const countElement = document.getElementById('pending-count');
        const progressElement = document.getElementById('pending-progress');
        const percentageElement = document.getElementById('pending-percentage');
        
        if (data && countElement && progressElement && percentageElement) {
            const total = 500; // Target
            const percentage = Math.round((data.count / total) * 100);
            
            countElement.textContent = `${data.count} / ${total}`;
            progressElement.style.width = `${percentage}%`;
            percentageElement.textContent = `${percentage}% Pending`;
            
            console.log(`✅ Updated pending orders: ${data.count}/${total} (${percentage}%)`);
        } else {
            console.warn('⚠️ Could not update pending orders card - missing elements or data');
        }
    }

    updateStockQuantityCard(quantity) {
        console.log('📦 Updating stock quantity card with:', quantity);
        const countElement = document.getElementById('stock-count');
        const progressElement = document.getElementById('stock-progress');
        const percentageElement = document.getElementById('stock-percentage');
        
        if (quantity !== null && countElement && progressElement && percentageElement) {
            const total = 500; // Capacity
            const percentage = Math.round((quantity / total) * 100);
            
            countElement.textContent = `${quantity} / ${total}`;
            progressElement.style.width = `${percentage}%`;
            percentageElement.textContent = `${percentage}% Stock`;
            
            console.log(`✅ Updated stock quantity: ${quantity}/${total} (${percentage}%)`);
        } else {
            console.warn('⚠️ Could not update stock quantity card - missing elements or data');
        }
    }

    updateEarningsCard(earnings) {
        console.log('💰 Updating earnings card with:', earnings);
        const earningsElement = document.getElementById('total-earnings');
        if (earningsElement && earnings !== null) {
            // ✅ Updated: Use VND formatting with K/M/B
            const formatted = this.formatVND(earnings);
            earningsElement.textContent = formatted;
            console.log(`✅ Updated earnings: ${formatted}`);
        } else {
            console.warn('⚠️ Could not update earnings card - missing element or data');
        }
    }

    updateSalesCard(totalSales) {
        console.log('📊 Updating sales card with:', totalSales);
        const salesElement = document.getElementById('total-sales');
        if (salesElement && totalSales !== null) {
            salesElement.textContent = totalSales.toLocaleString();
            console.log(`✅ Updated total sales: ${totalSales}`);
        } else {
            console.warn('⚠️ Could not update sales card - missing element or data');
        }
    }

    // Function to update the orders table
    updateOrdersTable(orders) {
        console.log('📋 Updating orders table with:', orders);
        
        const tbody = document.getElementById('recent-orders-tbody');
        
        if (!tbody) {
            console.warn('⚠️ Orders table tbody not found');
            return;
        }

        // Clear existing content
        tbody.innerHTML = '';

        // Update orders count badge
       

        if (orders.length === 0) {
            // Show empty state
            tbody.innerHTML = `
                <tr>
                    <td colspan="5" class="text-center py-4 text-muted">
                        <i class="bi bi-inbox me-2"></i>
                        No successful orders found
                    </td>
                </tr>
            `;
            return;
        }

        // Add orders to table (limit to recent 10)
        const recentOrders = orders.slice(0, 10);
        
        recentOrders.forEach(order => {
            const row = this.createOrderRow(order);
            tbody.appendChild(row);
        });

        console.log(`✅ Updated orders table with ${recentOrders.length} orders`);
    }

    createOrderRow(order) {
        const row = document.createElement('tr');
        
        // Format date
        const orderDate = new Date(order.orderDate);
        const formattedDate = orderDate.toLocaleDateString('vi-VN', {
            day: '2-digit',
            month: '2-digit',
            year: 'numeric'
        });

        // ✅ Updated: Format amount in VND with full format for table
        const formattedAmount = this.formatFullVND(order.totalAmount);

        // Status badge
        const statusClass = order.status === 'Success' ? 'bg-success' : 'bg-secondary';
        
        row.innerHTML = `
            <td class="fw-bold">#${order.orderId}</td>
            <td>
                <div class="d-flex align-items-center">
                    <i class="bi bi-person-circle me-2 text-muted"></i>
                    ${order.userName || 'Unknown'}
                </div>
            </td>
            <td class="text-muted">${formattedDate}</td>
            <td class="fw-semibold">${formattedAmount}</td>
            <td>
                <span class="badge ${statusClass} rounded-pill">
                    ${order.status}
                </span>
            </td>
        `;

        return row;
    }

    async loadDashboardData() {
        console.log('🚀 Starting dashboard data load...');
        
        // Load dashboard stats
        const stats = await this.getDashboardStats();
        
        if (stats) {
            console.log('📊 Processing dashboard stats:', stats);
            
            // Handle different response formats
            let actualStats = stats;
            if (stats.data) {
                // If wrapped in { success: true, data: {...} }
                actualStats = stats.data;
            }
            
            this.updateSuccessfulOrdersCard({
                count: actualStats.successfulOrdersCount || 0
            });

            this.updatePendingOrdersCard({
                count: actualStats.pendingOrdersCount || 0
            });

            this.updateStockQuantityCard(actualStats.stockQuantity || 0);
            this.updateEarningsCard(actualStats.totalEarnings || 0);
            this.updateSalesCard(actualStats.totalSales || 0);

            console.log('✅ Dashboard data loaded successfully');
        }

        // Load chart data AND table data
        await this.loadChartAndTableData();
    }

    async loadChartAndTableData() {
        console.log('📈 Loading chart and table data...');
        
        const ordersData = await this.getSuccessfulOrders();
        
        if (ordersData) {
            // Update chart
            if (window.updateChartWithRealData) {
                console.log('📊 Updating chart with real data');
                window.updateChartWithRealData(ordersData);
            }

            // Update table
            this.updateOrdersTable(ordersData);
        } else {
            console.warn('⚠️ No orders data available');
            
            // Show error in table
            const tbody = document.getElementById('recent-orders-tbody');
            if (tbody) {
                tbody.innerHTML = `
                    <tr>
                        <td colspan="5" class="text-center py-4 text-danger">
                            <i class="bi bi-exclamation-triangle me-2"></i>
                            Failed to load orders data
                        </td>
                    </tr>
                `;
            }
        }
    }

    // Separate function to refresh only orders
    async refreshOrdersTable() {
        console.log('🔄 Refreshing orders table...');
        
        const tbody = document.getElementById('recent-orders-tbody');
        if (tbody) {
            tbody.innerHTML = `
                <tr>
                    <td colspan="5" class="text-center py-3">
                        <div class="spinner-border spinner-border-sm text-primary" role="status">
                            <span class="visually-hidden">Loading...</span>
                        </div>
                        <span class="ms-2 text-muted">Refreshing orders...</span>
                    </td>
                </tr>
            `;
        }

        const ordersData = await this.getSuccessfulOrders();
        if (ordersData) {
            this.updateOrdersTable(ordersData);
        }
    }
}

// Make DashboardAPI globally available
window.DashboardAPI = DashboardAPI;

// Initialize when DOM is loaded
document.addEventListener('DOMContentLoaded', function() {
    console.log('📄 DOM loaded - initializing dashboard API...');
    
    const dashboardAPI = new DashboardAPI();
    window.dashboardAPI = dashboardAPI;
    
    // Load initial data
    dashboardAPI.loadDashboardData();
    
    // Add manual refresh button handler for dashboard
    const refreshBtn = document.getElementById('refresh-dashboard');
    if (refreshBtn) {
        refreshBtn.addEventListener('click', () => {
            console.log('🔄 Manual dashboard refresh triggered');
            dashboardAPI.loadDashboardData();
        });
    }

    // Add manual refresh button handler for orders table
    const refreshOrdersBtn = document.getElementById('refresh-orders');
    if (refreshOrdersBtn) {
        refreshBtn.addEventListener('click', () => {
            console.log('🔄 Manual orders refresh triggered');
            dashboardAPI.refreshOrdersTable();
        });
    }
    
    // Auto refresh every 30 seconds
    setInterval(() => {
        console.log('⏰ Auto refresh triggered');
        dashboardAPI.loadDashboardData();
    }, 30000);
});