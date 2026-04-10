// ============================================================
// api.js — API helpers, toast, modal utilities
// ============================================================

const API = 'http://localhost:5234/api';

// ── HTTP helper ──────────────────────────────────────────────
async function apiFetch(url, opts = {}) {
  const res = await fetch(API + url, {
    headers: { 'Content-Type': 'application/json' },
    ...opts
  });
  if (!res.ok) {
    const err = await res.json().catch(() => ({ message: 'Lỗi không xác định' }));
    throw new Error(err.message || `HTTP ${res.status}`);
  }
  // 204 No Content
  if (res.status === 204) return null;
  return res.json();
}

// ── Toast ────────────────────────────────────────────────────
function toast(msg, type = 'success') {
  const icons = { success: '✅', error: '❌', info: 'ℹ️' };
  const el = document.createElement('div');
  el.className = `toast ${type}`;
  el.innerHTML = `<span>${icons[type] || '✅'}</span>${msg}`;
  document.getElementById('toast-container').appendChild(el);
  setTimeout(() => el.remove(), 3500);
}

// ── Modal ────────────────────────────────────────────────────
function openModal(id) {
  document.getElementById(id).classList.add('open');
}
function closeModal(id) {
  document.getElementById(id).classList.remove('open');
}

// Close modals on overlay click or [data-close] button
document.addEventListener('click', (e) => {
  // data-close attribute
  if (e.target.dataset.close) {
    closeModal(e.target.dataset.close);
    return;
  }
  // click outside .modal inside .overlay
  if (e.target.classList.contains('overlay')) {
    e.target.classList.remove('open');
  }
});

// ── Confirm dialog (custom modal) ───────────────────────────
let _confirmResolve = null;

function showConfirm(message) {
  document.getElementById('confirm-message').textContent = message;
  openModal('modal-confirm');
  return new Promise((resolve) => { _confirmResolve = resolve; });
}

document.getElementById('btn-confirm-ok').addEventListener('click', () => {
  closeModal('modal-confirm');
  if (_confirmResolve) { _confirmResolve(true); _confirmResolve = null; }
});

// Cancel trong modal-confirm cũng resolve false (via data-close thì chỉ close)
document.querySelector('[data-close="modal-confirm"]').addEventListener('click', () => {
  if (_confirmResolve) { _confirmResolve(false); _confirmResolve = null; }
});

// ── Utilities ────────────────────────────────────────────────
const fmtDate = (d) => d ? new Date(d).toLocaleDateString('vi-VN') : '–';

function daysDiff(d) {
  return Math.ceil((new Date() - new Date(d)) / 86400000);
}

function statusBadge(rec) {
  if (rec.isReturned) return '<span class="badge badge-green">✅ Đã Trả</span>';
  const over = new Date() > new Date(rec.dueDate);
  return over
    ? '<span class="badge badge-red">⚠️ Quá Hạn</span>'
    : '<span class="badge badge-blue">📤 Đang Mượn</span>';
}

function renderEmpty(tbodyId, msg = 'Không có dữ liệu') {
  const cols = document.querySelector(`#${tbodyId}`)?.closest('table')
    ?.querySelector('thead tr')?.children.length || 8;
  document.getElementById(tbodyId).innerHTML =
    `<tr><td colspan="${cols}"><div class="empty">
       <div class="empty-icon">📭</div><p>${msg}</p>
     </div></td></tr>`;
}

function renderLoading(tbodyId) {
  const cols = document.querySelector(`#${tbodyId}`)?.closest('table')
    ?.querySelector('thead tr')?.children.length || 8;
  document.getElementById(tbodyId).innerHTML =
    `<tr class="loading-row"><td colspan="${cols}"><div class="spinner"></div></td></tr>`;
}

function availBar(available, total) {
  const pct = total ? Math.round(available / total * 100) : 0;
  const color = pct === 0 ? 'var(--danger)' : pct < 50 ? 'var(--warn)' : 'var(--accent2)';
  return { pct, color };
}

// ── Pagination Helper ────────────────────────────────────────
function renderPagination(containerId, data, onPageClick) {
  const container = document.getElementById(containerId);
  if (!container) return;
  if (!data || data.totalPages <= 1) {
    container.innerHTML = '';
    return;
  }

  const { totalCount, totalPages, pageNumber } = data;
  
  let html = `<div class="pagination-info">Hiển thị ${data.items.length}/${totalCount} kết quả (Trang ${pageNumber}/${totalPages})</div>`;
  
  // Previous
  html += `<button class="pagination-btn" ${pageNumber <= 1 ? 'disabled' : ''} data-page="${pageNumber - 1}">‹</button>`;
  
  // Pages (show max 5)
  let start = Math.max(1, pageNumber - 2);
  let end = Math.min(totalPages, start + 4);
  if (end - start < 4) start = Math.max(1, end - 4);

  for (let i = start; i <= end; i++) {
    html += `<button class="pagination-btn ${i === pageNumber ? 'active' : ''}" data-page="${i}">${i}</button>`;
  }

  // Next
  html += `<button class="pagination-btn" ${pageNumber >= totalPages ? 'disabled' : ''} data-page="${pageNumber + 1}">›</button>`;

  container.innerHTML = html;

  // Add listeners
  container.querySelectorAll('.pagination-btn[data-page]').forEach(btn => {
    btn.addEventListener('click', () => {
      const p = parseInt(btn.dataset.page);
      if (p !== pageNumber) onPageClick(p);
    });
  });
}
