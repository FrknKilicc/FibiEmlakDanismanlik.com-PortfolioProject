/**
 * Fibi Emlak - İlan düzenleme "Yakın Yerler" kartı (okul, hastane, ulaşım vb. ve uzaklıkları)
 */
document.addEventListener('DOMContentLoaded', function () {
    const card = document.getElementById('nearbyCard');
    if (!card) return;

    const MAX_PLACES = 40;
    const url = `/ilanlar/${card.dataset.typeId}/${card.dataset.listingId}/yakin-yerler`;
    const rowsEl = document.getElementById('nearbyRows');
    const alertArea = document.getElementById('nearbyAlertArea');
    const addBtn = document.getElementById('nearbyAddBtn');
    const saveBtn = document.getElementById('nearbySaveBtn');

    let categories = [];
    let canEdit = false;

    function getAntiForgeryToken() {
        const tokenInput = document.querySelector('input[name="__RequestVerificationToken"]');
        return tokenInput ? tokenInput.value : '';
    }

    function showAlert(message, type) {
        if (type === 'success' && typeof window.fibiToast === 'function') {
            window.fibiToast(message, 'success');
            return;
        }
        alertArea.replaceChildren();
        const div = document.createElement('div');
        div.className = `alert alert-${type} alert-dismissible fade show small mb-2`;
        div.setAttribute('role', 'alert');
        div.textContent = message;
        const close = document.createElement('button');
        close.type = 'button';
        close.className = 'btn-close';
        close.setAttribute('data-bs-dismiss', 'alert');
        close.setAttribute('aria-label', 'Kapat');
        div.appendChild(close);
        alertArea.appendChild(div);
    }

    // Sunucu yanıtı { ok, message } JSON'u veya düz metin olabilir
    async function readResponse(res) {
        const text = await res.text();
        try {
            return JSON.parse(text);
        } catch (_) {
            return { ok: res.ok, message: text };
        }
    }

    function formatKm(value) {
        return Number(value).toLocaleString('tr-TR', { maximumFractionDigits: 2 });
    }

    function parseKm(raw) {
        const s = (raw || '').trim().replace(/\s/g, '').replace(',', '.');
        if (!/^\d+(\.\d+)?$/.test(s)) return NaN;
        return parseFloat(s);
    }

    function updateEmptyState() {
        const empty = rowsEl.querySelector('.nearby-empty');
        const hasRows = rowsEl.querySelector('.nearby-row') !== null;
        if (!hasRows && !empty) {
            const p = document.createElement('div');
            p.className = 'nearby-empty text-muted small';
            p.textContent = 'Henüz yakın yer eklenmedi.';
            rowsEl.appendChild(p);
        } else if (hasRows && empty) {
            empty.remove();
        }
    }

    function addRow(place) {
        const row = document.createElement('div');
        row.className = 'nearby-row row g-2 align-items-center';

        const catCol = document.createElement('div');
        catCol.className = 'col-md-4 col-12';
        const select = document.createElement('select');
        select.className = 'form-select form-select-sm nearby-category';
        select.setAttribute('aria-label', 'Kategori');
        categories.forEach(function (c) {
            const opt = document.createElement('option');
            opt.value = c.id;
            opt.textContent = c.name;
            select.appendChild(opt);
        });
        if (place && place.nearbyCategoryId) select.value = String(place.nearbyCategoryId);
        catCol.appendChild(select);

        const nameCol = document.createElement('div');
        nameCol.className = 'col-md-5 col-12';
        const nameInput = document.createElement('input');
        nameInput.type = 'text';
        nameInput.maxLength = 120;
        nameInput.className = 'form-control form-control-sm nearby-name';
        nameInput.placeholder = 'Yer adı (örn. Atatürk İlkokulu)';
        nameInput.setAttribute('aria-label', 'Yer adı');
        nameInput.value = place ? place.placeName : '';
        nameCol.appendChild(nameInput);

        const kmCol = document.createElement('div');
        kmCol.className = 'col-md-2 col-8';
        const group = document.createElement('div');
        group.className = 'input-group input-group-sm';
        const kmInput = document.createElement('input');
        kmInput.type = 'text';
        kmInput.inputMode = 'decimal';
        kmInput.className = 'form-control nearby-km';
        kmInput.placeholder = '0,5';
        kmInput.setAttribute('aria-label', 'Uzaklık (km)');
        kmInput.value = place ? formatKm(place.distanceKm) : '';
        const suffix = document.createElement('span');
        suffix.className = 'input-group-text';
        suffix.textContent = 'km';
        group.append(kmInput, suffix);
        kmCol.appendChild(group);

        row.append(catCol, nameCol, kmCol);

        if (canEdit) {
            const delCol = document.createElement('div');
            delCol.className = 'col-md-1 col-4 text-end';
            const delBtn = document.createElement('button');
            delBtn.type = 'button';
            delBtn.className = 'btn btn-outline-danger btn-sm';
            delBtn.setAttribute('aria-label', 'Satırı sil');
            delBtn.title = 'Satırı sil';
            const icon = document.createElement('i');
            icon.className = 'bi bi-trash';
            delBtn.appendChild(icon);
            delBtn.addEventListener('click', function () {
                row.remove();
                updateEmptyState();
            });
            delCol.appendChild(delBtn);
            row.appendChild(delCol);
        } else {
            [select, nameInput, kmInput].forEach(function (el) { el.disabled = true; });
        }

        rowsEl.appendChild(row);
        updateEmptyState();
        return row;
    }

    function load() {
        fetch(url, { headers: { 'Accept': 'application/json' } })
            .then(readResponse)
            .then(function (data) {
                rowsEl.replaceChildren();
                if (!data || !data.ok || !data.data) {
                    showAlert((data && data.message) || 'Yakın yerler yüklenemedi.', 'danger');
                    updateEmptyState();
                    return;
                }
                categories = data.data.categories || [];
                canEdit = !!data.data.canEdit;
                (data.data.places || []).forEach(addRow);
                updateEmptyState();
            })
            .catch(function () {
                rowsEl.replaceChildren();
                showAlert('Sunucu ile iletişim kurulurken hata oluştu.', 'danger');
            });
    }

    function collect() {
        let valid = true;
        const places = [];
        rowsEl.querySelectorAll('.nearby-row').forEach(function (row) {
            const select = row.querySelector('.nearby-category');
            const nameInput = row.querySelector('.nearby-name');
            const kmInput = row.querySelector('.nearby-km');
            const name = nameInput.value.trim();
            const km = parseKm(kmInput.value);

            nameInput.classList.toggle('is-invalid', !name);
            const kmBad = isNaN(km) || km < 0 || km > 100;
            kmInput.classList.toggle('is-invalid', kmBad);
            if (!name || kmBad) valid = false;

            places.push({ nearbyCategoryId: parseInt(select.value, 10), placeName: name, distanceKm: km });
        });
        return valid ? places : null;
    }

    if (addBtn) {
        addBtn.addEventListener('click', function () {
            if (!categories.length) return;
            if (rowsEl.querySelectorAll('.nearby-row').length >= MAX_PLACES) {
                showAlert(`En fazla ${MAX_PLACES} yakın yer girilebilir.`, 'warning');
                return;
            }
            const row = addRow(null);
            row.querySelector('.nearby-name').focus();
        });
    }

    if (saveBtn) {
        saveBtn.addEventListener('click', function () {
            const places = collect();
            if (!places) {
                showAlert('Yer adı boş olamaz; uzaklık 0 ile 100 km arasında bir sayı olmalı (örn. 0,8).', 'danger');
                return;
            }

            const origHtml = saveBtn.innerHTML;
            saveBtn.disabled = true;
            saveBtn.innerHTML = '<span class="spinner-border spinner-border-sm me-1" role="status" aria-hidden="true"></span> Kaydediliyor...';

            fetch(url, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': getAntiForgeryToken()
                },
                body: JSON.stringify({ places: places })
            })
                .then(function (res) {
                    return readResponse(res).then(function (data) { return { ok: res.ok, data: data }; });
                })
                .then(function (r) {
                    if (r.ok && r.data && r.data.ok) {
                        alertArea.replaceChildren();
                        showAlert('Yakın yerler kaydedildi.', 'success');
                        load();
                    } else {
                        showAlert((r.data && r.data.message) || 'Yakın yerler kaydedilemedi.', 'danger');
                    }
                })
                .catch(function () {
                    showAlert('Sunucu ile iletişim kurulurken hata oluştu.', 'danger');
                })
                .finally(function () {
                    saveBtn.innerHTML = origHtml;
                    saveBtn.disabled = false;
                });
        });
    }

    load();
});
