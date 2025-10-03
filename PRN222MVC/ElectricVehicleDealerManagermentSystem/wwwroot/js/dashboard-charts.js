// Global chart variable
let yearlyChart = null;

// Function to calculate revenue by month from real API data
function calculateMonthlyRevenueFromAPI(orders) {
    const monthlyData = {};
    
    console.log('📊 Processing orders for chart:', orders);

    orders.forEach(order => {
        const date = new Date(order.orderDate); // API field name
        const month = date.getMonth(); // 0-11
        
        if (!monthlyData[month]) {
            monthlyData[month] = {
                totalRevenue: 0,
                totalOrders: 0
            };
        }

        monthlyData[month].totalRevenue += parseFloat(order.totalAmount);
        monthlyData[month].totalOrders += 1;
    });

    return monthlyData;
}

// Function to create/update chart with real data
function updateChartWithRealData(ordersData) {
    console.log('📈 Updating chart with real orders data:', ordersData);
    
    const ctx = document.getElementById('yearlyChart');
    if (!ctx) {
        console.error('❌ Canvas element not found');
        return;
    }

    // Destroy existing chart if any
    if (yearlyChart) {
        yearlyChart.destroy();
    }

    // Calculate monthly revenue from API data
    const monthlyData = calculateMonthlyRevenueFromAPI(ordersData);

    // Prepare chart data for all 12 months
    const months = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
    const totalRevenueData = [];

    for (let i = 0; i < 12; i++) {
        const monthData = monthlyData[i] || { totalRevenue: 0 };
        totalRevenueData.push(Math.round(monthData.totalRevenue));
    }

    const chartData = {
        labels: months,
        datasets: [{
            label: 'Total Revenue',
            data: totalRevenueData,
            backgroundColor: 'rgba(23, 162, 184, 0.8)',
            borderColor: 'rgba(23, 162, 184, 1)',
            borderWidth: 2,
            borderRadius: 6,
            borderSkipped: false,
        }]
    };

    const config = {
        type: 'bar',
        data: chartData,
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    display: false,
                },
                tooltip: {
                    backgroundColor: 'rgba(0, 0, 0, 0.8)',
                    titleFont: {
                        size: 14,
                        weight: 'bold'
                    },
                    bodyFont: {
                        size: 12
                    },
                    cornerRadius: 8,
                    callbacks: {
                        label: function (context) {
                            return 'Revenue: $' + context.parsed.y.toLocaleString();
                        }
                    }
                }
            },
            scales: {
                x: {
                    grid: {
                        display: false
                    },
                    ticks: {
                        font: {
                            size: 12,
                            weight: '500'
                        },
                        color: '#666'
                    }
                },
                y: {
                    beginAtZero: true,
                    grid: {
                        color: 'rgba(0, 0, 0, 0.1)',
                        lineWidth: 1
                    },
                    ticks: {
                        font: {
                            size: 12
                        },
                        color: '#666',
                        callback: function (value) {
                            return '$' + (value / 1000).toFixed(1) + 'k';
                        }
                    }
                }
            },
            interaction: {
                intersect: false,
                mode: 'index'
            },
            animation: {
                duration: 1000,
                easing: 'easeInOutQuart'
            }
        }
    };

    // Create the chart
    yearlyChart = new Chart(ctx.getContext('2d'), config);

    console.log('✅ Chart updated with real API data');
    console.log('📊 Monthly Data:', monthlyData);
    console.log('📊 Total Revenue Data:', totalRevenueData);
}

// Make the function globally available
window.updateChartWithRealData = updateChartWithRealData;

// Initialize chart with loading state
document.addEventListener('DOMContentLoaded', function () {
    console.log('📊 Chart script loaded');
    
    const ctx = document.getElementById('yearlyChart');
    if (ctx) {
        // Create initial loading chart
        createLoadingChart(ctx);
        
        // Wait for dashboard API to load, then update with real data
        setTimeout(() => {
            if (window.dashboardAPI) {
                console.log('🔄 Dashboard API available, loading chart data...');
                window.dashboardAPI.loadChartData();
            } else {
                console.log('⏳ Waiting for dashboard API...');
                // Retry after a short delay
                setTimeout(() => {
                    if (window.dashboardAPI) {
                        window.dashboardAPI.loadChartData();
                    }
                }, 1000);
            }
        }, 500);
    }
});

// Function to create loading chart
function createLoadingChart(ctx) {
    const loadingData = {
        labels: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
        datasets: [{
            label: 'Loading...',
            data: [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
            backgroundColor: 'rgba(200, 200, 200, 0.3)',
            borderColor: 'rgba(200, 200, 200, 0.5)',
            borderWidth: 1,
        }]
    };

    yearlyChart = new Chart(ctx.getContext('2d'), {
        type: 'bar',
        data: loadingData,
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    display: false,
                }
            }
        }
    });

    console.log('📊 Loading chart created');
}