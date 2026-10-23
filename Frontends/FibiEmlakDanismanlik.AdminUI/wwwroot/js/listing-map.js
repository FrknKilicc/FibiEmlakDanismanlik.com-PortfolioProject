/**
 * Fibi Emlak - İlan Konumu Harita Pini Yönetim Betiği (Vanilla JS & Leaflet)
 * OpenStreetMap karoları ve Leaflet ile harita pini yerleştirme, sürükleme ve koordinat kaydetme
 */
document.addEventListener('DOMContentLoaded', function () {
    const card = document.getElementById('listingMapCard');
    if (!card || typeof L === 'undefined') return;

    const mapElement = document.getElementById('listingMap');
    if (!mapElement) return;

    const typeId = card.dataset.typeId;
    const listingId = card.dataset.listingId;
    const canEdit = card.dataset.canEdit === 'true';

    const rawLat = card.dataset.lat;
    const rawLng = card.dataset.lng;

    let currentLat = rawLat && !isNaN(parseFloat(rawLat)) ? parseFloat(rawLat) : null;
    let currentLng = rawLng && !isNaN(parseFloat(rawLng)) ? parseFloat(rawLng) : null;

    const coordDisplay = document.getElementById('coordDisplay');
    const saveBtn = document.getElementById('saveLocationBtn');
    const geoSourceBadge = document.getElementById('geoSourceBadge');
    const alertArea = document.getElementById('mapAlertArea');

    function getAntiForgeryToken() {
        const tokenInput = document.querySelector('input[name="__RequestVerificationToken"]');
        return tokenInput ? tokenInput.value : '';
    }

    function showMapAlert(message, type = 'success') {
        if (!alertArea) return;
        const icon = type === 'success' ? 'check-circle-fill' : 'exclamation-triangle-fill';
        alertArea.innerHTML = `
            <div class="alert alert-${type} alert-dismissible fade show d-flex align-items-center mb-2" role="alert">
                <i class="bi bi-${icon} flex-shrink-0 me-2"></i>
                <div class="flex-grow-1 small">${escapeHtml(message)}</div>
                <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Kapat"></button>
            </div>
        `;
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

    // Leaflet yerel ikon yollarını garantiye al
    try {
        delete L.Icon.Default.prototype._getIconUrl;
        L.Icon.Default.mergeOptions({
            iconRetinaUrl: '/lib/leaflet/images/marker-icon-2x.png',
            iconUrl: '/lib/leaflet/images/marker-icon.png',
            shadowUrl: '/lib/leaflet/images/marker-shadow.png'
        });
    } catch (_) { }

    const hasCoords = currentLat !== null && currentLng !== null;
    const defaultCenter = hasCoords ? [currentLat, currentLng] : [39.0, 35.0]; // Türkiye geneli
    const defaultZoom = hasCoords ? 15 : 6;

    const map = L.map('listingMap', {
        center: defaultCenter,
        zoom: defaultZoom,
        scrollWheelZoom: false
    });

    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        maxZoom: 19,
        attribution: '&copy; <a href="https://www.openstreetmap.org/copyright" target="_blank" rel="noopener">OpenStreetMap</a> katkıcıları'
    }).addTo(map);

    let marker = null;

    function attachMarkerEvents(m) {
        if (!canEdit) return;
        m.on('dragend', function (e) {
            const pos = e.target.getLatLng();
            onLocationChanged(pos.lat, pos.lng);
        });
    }

    function onLocationChanged(lat, lng) {
        currentLat = lat;
        currentLng = lng;

        if (coordDisplay) {
            coordDisplay.textContent = `${lat.toFixed(5)}, ${lng.toFixed(5)}`;
        }

        if (saveBtn && canEdit) {
            saveBtn.disabled = false;
        }
    }

    if (hasCoords) {
        marker = L.marker([currentLat, currentLng], {
            draggable: canEdit
        }).addTo(map);
        attachMarkerEvents(marker);
    }

    if (canEdit) {
        map.on('click', function (e) {
            const lat = e.latlng.lat;
            const lng = e.latlng.lng;

            if (!marker) {
                marker = L.marker([lat, lng], {
                    draggable: true
                }).addTo(map);
                attachMarkerEvents(marker);
            } else {
                marker.setLatLng([lat, lng]);
            }

            onLocationChanged(lat, lng);
        });
    }

    if (saveBtn && canEdit) {
        saveBtn.addEventListener('click', function () {
            if (currentLat === null || currentLng === null) {
                showMapAlert('Lütfen önce harita üzerinden bir konum belirleyin.', 'danger');
                return;
            }

            saveBtn.disabled = true;
            const origHtml = saveBtn.innerHTML;
            saveBtn.innerHTML = '<span class="spinner-border spinner-border-sm me-1" role="status" aria-hidden="true"></span> Kaydediliyor...';

            const token = getAntiForgeryToken();

            fetch(`/ilanlar/${typeId}/${listingId}/konum`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': token
                },
                body: JSON.stringify({
                    latitude: currentLat,
                    longitude: currentLng
                })
            })
            .then(res => res.json().then(data => ({ status: res.status, ok: res.ok, data })))
            .then(({ ok, data }) => {
                saveBtn.innerHTML = origHtml;
                if (ok && data && data.ok) {
                    showMapAlert('Konum başarıyla güncellendi.', 'success');
                    if (geoSourceBadge) {
                        geoSourceBadge.className = 'badge bg-success';
                        geoSourceBadge.innerHTML = '<i class="bi bi-geo-alt-fill me-1"></i>Kesin Konum';
                    }
                    saveBtn.disabled = true;
                } else {
                    saveBtn.disabled = false;
                    showMapAlert((data && data.message) ? data.message : 'Konum güncellenemedi.', 'danger');
                }
            })
            .catch(() => {
                saveBtn.innerHTML = origHtml;
                saveBtn.disabled = false;
                showMapAlert('Sunucu ile iletişim kurulurken hata oluştu.', 'danger');
            });
        });
    }

    // Harita karo boyutu hesaplama düzeltmesi
    setTimeout(function () {
        map.invalidateSize();
    }, 250);
});