// Dashboard Chart Script
document.addEventListener('DOMContentLoaded', function() {
  // Yearly Summary Chart
  const ctx = document.getElementById('yearlyChart').getContext('2d');
  const yearlyChart = new Chart(ctx, {
    type: 'bar',
    data: {
      labels: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun'],
      datasets: [{
        label: 'Revenue',
        data: [40, 20, 70, 35, 30, 60],
        backgroundColor: '#17a2b8',
        borderRadius: 4,
        maxBarThickness: 30
      }, {
        label: 'Profit',
        data: [20, 15, 40, 30, 25, 45],
        backgroundColor: '#6f42c1',
        borderRadius: 4,
        maxBarThickness: 30
      }]
    },
    options: {
      responsive: true,
      maintainAspectRatio: false,
      plugins: {
        legend: {
          display: false
        }
      },
      scales: {
        y: {
          beginAtZero: true,
          max: 80,
          grid: {
            color: '#f1f1f1'
          },
          ticks: {
            stepSize: 20,
            color: '#6c757d'
          }
        },
        x: {
          grid: {
            display: false
          },
          ticks: {
            color: '#6c757d'
          }
        }
      },
      layout: {
        padding: {
          top: 20
        }
      }
    }
  });
});