// ============================================================
// app.js — Navigation, search, khởi động
// ============================================================

const TAB_CONFIG = {
  dashboard: { title: '📊 Dashboard',        load: loadDashboard },
  books:     { title: '📖 Danh Sách Sách',   load: loadBooks, btnId: 'btn-add-book' },
  borrowers: { title: '👥 Thành Viên',       load: loadBorrowers, btnId: 'btn-add-borrower' },
  borrows:   { title: '🔄 Mượn / Trả Sách', load: loadBorrows, btnId: 'btn-borrow' },
  overdue:   { title: '⚠️ Sách Quá Hạn',    load: loadOverdue }
};

let currentTab = 'dashboard';
let searchTimer;

// ── Navigation ───────────────────────────────────────────────
function gotoTab(name) {
  if (!TAB_CONFIG[name]) return;
  currentTab = name;

  // Update tab panels
  document.querySelectorAll('.tab-panel').forEach(p => p.classList.remove('active'));
  document.getElementById(`tab-${name}`).classList.add('active');

  // Update nav items
  document.querySelectorAll('.nav-item').forEach(item => {
    item.classList.toggle('active', item.dataset.tab === name);
  });

  // Update topbar title
  document.getElementById('page-title').textContent = TAB_CONFIG[name].title;

  // Show/hide topbar button — always points to the relevant button inside
  document.getElementById('topbar-btn').style.display = 'none';

  // Load data
  TAB_CONFIG[name].load();

  // Clear search
  document.getElementById('global-search').value = '';
}

// ── Sidebar click events ─────────────────────────────────────
document.querySelectorAll('.nav-item[data-tab]').forEach(item => {
  item.addEventListener('click', () => gotoTab(item.dataset.tab));
});

// ── Topbar search ────────────────────────────────────────────
document.getElementById('global-search').addEventListener('input', (e) => {
  clearTimeout(searchTimer);
  const kw = e.target.value.trim();
  searchTimer = setTimeout(() => {
    if (currentTab === 'books') {
      loadBooks(1, kw);
    }
    if (currentTab === 'borrowers') {
      loadBorrowers(1, kw);
    }
  }, 300);
});

// ── Init ─────────────────────────────────────────────────────
gotoTab('dashboard');
