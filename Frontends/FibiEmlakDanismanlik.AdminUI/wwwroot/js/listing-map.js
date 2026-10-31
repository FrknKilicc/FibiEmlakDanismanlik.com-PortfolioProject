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
    const latInput = document.getElementById('latInput');
    const lngInput = document.getElementById('lngInput');
    const applyCoordsBtn = document.getElementById('applyCoordsBtn');

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

    // Sayfa no-referrer; OSM karo politikası kaynak (origin) bilgisi istiyor, yalnızca karolara gönderilir
    L.tileLayer('https://tile.openstreetmap.org/{z}/{x}/{y}.png', {
        referrerPolicy: 'strict-origin-when-cross-origin',
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

        if (latInput) {
            latInput.value = lat.toFixed(6);
        }
        if (lngInput) {
            lngInput.value = lng.toFixed(6);
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

    // Koordinatları oku, parse et ve haritada uygula
    function applyCoordinates() {
        if (!canEdit) return;

        let rawLatVal = latInput ? latInput.value.trim() : '';
        let rawLngVal = lngInput ? lngInput.value.trim() : '';

        // Enlem alanına "41.01, 28.97" gibi iki sayı yapıştırılmışsa böl
        if (rawLatVal) {
            let parts = null;
            if (rawLatVal.indexOf(';') !== -1) {
                parts = rawLatVal.split(';');
            } else if (rawLatVal.indexOf(', ') !== -1) {
                parts = rawLatVal.split(', ');
            } else if (/\s+/.test(rawLatVal)) {
                parts = rawLatVal.split(/\s+/);
            } else if (/^-?\d+(?:[.,]\d+)?,-?\d+(?:[.,]\d+)?$/.test(rawLatVal)) {
                parts = rawLatVal.split(',');
            }

            if (parts && parts.length >= 2) {
                rawLatVal = parts[0].trim();
                rawLngVal = parts[1].trim();
                if (latInput) latInput.value = rawLatVal;
                if (lngInput) lngInput.value = rawLngVal;
            }
        }

        // Virgül ondalık ayırıcıysa noktaya çevir
        rawLatVal = rawLatVal.replace(',', '.');
        rawLngVal = rawLngVal.replace(',', '.');

        const lat = parseFloat(rawLatVal);
        const lng = parseFloat(rawLngVal);

        if (isNaN(lat) || isNaN(lng) || lat < -90 || lat > 90 || lng < -180 || lng > 180) {
            showMapAlert('Geçersiz koordinat: Enlem -90 ile 90, boylam -180 ile 180 arasında olmalı (örn. 41.01234, 28.97654).', 'danger');
            return;
        }

        const isOutsideTurkey = (lat < 35.5 || lat > 42.5 || lng < 25.5 || lng > 45.0);
        if (isOutsideTurkey) {
            showMapAlert('Girilen koordinatlar Türkiye sınırları dışında görünüyor. Konum haritada gösterildi.', 'warning');
        }

        if (!marker) {
            marker = L.marker([lat, lng], { draggable: canEdit }).addTo(map);
            attachMarkerEvents(marker);
        } else {
            marker.setLatLng([lat, lng]);
        }

        map.setView([lat, lng], 17);
        onLocationChanged(lat, lng);
    }

    if (applyCoordsBtn && canEdit) {
        applyCoordsBtn.addEventListener('click', function (e) {
            e.preventDefault();
            applyCoordinates();
        });
    }

    function handleCoordsKeyDown(e) {
        if (e.key === 'Enter') {
            e.preventDefault();
            applyCoordinates();
        }
    }

    if (latInput && canEdit) {
        latInput.addEventListener('keydown', handleCoordsKeyDown);
    }
    if (lngInput && canEdit) {
        lngInput.addEventListener('keydown', handleCoordsKeyDown);
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
