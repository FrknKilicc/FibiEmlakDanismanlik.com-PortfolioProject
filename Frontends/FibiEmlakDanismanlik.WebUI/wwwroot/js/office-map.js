(function () {
    "use strict";

    // Harita öğesi veya Leaflet kütüphanesi yoksa çık
    const el = document.getElementById("officeMap");
    if (!el || typeof L === "undefined") {
        return;
    }

    // data-lat, data-lng ve data-zoom özniteliklerini sayıya çevir
    const lat = Number(el.getAttribute("data-lat"));
    const lng = Number(el.getAttribute("data-lng"));
    const zoom = Number(el.getAttribute("data-zoom") || 16);

    // Koordinatlar veya yakınlaştırma seviyesi geçersizse çık
    if (!Number.isFinite(lat) || !Number.isFinite(lng) || !Number.isFinite(zoom)) {
        return;
    }

    // Haritayı başlat ve fare tekerleği ile yakınlaştırmayı devre dışı bırak
    const map = L.map(el, {
        scrollWheelZoom: false
    }).setView([lat, lng], zoom);

    // OpenStreetMap katmanını ekle
    L.tileLayer("https://tile.openstreetmap.org/{z}/{x}/{y}.png", {
        maxZoom: 19,
        attribution: "&copy; OpenStreetMap katkıcıları"
    }).addTo(map);

    // Ofis konumuna marker ekle ve popup aç
    L.marker([lat, lng])
        .addTo(map)
        .bindPopup("Fibi Yapı Emlak & Danışmanlık")
        .openPopup();
})();
