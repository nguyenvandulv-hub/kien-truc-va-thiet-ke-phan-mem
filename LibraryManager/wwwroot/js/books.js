// ============================================================
// books.js — Tab Quản Lý Sách (CRUD hoàn chỉnh)
// ============================================================

let currentBooksPage = 1;
let currentBooksKeyword = '';

// ── Load & Render ────────────────────────────────────────────
async function loadBooks(page = 1, keyword = '') {
  currentBooksPage = page;
  currentBooksKeyword = keyword;
  renderLoading('books-table');
  try {
    const url = keyword 
      ? `/books/search?keyword=${encodeURIComponent(keyword)}&page=${page}&pageSize=10`
      : `/books?page=${page}&pageSize=10`;
      
    const res = await apiFetch(url);
    allBooks = res.items; // For edit modal ref
    renderBooks(res);
  } catch (e) {
    toast(e.message, 'error');
    renderEmpty('books-table', 'Lỗi tải dữ liệu sách');
  }
}

function renderBooks(res) {
  // res is now PagedResultDto
  const books = res.items;
  document.getElementById('book-count').textContent = `(${res.totalCount} cuốn)`;
  
  if (!books.length) {
    renderEmpty('books-table', 'Chưa có sách nào');
    document.getElementById('books-pagination').innerHTML = '';
    return;
  }

  document.getElementById('books-table').innerHTML = books.map((b, i) => {
    const rowIndex = (res.pageNumber - 1) * res.pageSize + i + 1;
    const { pct, color } = availBar(b.availableQuantity, b.quantity);
    return `
      <tr>
        <td style="color:var(--text2)">${rowIndex}</td>
        <td>
          <b>${escHtml(b.title)}</b>
          ${b.description ? `<br><span style="color:var(--text2);font-size:11px">${escHtml(b.description.slice(0, 60))}...</span>` : ''}
        </td>
        <td>${escHtml(b.author)}</td>
        <td style="color:var(--text2);font-size:12px">${b.isbn || '–'}</td>
        <td>${b.publishedYear || '–'}</td>
        <td>
          <span style="color:${color};font-weight:700">${b.availableQuantity}</span>
          <span style="color:var(--text2)">/${b.quantity}</span>
          <div class="avail-bar">
            <div class="avail-fill" style="width:${pct}%;background:${color}"></div>
          </div>
        </td>
        <td>
          ${b.isActive
            ? '<span class="badge badge-green">Hoạt động</span>'
            : '<span class="badge badge-gray">Vô hiệu</span>'}
        </td>
        <td>
          <div class="actions">
            <button class="btn btn-ghost btn-sm" data-action="edit-book" data-id="${b.id}">✏️ Sửa</button>
            <button class="btn btn-danger btn-sm" data-action="delete-book" data-id="${b.id}" data-name="${escAttr(b.title)}">🗑️ Xóa</button>
          </div>
        </td>
      </tr>`;
  }).join('');

  // Render pagination buttons
  renderPagination('books-pagination', res, (p) => loadBooks(p, currentBooksKeyword));
}

// ── Add / Edit Modal ─────────────────────────────────────────
function openAddBook() {
  document.getElementById('book-id').value    = '';
  document.getElementById('modal-book-title').textContent = '📖 Thêm Sách Mới';
  document.getElementById('book-title').value  = '';
  document.getElementById('book-author').value = '';
  document.getElementById('book-isbn').value   = '';
  document.getElementById('book-qty').value    = 1;
  document.getElementById('book-year').value   = '';
  document.getElementById('book-desc').value   = '';
  openModal('modal-book');
}

function openEditBook(id) {
  const b = allBooks.find(x => x.id === id);
  if (!b) return;
  document.getElementById('book-id').value    = b.id;
  document.getElementById('modal-book-title').textContent = '✏️ Sửa Thông Tin Sách';
  document.getElementById('book-title').value  = b.title;
  document.getElementById('book-author').value = b.author;
  document.getElementById('book-isbn').value   = b.isbn || '';
  document.getElementById('book-qty').value    = b.quantity;
  document.getElementById('book-year').value   = b.publishedYear || '';
  document.getElementById('book-desc').value   = b.description || '';
  openModal('modal-book');
}

async function saveBook() {
  const id     = document.getElementById('book-id').value;
  const title  = document.getElementById('book-title').value.trim();
  const author = document.getElementById('book-author').value.trim();
  if (!title)  { toast('Vui lòng nhập tên sách', 'error'); return; }
  if (!author) { toast('Vui lòng nhập tên tác giả', 'error'); return; }

  const payload = {
    title,
    author,
    isbn:          document.getElementById('book-isbn').value.trim() || null,
    quantity:      parseInt(document.getElementById('book-qty').value) || 1,
    publishedYear: parseInt(document.getElementById('book-year').value) || null,
    description:   document.getElementById('book-desc').value.trim() || null
  };

  try {
    const btn = document.getElementById('btn-save-book');
    btn.disabled = true;
    if (id) {
      await apiFetch(`/books/${id}`, { method: 'PUT', body: JSON.stringify(payload) });
      toast('Cập nhật sách thành công!');
    } else {
      await apiFetch('/books', { method: 'POST', body: JSON.stringify(payload) });
      toast('Thêm sách thành công!');
    }
    closeModal('modal-book');
    loadBooks();
  } catch (e) {
    toast(e.message, 'error');
  } finally {
    document.getElementById('btn-save-book').disabled = false;
  }
}

async function deleteBook(id, name) {
  const confirmed = await showConfirm(`Bạn có chắc muốn xóa sách "${name}" khỏi hệ thống không?\n(Sách sẽ bị đánh dấu vô hiệu - soft delete)`);
  if (!confirmed) return;
  try {
    await apiFetch(`/books/${id}`, { method: 'DELETE' });
    toast('Đã xóa sách thành công!');
    loadBooks();
  } catch (e) {
    toast(e.message, 'error');
  }
}

// ── Escape helpers ───────────────────────────────────────────
function escHtml(str) {
  return String(str)
    .replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;')
    .replace(/"/g, '&quot;').replace(/'/g, '&#39;');
}
function escAttr(str) {
  return String(str).replace(/"/g, '&quot;').replace(/'/g, '&#39;');
}

// ── Event listeners ──────────────────────────────────────────
document.getElementById('btn-add-book').addEventListener('click', openAddBook);
document.getElementById('btn-save-book').addEventListener('click', saveBook);

// Delegated click for edit/delete buttons in table
document.getElementById('books-table').addEventListener('click', (e) => {
  const btn = e.target.closest('[data-action]');
  if (!btn) return;
  const action = btn.dataset.action;
  const id     = parseInt(btn.dataset.id);
  if (action === 'edit-book')   openEditBook(id);
  if (action === 'delete-book') deleteBook(id, btn.dataset.name);
});
