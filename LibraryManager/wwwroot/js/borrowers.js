// ============================================================
// borrowers.js — Tab Quản Lý Người Mượn (CRUD hoàn chỉnh)
// ============================================================

let currentBorrowersPage = 1;
let currentBorrowersKeyword = '';

// ── Load & Render ────────────────────────────────────────────
async function loadBorrowers(page = 1, keyword = '') {
  currentBorrowersPage = page;
  currentBorrowersKeyword = keyword;
  renderLoading('borrowers-table');
  try {
    const url = keyword
      ? `/borrowers/search?keyword=${encodeURIComponent(keyword)}&page=${page}&pageSize=10`
      : `/borrowers?page=${page}&pageSize=10`;
      
    const res = await apiFetch(url);
    allBorrowers = res.items; // For edit modal ref
    renderBorrowers(res);
  } catch (e) {
    toast(e.message, 'error');
    renderEmpty('borrowers-table', 'Lỗi tải dữ liệu');
  }
}

function renderBorrowers(res) {
  // res is now PagedResultDto
  const list = res.items;
  document.getElementById('borrower-count').textContent = `(${res.totalCount} thành viên)`;
  
  if (!list.length) {
    renderEmpty('borrowers-table', 'Chưa có thành viên nào');
    document.getElementById('borrowers-pagination').innerHTML = '';
    return;
  }

  document.getElementById('borrowers-table').innerHTML = list.map((b, i) => {
    const rowIndex = (res.pageNumber - 1) * res.pageSize + i + 1;
    return `
    <tr>
      <td style="color:var(--text2)">${rowIndex}</td>
      <td><b>${escHtml(b.fullName)}</b></td>
      <td>${b.email ? escHtml(b.email) : '–'}</td>
      <td>${b.phone || '–'}</td>
      <td>${fmtDate(b.membershipDate)}</td>
      <td>
        ${b.isActive
          ? '<span class="badge badge-green">Hoạt động</span>'
          : '<span class="badge badge-gray">Vô hiệu</span>'}
      </td>
      <td>
        <div class="actions">
          <button class="btn btn-ghost btn-sm" data-action="edit-borrower" data-id="${b.id}">✏️ Sửa</button>
          <button class="btn btn-danger btn-sm" data-action="delete-borrower" data-id="${b.id}" data-name="${escAttr(b.fullName)}">🗑️ Xóa</button>
        </div>
      </td>
    `;
  }).join('');

  // Render pagination buttons
  renderPagination('borrowers-pagination', res, (p) => loadBorrowers(p, currentBorrowersKeyword));
}

// ── Add / Edit Modal ─────────────────────────────────────────
function openAddBorrower() {
  document.getElementById('borrower-id').value    = '';
  document.getElementById('modal-borrower-title').textContent = '👤 Thêm Thành Viên';
  document.getElementById('borrower-name').value  = '';
  document.getElementById('borrower-email').value = '';
  document.getElementById('borrower-phone').value = '';
  document.getElementById('borrower-dob').value   = '';
  document.getElementById('borrower-addr').value  = '';
  openModal('modal-borrower');
}

function openEditBorrower(id) {
  const b = allBorrowers.find(x => x.id === id);
  if (!b) return;
  document.getElementById('borrower-id').value    = b.id;
  document.getElementById('modal-borrower-title').textContent = '✏️ Sửa Thông Tin Thành Viên';
  document.getElementById('borrower-name').value  = b.fullName;
  document.getElementById('borrower-email').value = b.email || '';
  document.getElementById('borrower-phone').value = b.phone || '';
  document.getElementById('borrower-dob').value   = b.dateOfBirth || '';
  document.getElementById('borrower-addr').value  = b.address || '';
  openModal('modal-borrower');
}

async function saveBorrower() {
  const id   = document.getElementById('borrower-id').value;
  const name = document.getElementById('borrower-name').value.trim();
  if (!name) { toast('Vui lòng nhập họ và tên', 'error'); return; }

  const payload = {
    fullName:    name,
    email:       document.getElementById('borrower-email').value.trim() || null,
    phone:       document.getElementById('borrower-phone').value.trim() || null,
    dateOfBirth: document.getElementById('borrower-dob').value || null,
    address:     document.getElementById('borrower-addr').value.trim() || null
  };

  try {
    const btn = document.getElementById('btn-save-borrower');
    btn.disabled = true;
    if (id) {
      await apiFetch(`/borrowers/${id}`, { method: 'PUT', body: JSON.stringify(payload) });
      toast('Cập nhật thành viên thành công!');
    } else {
      await apiFetch('/borrowers', { method: 'POST', body: JSON.stringify(payload) });
      toast('Thêm thành viên thành công!');
    }
    closeModal('modal-borrower');
    loadBorrowers();
  } catch (e) {
    toast(e.message, 'error');
  } finally {
    document.getElementById('btn-save-borrower').disabled = false;
  }
}

async function deleteBorrower(id, name) {
  const confirmed = await showConfirm(`Bạn có chắc muốn xóa thành viên "${name}" không?`);
  if (!confirmed) return;
  try {
    await apiFetch(`/borrowers/${id}`, { method: 'DELETE' });
    toast('Đã xóa thành viên thành công!');
    loadBorrowers();
  } catch (e) {
    toast(e.message, 'error');
  }
}

// ── Event listeners ──────────────────────────────────────────
document.getElementById('btn-add-borrower').addEventListener('click', openAddBorrower);
document.getElementById('btn-save-borrower').addEventListener('click', saveBorrower);

document.getElementById('borrowers-table').addEventListener('click', (e) => {
  const btn = e.target.closest('[data-action]');
  if (!btn) return;
  const action = btn.dataset.action;
  const id     = parseInt(btn.dataset.id);
  if (action === 'edit-borrower')   openEditBorrower(id);
  if (action === 'delete-borrower') deleteBorrower(id, btn.dataset.name);
});
