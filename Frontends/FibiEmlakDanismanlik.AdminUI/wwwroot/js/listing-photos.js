/**
 * Fibi Emlak - İlan Fotoğrafları Yönetim Betiği (Vanilla JS)
 * Fotoğraf yükleme (XHR ile ilerleme çubuğu), silme, kapak yapma, tür güncelleme ve sürükle-bırak sıralama
 */
document.addEventListener('DOMContentLoaded', function () {
    const container = document.getElementById('photosCard');
    if (!container) return;

    const typeId = container.dataset.listingTypeId;
    const listingId = container.dataset.listingId;
    const canEdit = container.dataset.canEdit === 'true';

    const alertArea = document.getElementById('photosAlertArea');
    const fileInput = document.getElementById('photoFileInput');
    const dropzone = document.getElementById('photoDropzone');
    const progressWrapper = document.getElementById('photoUploadProgressWrapper');
    const progressBar = document.getElementById('photoUploadProgressBar');
    const grid = document.getElementById('photoGrid');
    const saveOrderBtn = document.getElementById('saveOrderBtn');
    const photoCountBadge = document.getElementById('photoCountBadge');

    let draggedItem = null;
    let initialOrderIds = getOrderedIds();
    let isUploading = false;

    function getAntiForgeryToken() {
        const tokenInput = document.querySelector('input[name="__RequestVerificationToken"]');
        return tokenInput ? tokenInput.value : '';
    }

    function showAlert(message, type = 'danger') {
        if (!alertArea) return;
        const icon = type === 'success' ? 'check-circle-fill' : 'exclamation-triangle-fill';
        alertArea.innerHTML = `
            <div class="alert alert-${type} alert-dismissible fade show d-flex align-items-center mb-3" role="alert">
                <i class="bi bi-${icon} flex-shrink-0 me-2 fs-5"></i>
                <div class="flex-grow-1">${escapeHtml(message)}</div>
                <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Kapat"></button>
            </div>
        `;
    }

    function clearAlert() {
        if (alertArea) alertArea.innerHTML = '';
    }

    function escapeHtml(text) {
        if (!text) return '';
        const map = {
            '&': '&amp;',
            '<': '&lt;',
            '>': '&gt;',
            '"': '&quot;',
            "'": '&#039;'
        };
        return text.toString().replace(/[&<>"']/g, m => map[m]);
    }

    function getOrderedIds() {
        if (!grid) return [];
        return Array.from(grid.querySelectorAll('.photo-item'))
            .map(item => parseInt(item.dataset.imageId, 10))
            .filter(id => !isNaN(id) && id > 0);
    }

    function updateCountBadge() {
        if (!grid) return;
        const items = grid.querySelectorAll('.photo-item');
        if (photoCountBadge) {
            photoCountBadge.textContent = `Toplam ${items.length} / 30 fotoğraf`;
        }

        const emptyPlaceholder = grid.querySelector('.empty-photos-placeholder');
        if (items.length === 0 && !emptyPlaceholder) {
            grid.innerHTML = `
                <div class="col-12 text-center py-4 text-muted empty-photos-placeholder">
                    <i class="bi bi-images display-6 d-block mb-2 text-secondary"></i>
                    Henüz hiç fotoğraf eklenmemiş.
                </div>
            `;
        } else if (items.length > 0 && emptyPlaceholder) {
            emptyPlaceholder.remove();
        }
    }

    function loadPhotos() {
        return fetch(`/ilanlar/${typeId}/${listingId}/fotograflar`)
            .then(res => res.json())
            .then(result => {
                if (result && result.ok && result.data) {
                    renderPhotos(result.data);
                }
            })
            .catch(() => {
                showAlert('Fotoğraflar sunucudan yüklenirken bir hata oluştu.', 'danger');
            });
    }

    function renderPhotos(photos) {
        if (!grid) return;
        grid.innerHTML = '';

        if (!photos || photos.length === 0) {
            updateCountBadge();
            initialOrderIds = [];
            if (saveOrderBtn) saveOrderBtn.classList.add('d-none');
            return;
        }

        photos.sort((a, b) => a.sortOrder - b.sortOrder);

        photos.forEach(img => {
            const col = document.createElement('div');
            col.className = 'col-6 col-sm-4 col-md-3 col-lg-2 photo-item';
            col.dataset.imageId = img.listingImageId;
            col.dataset.currentKind = img.kind || 0;
            col.dataset.currentCaption = img.caption || '';
            col.draggable = canEdit;

            const kind = parseInt(img.kind, 10) || 0;
            const isPhoto = (kind === 0);
            const kindText = kind === 1 ? 'Kat Planı' : (kind === 2 ? 'Kroki' : (kind === 3 ? 'İmar Planı' : 'Kat Planı'));
            const thumb = img.thumbUrl || img.url || '';
            const imgClass = isPhoto ? 'object-fit-cover w-100 h-100' : 'object-fit-contain bg-light w-100 h-100';

            col.innerHTML = `
                <div class="card h-100 border shadow-none photo-card position-relative overflow-hidden">
                    <div class="photo-thumb-wrapper ratio ratio-4x3 bg-light">
                        <img src="${escapeHtml(thumb)}" class="${imgClass}" alt="Görsel ${img.listingImageId}" loading="lazy" />
                    </div>
                    ${isPhoto ? `
                    <span class="badge bg-primary position-absolute top-0 start-0 m-2 cover-badge ${img.isCover ? '' : 'd-none'}">
                        <i class="bi bi-star-fill me-1"></i> Kapak
                    </span>
                    ` : `
                    <span class="badge bg-info text-dark position-absolute top-0 start-0 m-2">
                        <i class="bi bi-rulers me-1"></i>${kindText}
                    </span>
                    `}
                    <div class="card-footer p-1 bg-white border-top-0">
                        ${canEdit ? `
                        <div class="d-flex justify-content-between gap-1 mb-1">
                            ${isPhoto ? `
                            <button type="button" class="btn btn-sm btn-outline-secondary flex-grow-1 p-1 set-cover-btn ${img.isCover ? 'd-none' : ''}" data-action="set-cover" title="Kapak yap">
                                <i class="bi bi-star"></i> Kapak
                            </button>
                            ` : ''}
                            <button type="button" class="btn btn-sm btn-outline-danger p-1 px-2 delete-photo-btn ${!isPhoto ? 'w-100' : ''}" data-action="delete" title="Fotoğrafı sil">
                                <i class="bi bi-trash"></i>
                            </button>
                        </div>
                        ` : ''}
                        <select class="form-select form-select-sm kind-select" data-action="kind" ${canEdit ? '' : 'disabled'}>
                            <option value="0" ${kind === 0 ? 'selected' : ''}>Fotoğraf</option>
                            <option value="1" ${kind === 1 ? 'selected' : ''}>Kat Planı</option>
                            <option value="2" ${kind === 2 ? 'selected' : ''}>Kroki</option>
                            <option value="3" ${kind === 3 ? 'selected' : ''}>İmar Planı</option>
                        </select>
                        ${!isPhoto ? `
                        <input type="text" class="form-control form-control-sm mt-1 caption-input" maxlength="100" placeholder="Başlık (ör. Zemin kat)" value="${escapeHtml(img.caption || '')}" ${canEdit ? '' : 'disabled'}>
                        ` : ''}
                    </div>
                </div>
            `;
            grid.appendChild(col);
        });

        if (canEdit) {
            initDragAndDrop();
        }
        updateCountBadge();
        initialOrderIds = getOrderedIds();
        if (saveOrderBtn) saveOrderBtn.classList.add('d-none');
    }

    function handleFiles(fileList) {
        clearAlert();
        if (!fileList || fileList.length === 0) return;

        if (isUploading) {
            showAlert('Yükleme sürüyor, lütfen bekleyin.', 'danger');
            return;
        }

        const currentCount = grid.querySelectorAll('.photo-item').length;
        if (currentCount + fileList.length > 30) {
            showAlert(`En fazla 30 fotoğraf yükleyebilirsiniz. Şu an ${currentCount} fotoğraf mevcut.`, 'danger');
            return;
        }

        if (fileList.length > 20) {
            showAlert('Tek seferde en fazla 20 fotoğraf yükleyebilirsiniz.', 'danger');
            return;
        }

        const allowedExts = ['.jpg', '.jpeg', '.png', '.webp'];
        const maxBytes = 15 * 1024 * 1024; // 15 MB

        const formData = new FormData();
        for (let i = 0; i < fileList.length; i++) {
            const file = fileList[i];
            const name = file.name || '';
            const lastDot = name.lastIndexOf('.');
            const ext = lastDot !== -1 ? name.substring(lastDot).toLowerCase() : '';

            if (!allowedExts.includes(ext)) {
                showAlert(`Desteklenmeyen dosya türü: ${name}. Sadece JPG, PNG veya WebP yükleyebilirsiniz.`, 'danger');
                return;
            }

            if (file.size > maxBytes) {
                showAlert(`${name} dosyası 15 MB sınırını aşıyor.`, 'danger');
                return;
            }

            formData.append('files', file);
        }

        uploadPhotos(formData);
    }

    function uploadPhotos(formData) {
        isUploading = true;

        if (progressWrapper && progressBar) {
            progressWrapper.classList.remove('d-none');
            progressBar.style.width = '0%';
            progressBar.setAttribute('aria-valuenow', '0');
        }

        const xhr = new XMLHttpRequest();
        xhr.open('POST', `/ilanlar/${typeId}/${listingId}/fotograflar`, true);
        xhr.setRequestHeader('RequestVerificationToken', getAntiForgeryToken());

        xhr.upload.onprogress = function (e) {
            if (e.lengthComputable && progressBar) {
                const percent = Math.round((e.loaded / e.total) * 100);
                progressBar.style.width = percent + '%';
                progressBar.setAttribute('aria-valuenow', percent.toString());
            }
        };

        xhr.onload = function () {
            isUploading = false;
            if (progressWrapper) progressWrapper.classList.add('d-none');
            if (fileInput) fileInput.value = '';

            let result = null;
            try {
                result = JSON.parse(xhr.responseText);
            } catch (_) { }

            if (xhr.status >= 200 && xhr.status < 300 && result && result.ok) {
                showAlert(result.message || 'Fotoğraflar başarıyla yüklendi.', 'success');
                if (result.data) {
                    renderPhotos(result.data);
                }
            } else {
                const message = (result && result.message) ? result.message : `Fotoğraf yüklenemedi (HTTP ${xhr.status}).`;
                showAlert(message, 'danger');
            }
        };

        xhr.onerror = function () {
            isUploading = false;
            if (progressWrapper) progressWrapper.classList.add('d-none');
            if (fileInput) fileInput.value = '';
            showAlert('Sunucu ile iletişim kurulurken bir ağ hatası oluştu.', 'danger');
        };

        xhr.send(formData);
    }

    function setCoverPhoto(imageId) {
        clearAlert();
        const orderedImageIds = getOrderedIds();
        const token = getAntiForgeryToken();

        fetch(`/ilanlar/${typeId}/${listingId}/fotograf-sirasi`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': token
            },
            body: JSON.stringify({
                orderedImageIds: orderedImageIds,
                coverImageId: imageId
            })
        })
        .then(res => res.json().then(data => ({ status: res.status, ok: res.ok, data })))
        .then(({ ok, data }) => {
            if (ok && data && data.ok) {
                showAlert('Kapak fotoğrafı başarıyla belirlendi.', 'success');
                loadPhotos();
            } else {
                showAlert((data && data.message) ? data.message : 'Kapak fotoğrafı güncellenemedi.', 'danger');
            }
        })
        .catch(() => {
            showAlert('Sunucu ile iletişim kurulurken hata oluştu.', 'danger');
        });
    }

    function deletePhoto(imageId, itemElement) {
        if (!confirm('Bu fotoğrafı silmek istediğinize emin misiniz?')) {
            return;
        }

        clearAlert();
        const token = getAntiForgeryToken();

        fetch(`/ilanlar/fotograf/${imageId}`, {
            method: 'DELETE',
            headers: {
                'RequestVerificationToken': token
            }
        })
        .then(res => res.json().then(data => ({ status: res.status, ok: res.ok, data })))
        .then(({ ok, data }) => {
            if (ok && data && data.ok) {
                showAlert('Fotoğraf silindi.', 'success');
                loadPhotos();
            } else {
                showAlert((data && data.message) ? data.message : 'Fotoğraf silinemedi.', 'danger');
            }
        })
        .catch(() => {
            showAlert('Sunucu ile iletişim kurulurken hata oluştu.', 'danger');
        });
    }

    function handleKindOrCaptionChange(e) {
        const target = e.target;
        if (!target.classList.contains('kind-select') && !target.classList.contains('caption-input')) {
            return;
        }

        const item = target.closest('.photo-item');
        if (!item) return;

        const imageId = parseInt(item.dataset.imageId, 10);
        if (!imageId) return;

        const kindSelect = item.querySelector('.kind-select');
        const captionInput = item.querySelector('.caption-input');

        const newKind = kindSelect ? parseInt(kindSelect.value, 10) : 0;
        const newCaption = captionInput ? captionInput.value : '';

        const previousKind = parseInt(item.dataset.currentKind, 10) || 0;
        const previousCaption = item.dataset.currentCaption || '';

        if (target.classList.contains('caption-input') && newCaption === previousCaption) {
            return;
        }
        if (target.classList.contains('kind-select') && newKind === previousKind) {
            return;
        }

        clearAlert();
        const token = getAntiForgeryToken();

        fetch(`/ilanlar/fotograf/${imageId}/tur`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': token
            },
            body: JSON.stringify({
                kind: newKind,
                caption: newCaption
            })
        })
        .then(res => res.json().then(data => ({ status: res.status, ok: res.ok, data })))
        .then(({ ok, data }) => {
            if (ok && data && data.ok) {
                showAlert('Görsel türü güncellendi.', 'success');
                loadPhotos();
            } else {
                showAlert((data && data.message) ? data.message : 'Görsel türü güncellenemedi.', 'danger');
                if (kindSelect) kindSelect.value = previousKind;
                if (captionInput) captionInput.value = previousCaption;
            }
        })
        .catch(() => {
            showAlert('Sunucu ile iletişim kurulurken hata oluştu.', 'danger');
            if (kindSelect) kindSelect.value = previousKind;
            if (captionInput) captionInput.value = previousCaption;
        });
    }

    function initDragAndDrop() {
        if (!grid || !canEdit) return;

        const items = grid.querySelectorAll('.photo-item');
        items.forEach(item => {
            item.setAttribute('draggable', 'true');

            item.removeEventListener('dragstart', handleDragStart);
            item.removeEventListener('dragover', handleDragOver);
            item.removeEventListener('drop', handleDrop);
            item.removeEventListener('dragend', handleDragEnd);

            item.addEventListener('dragstart', handleDragStart);
            item.addEventListener('dragover', handleDragOver);
            item.addEventListener('drop', handleDrop);
            item.addEventListener('dragend', handleDragEnd);
        });
    }

    function handleDragStart(e) {
        // Form elemanlarından sürükleme başlamasını engelle
        if (e.target.closest('select, input, button')) {
            e.preventDefault();
            return;
        }
        draggedItem = this;
        this.classList.add('is-dragging');
        e.dataTransfer.effectAllowed = 'move';
        e.dataTransfer.setData('text/plain', this.dataset.imageId);
    }

    function handleDragOver(e) {
        e.preventDefault();
        e.dataTransfer.dropEffect = 'move';

        if (this === draggedItem) return;

        const bounding = this.getBoundingClientRect();
        const offset = e.clientX - bounding.left;
        if (offset > bounding.width / 2) {
            this.after(draggedItem);
        } else {
            this.before(draggedItem);
        }
    }

    function handleDrop(e) {
        e.preventDefault();
    }

    function handleDragEnd() {
        this.classList.remove('is-dragging');
        draggedItem = null;

        const currentIds = getOrderedIds();
        const changed = currentIds.length !== initialOrderIds.length ||
            currentIds.some((id, idx) => id !== initialOrderIds[idx]);

        if (saveOrderBtn) {
            if (changed) {
                saveOrderBtn.classList.remove('d-none');
            } else {
                saveOrderBtn.classList.add('d-none');
            }
        }
    }

    if (canEdit && dropzone) {
        dropzone.addEventListener('dragover', function (e) {
            e.preventDefault();
            dropzone.classList.add('photo-dropzone-active');
        });

        dropzone.addEventListener('dragleave', function () {
            dropzone.classList.remove('photo-dropzone-active');
        });

        dropzone.addEventListener('drop', function (e) {
            e.preventDefault();
            dropzone.classList.remove('photo-dropzone-active');
            if (e.dataTransfer && e.dataTransfer.files) {
                handleFiles(e.dataTransfer.files);
            }
        });
    }

    if (canEdit && fileInput) {
        fileInput.addEventListener('change', function () {
            if (fileInput.files) {
                handleFiles(fileInput.files);
            }
        });
    }

    if (canEdit && grid) {
        grid.addEventListener('change', handleKindOrCaptionChange);

        grid.addEventListener('click', function (e) {
            const btn = e.target.closest('button');
            if (!btn) return;

            const action = btn.dataset.action;
            const item = btn.closest('.photo-item');
            if (!item) return;

            const imageId = parseInt(item.dataset.imageId, 10);
            if (!imageId) return;

            if (action === 'set-cover') {
                setCoverPhoto(imageId);
            } else if (action === 'delete') {
                deletePhoto(imageId, item);
            }
        });
    }

    if (canEdit && saveOrderBtn) {
        saveOrderBtn.addEventListener('click', function () {
            clearAlert();
            const currentIds = getOrderedIds();
            const token = getAntiForgeryToken();

            saveOrderBtn.disabled = true;
            const origHtml = saveOrderBtn.innerHTML;
            saveOrderBtn.innerHTML = '<span class="spinner-border spinner-border-sm me-1" role="status" aria-hidden="true"></span> Kaydediliyor...';

            fetch(`/ilanlar/${typeId}/${listingId}/fotograf-sirasi`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': token
                },
                body: JSON.stringify({
                    orderedImageIds: currentIds,
                    coverImageId: null
                })
            })
            .then(res => res.json().then(data => ({ status: res.status, ok: res.ok, data })))
            .then(({ ok, data }) => {
                saveOrderBtn.disabled = false;
                saveOrderBtn.innerHTML = origHtml;
                if (ok && data && data.ok) {
                    showAlert('Fotoğraf sıralaması başarıyla kaydedildi.', 'success');
                    initialOrderIds = currentIds;
                    saveOrderBtn.classList.add('d-none');
                } else {
                    showAlert((data && data.message) ? data.message : 'Sıralama kaydedilemedi.', 'danger');
                }
            })
            .catch(() => {
                saveOrderBtn.disabled = false;
                saveOrderBtn.innerHTML = origHtml;
                showAlert('Sunucu ile iletişim kurulurken hata oluştu.', 'danger');
            });
        });
    }

    // Tehlikeli bölge: Silme onay metni kontrolü
    const deleteConfirmInput = document.getElementById('deleteConfirmInput');
    const deleteSubmitBtn = document.getElementById('deleteSubmitBtn');
    if (deleteConfirmInput && deleteSubmitBtn) {
        deleteConfirmInput.addEventListener('input', function () {
            deleteSubmitBtn.disabled = this.value.trim() !== 'SİL';
        });
    }

    const deleteListingForm = document.getElementById('deleteListingForm');
    if (deleteListingForm) {
        deleteListingForm.addEventListener('submit', function (e) {
            if (!confirm('DİKKAT: İlan ve bağlı tüm fotoğraflar kalıcı olarak silinecektir. Devam etmek istediğinize emin misiniz?')) {
                e.preventDefault();
            }
        });
    }

    // İlk yüklemede fotoğrafları getir ve sürükle-bırak olaylarını bağla
    if (typeId && listingId) {
        loadPhotos();
    } else if (canEdit) {
        initDragAndDrop();
    }
});
