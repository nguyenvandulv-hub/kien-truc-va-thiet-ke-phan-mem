// ============================================================
// borrows.js — Tab Mượn/Trả Sách + Tab Quá Hạn
// ============================================================

let currentBorrowsPage = 1;

// ── Tab Mượn/Trả ─────────────────────────────────────────────
async function loadBorrows(page = 1) {
  currentBorrowsPage = page;
  renderLoading('borrows-table');
  try {
    const res = await apiFetch(`/borrowrecords?page=${page}&pageSize=10`);
    const list = res.items || [];
    
    if (!list.length) { 
      renderEmpty('borrows-table', 'Chưa có lịch sử mượn sách'); 
      document.getElementById('borrows-pagination').innerHTML = '';
      return; 
    }

    document.getElementById('borrows-table').innerHTML = list.map((r, i) => {
      const rowIndex = (res.pageNumber - 1) * res.pageSize + i + 1;
      const isOver = !r.isReturned && new Date() > new Date(r.dueDate);
      return `
        <tr class="${isOver ? 'overdue-row' : ''}">
          <td style="color:var(--text2)">${rowIndex}</td>
          <td><b>${escHtml(r.bookTitle || '–')}</b></td>
          <td>${escHtml(r.borrowerName || '–')}</td>
          <td>${fmtDate(r.borrowDate)}</td>
          <td style="color:${isOver ? 'var(--danger)' : 'inherit'}">${fmtDate(r.dueDate)}</td>
          <td>${r.returnDate ? fmtDate(r.returnDate) : '–'}</td>
          <td>${statusBadge(r)}</td>
          <td>
            ${!r.isReturned
              ? `<button class="btn btn-success btn-sm" data-action="return" data-id="${r.id}">📥 Trả</button>`
              : '<span style="color:var(--text2);font-size:12px">–</span>'}
          </td>
        </tr>`;
    }).join('');

    renderPagination('borrows-pagination', res, (p) => loadBorrows(p));
  } catch (e) {
    toast(e.message, 'error');
    renderEmpty('borrows-table', 'Lỗi tải dữ liệu');
  }
}

// ── Mượn sách modal ──────────────────────────────────────────
async function openBorrowModal() {
  try {
    const [booksRes, borrowersRes] = await Promise.all([
      apiFetch('/books?pageSize=1000'),
      apiFetch('/borrowers?pageSize=1000')
    ]);

    const availBooks = (booksRes.items || []).filter(b => b.availableQuantity > 0 && b.isActive);
    const activeMembers = (borrowersRes.items || []).filter(b => b.isActive);

    document.getElementById('borrow-book').innerHTML =
      '<option value="">-- Chọn sách --</option>' +
      availBooks.map(b =>
        `<option value="${b.id}">${escHtml(b.title)} (còn ${b.availableQuantity} bản)</option>`
      ).join('');

    document.getElementById('borrow-borrower').innerHTML =
      '<option value="">-- Chọn thành viên --</option>' +
      activeMembers.map(b =>
        `<option value="${b.id}">${escHtml(b.fullName)}</option>`
      ).join('');

    // Mặc định hạn 14 ngày
    const due = new Date();
    due.setDate(due.getDate() + 14);
    document.getElementById('borrow-due').value = due.toISOString().split('T')[0];
    document.getElementById('borrow-notes').value = '';
    openModal('modal-borrow');
  } catch (e) {
    toast(e.message, 'error');
  }
}

async function submitBorrow() {
  const bookId     = parseInt(document.getElementById('borrow-book').value);
  const borrowerId = parseInt(document.getElementById('borrow-borrower').value);
  const dueDate    = document.getElementById('borrow-due').value;

  if (!bookId)     { toast('Vui lòng chọn sách', 'error'); return; }
  if (!borrowerId) { toast('Vui lòng chọn thành viên', 'error'); return; }
  if (!dueDate)    { toast('Vui lòng chọn ngày hết hạn', 'error'); return; }

  try {
    const btn = document.getElementById('btn-submit-borrow');
    btn.disabled = true;
    await apiFetch('/borrowrecords/borrow', {
      method: 'POST',
      body: JSON.stringify({
        bookId,
        borrowerId,
        dueDate: new Date(dueDate).toISOString(),
        notes: document.getElementById('borrow-notes').value.trim() || null
      })
    });
    toast('Mượn sách thành công! 📤');
    closeModal('modal-borrow');
    loadBorrows();
    loadDashboard();
  } catch (e) {
    toast(e.message, 'error');
  } finally {
    document.getElementById('btn-submit-borrow').disabled = false;
  }
}

// ── Trả sách modal ───────────────────────────────────────────
async function openReturnModal(id) {
  try {
    const rec = await apiFetch(`/borrowrecords/${id}`);
    currentReturnId = id;
    const isOver = new Date() > new Date(rec.dueDate);
    const days   = isOver ? daysDiff(rec.dueDate) : 0;

    document.getElementById('return-info').innerHTML = `
      <b>📖 Sách:</b> ${escHtml(rec.bookTitle)}<br>
      <b>👤 Người mượn:</b> ${escHtml(rec.borrowerName)}<br>
      <b>📅 Ngày mượn:</b> ${fmtDate(rec.borrowDate)}<br>
      <b>⏰ Hạn trả:</b>
        <span style="color:${isOver ? 'var(--danger)' : 'inherit'}">${fmtDate(rec.dueDate)}</span>
      ${isOver ? `<br><b style="color:var(--danger)">⚠️ Đã trễ hạn ${days} ngày!</b>` : ''}
    `;
    document.getElementById('return-notes').value = '';
    openModal('modal-return');
  } catch (e) {
    toast(e.message, 'error');
  }
}

async function submitReturn() {
  if (!currentReturnId) return;
  try {
    const btn = document.getElementById('btn-submit-return');
    btn.disabled = true;
    await apiFetch(`/borrowrecords/${currentReturnId}/return`, {
      method: 'PUT',
      body: JSON.stringify({
        notes: document.getElementById('return-notes').value.trim() || null
      })
    });
    toast('Trả sách thành công! ✅');
    closeModal('modal-return');
    loadBorrows();
    loadDashboard();
  } catch (e) {
    toast(e.message, 'error');
  } finally {
    document.getElementById('btn-submit-return').disabled = false;
  }
}

let currentReturnId = null;
let currentOverduePage = 1;

// ── Tab Quá Hạn ──────────────────────────────────────────────
async function loadOverdue(page = 1) {
  currentOverduePage = page;
  renderLoading('overdue-table');
  try {
    const res = await apiFetch(`/borrowrecords/overdue?page=${page}&pageSize=10`);
    const list = res.items || [];

    const badge = document.getElementById('overdue-badge');
    badge.textContent = res.totalCount || 0;
    badge.style.display = res.totalCount ? '' : 'none';

    if (!list.length) {
      renderEmpty('overdue-table', '🎉 Tuyệt vời! Không có sách nào quá hạn');
      document.getElementById('overdue-pagination').innerHTML = '';
      return;
    }

    document.getElementById('overdue-table').innerHTML = list.map((r, i) => {
      const rowIndex = (res.pageNumber - 1) * res.pageSize + i + 1;
      const days     = daysDiff(r.dueDate);
      const urgency  = days > 14 ? 'var(--danger)' : days > 7 ? 'var(--warn)' : 'var(--orange)';
      return `
        <tr class="overdue-row">
          <td style="color:var(--text2)">${rowIndex}</td>
          <td><b>${escHtml(r.bookTitle || '–')}</b></td>
          <td>${escHtml(r.borrowerName || '–')}</td>
          <td>${fmtDate(r.borrowDate)}</td>
          <td style="color:var(--danger)">${fmtDate(r.dueDate)}</td>
          <td>
            <span style="color:${urgency};font-weight:700;font-size:15px">+${days} ngày</span>
          </td>
          <td style="color:var(--text2);font-size:12px">${r.notes ? escHtml(r.notes) : '–'}</td>
          <td>
            <button class="btn btn-success btn-sm" data-action="return-overdue" data-id="${r.id}">
              📥 Trả Ngay
            </button>
          </td>
        </tr>`;
    }).join('');

    renderPagination('overdue-pagination', res, (p) => loadOverdue(p));
  } catch (e) {
    toast(e.message, 'error');
    renderEmpty('overdue-table', 'Lỗi tải dữ liệu');
  }
}

// ── Event listeners ──────────────────────────────────────────
document.getElementById('btn-borrow').addEventListener('click', openBorrowModal);
document.getElementById('btn-submit-borrow').addEventListener('click', submitBorrow);
document.getElementById('btn-submit-return').addEventListener('click', submitReturn);

document.getElementById('borrows-table').addEventListener('click', (e) => {
  const btn = e.target.closest('[data-action]');
  if (!btn) return;
  if (btn.dataset.action === 'return') openReturnModal(parseInt(btn.dataset.id));
});

document.getElementById('overdue-table').addEventListener('click', (e) => {
  const btn = e.target.closest('[data-action]');
  if (!btn) return;
  if (btn.dataset.action === 'return-overdue') openReturnModal(parseInt(btn.dataset.id));
});
