(function () {
    'use strict';

    document.addEventListener('DOMContentLoaded', function () {
        const containers = document.querySelectorAll('[data-image-upload]');
        if (!containers || containers.length === 0) {
            return;
        }

        const maxBytes = 15 * 1024 * 1024; // 15 MB

        function getAntiForgeryToken() {
            const tokenInput = document.querySelector('input[name="__RequestVerificationToken"]');
            return tokenInput ? tokenInput.value : '';
        }

        containers.forEach(function (container) {
            const targetId = container.getAttribute('data-target');
            const folder = container.getAttribute('data-folder');
            const previewId = container.getAttribute('data-preview');

            // Gizli dosya input'u
            const fileInput = document.createElement('input');
            fileInput.type = 'file';
            fileInput.accept = '.jpg,.jpeg,.png,.webp';
            fileInput.className = 'd-none';

            // Yükleme butonu
            const btn = document.createElement('button');
            btn.type = 'button';
            btn.className = 'btn btn-outline-secondary btn-sm';
            btn.innerHTML = '<i class="bi bi-upload me-1"></i>Bilgisayardan yükle';

            // Durum metni
            const statusEl = document.createElement('small');
            statusEl.className = 'ms-2 small';

            container.appendChild(fileInput);
            container.appendChild(btn);
            container.appendChild(statusEl);

            btn.addEventListener('click', function () {
                fileInput.click();
            });

            fileInput.addEventListener('change', async function () {
                const files = fileInput.files;
                if (!files || files.length === 0) {
                    return;
                }

                const file = files[0];
                if (file.size > maxBytes) {
                    statusEl.className = 'ms-2 small text-danger';
                    statusEl.textContent = 'Görsel boyutu 15 MB\'tan büyük olamaz.';
                    fileInput.value = '';
                    return;
                }

                const token = getAntiForgeryToken();
                const formData = new FormData();
                formData.append('dosya', file);
                formData.append('klasor', folder || '');

                const originalBtnHtml = btn.innerHTML;
                btn.disabled = true;
                btn.innerHTML = '<span class="spinner-border spinner-border-sm me-1" role="status" aria-hidden="true"></span>Yükleniyor...';
                statusEl.className = 'ms-2 small text-muted';
                statusEl.textContent = '';

                try {
                    const headers = {};
                    if (token) {
                        headers['RequestVerificationToken'] = token;
                    }

                    const response = await fetch('/icerik/gorsel-yukle', {
                        method: 'POST',
                        headers: headers,
                        body: formData
                    });

                    let result = null;
                    try {
                        result = await response.json();
                    } catch (_) { }

                    if (response.ok && result && result.ok) {
                        if (targetId) {
                            const targetInput = document.getElementById(targetId);
                            if (targetInput) {
                                targetInput.value = result.url || '';
                                targetInput.dispatchEvent(new Event('input', { bubbles: true }));
                                targetInput.dispatchEvent(new Event('change', { bubbles: true }));
                            }
                        }

                        if (previewId) {
                            const previewImg = document.getElementById(previewId);
                            if (previewImg) {
                                previewImg.src = result.previewUrl || result.url || '';
                                previewImg.classList.remove('d-none');
                            }
                        }

                        statusEl.className = 'ms-2 small text-success';
                        statusEl.textContent = 'Görsel yüklendi.';
                    } else {
                        const message = (result && result.message) ? result.message : 'Görsel yüklenemedi.';
                        statusEl.className = 'ms-2 small text-danger';
                        statusEl.textContent = message;
                    }
                } catch (err) {
                    statusEl.className = 'ms-2 small text-danger';
                    statusEl.textContent = 'Bağlantı hatası oluştu.';
                } finally {
                    btn.disabled = false;
                    btn.innerHTML = originalBtnHtml;
                    fileInput.value = '';
                }
            });
        });
    });
})();
