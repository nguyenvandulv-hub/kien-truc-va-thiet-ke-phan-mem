// ============================================================
// dashboard.js — Tab Dashboard (stats + recent + low-stock)
// ============================================================

async function loadDashboard() {
  try {
    const [booksRes, borrowersRes, active, overdueRes] = await Promise.all([
      apiFetch('/books?pageSize=1000'),
      apiFetch('/borrowers?pageSize=1000'),
      apiFetch('/borrowrecords/active'),
      apiFetch('/borrowrecords/overdue?pageSize=1000')
    ]);

    const books     = booksRes.items || [];
    const borrowers = borrowersRes.items || [];
    const overdue   = overdueRes.items || [];

    // Stat cards
    document.getElementById('s-books').textContent        = booksRes.totalCount || 0;
    document.getElementById('s-books-avail').textContent  = `Còn lại: ${books.reduce((s, b) => s + (b.availableQuantity || 0), 0)} bản`;
    document.getElementById('s-borrowers').textContent    = borrowersRes.totalCount || 0;
    document.getElementById('s-active').textContent       = (active || []).length;
    document.getElementById('s-overdue').textContent      = (overdue || {}).totalCount || (overdue || []).length || 0;

    // Badge quá hạn trên sidebar
    const badge = document.getElementById('overdue-badge');
    badge.textContent = overdue.length;
    badge.style.display = overdue.length ? '' : 'none';

    // Mượn gần đây
    const allRecordsRes = await apiFetch('/borrowrecords?pageSize=8&page=1');
    const tbody = document.getElementById('db-recent');
    const recent = allRecordsRes.items || [];
    if (!recent.length) {
      renderEmpty('db-recent', 'Chưa có lịch sử mượn sách');
    } else {
      tbody.innerHTML = recent.map(r => `
        <tr>
          <td><b>${r.bookTitle || '–'}</b></td>
          <td>${r.borrowerName || '–'}</td>
          <td>${fmtDate(r.dueDate)}</td>
          <td>${statusBadge(r)}</td>
        </tr>
      `).join('');
    }

    // Sách ít sẵn nhất
    const lowBooks = [...books]
      .sort((a, b) => (a.availableQuantity / (a.quantity || 1)) - (b.availableQuantity / (b.quantity || 1)))
      .slice(0, 8);

    document.getElementById('db-books-low').innerHTML = lowBooks.map(b => {
      const { pct, color } = availBar(b.availableQuantity, b.quantity);
      return `
        <tr>
          <td><b>${b.title}</b></td>
          <td><span style="color:${color};font-weight:700">${b.availableQuantity}</span></td>
          <td>
            <span style="color:var(--text2)">${b.quantity}</span>
            <div class="avail-bar">
              <div class="avail-fill" style="width:${pct}%;background:${color}"></div>
            </div>
          </td>
        </tr>`;
    }).join('');

  } catch (e) {
    toast(e.message, 'error');
  }
}
