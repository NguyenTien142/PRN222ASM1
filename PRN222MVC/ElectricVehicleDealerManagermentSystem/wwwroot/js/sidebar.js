// Dropdown functionality
document.addEventListener('DOMContentLoaded', function() {
  const dropdownToggles = document.querySelectorAll('.dropdown-toggle');
  
  dropdownToggles.forEach(toggle => {
    toggle.addEventListener('click', function(e) {
      e.preventDefault();
      
      const targetId = this.getAttribute('data-toggle');
      const submenu = document.getElementById('submenu-' + targetId);
      const arrow = document.getElementById('arrow-' + targetId);
      
      if (submenu.style.display === 'none' || !submenu.style.display) {
        // Close all other submenus
        document.querySelectorAll('.submenu').forEach(menu => {
          menu.style.display = 'none';
        });
        document.querySelectorAll('.nav-arrow').forEach(arr => {
          arr.classList.remove('rotated');
          arr.classList.remove('bi-chevron-down');
          arr.classList.add('bi-chevron-right');
        });
        
        // Open this submenu
        submenu.style.display = 'block';
        arrow.classList.add('rotated');
        arrow.classList.remove('bi-chevron-right');
        arrow.classList.add('bi-chevron-down');
      } else {
        // Close this submenu
        submenu.style.display = 'none';
        arrow.classList.remove('rotated');
        arrow.classList.remove('bi-chevron-down');
        arrow.classList.add('bi-chevron-right');
      }
    });
  });
});