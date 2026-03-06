(function () {
    "use strict";

    const el = document.getElementById("contact-google-map");
    if (!el) return;

    const api = el.getAttribute("data-api"); 
    const listingId = el.getAttribute("data-listing-id");
    const listingTypeId = el.getAttribute("data-listing-type-id");

    if (!api || !listingId || !listingTypeId) return;
    if (typeof L === "undefined") return;

    const url =
        `${api}Map/GetListingMarker?listingId=${encodeURIComponent(listingId)}&listingTypeId=${encodeURIComponent(listingTypeId)}`;

    fetch(url)
        .then(r => r.ok ? r.json() : null)
        .then(data => {
            if (!data) {
                el.innerHTML =
                    `<div style="padding:30px;text-align:center">
                  Konum Bilgisi Alınıyor... Lütfen Bekleyiniz
              </div>`;
                return;
            }

            const lat = Number(data.latitude);
            const lng = Number(data.longitude);
            if (!Number.isFinite(lat) || !Number.isFinite(lng)) return;

            const zoom = Number(data.zoom || 15);

            const map = L.map("contact-google-map", {
                scrollWheelZoom: false
            }).setView([lat, lng], zoom);

            L.tileLayer("https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png", {
                attribution: "&copy; OpenStreetMap contributors"
            }).addTo(map);

            const iconHtml = buildMarkerHtml(data.iconKey);

            const icon = L.divIcon({
                className: "",
                html: iconHtml,
                iconSize: [40, 40],
                iconAnchor: [20, 40],
                popupAnchor: [0, -40]
            });

            const title = escapeHtml(data.title || "");
            const subtitle = escapeHtml(data.subtitle || "");

            const popupHtml =
                `<div class="map-popup"><strong>${title}</strong>` +
                (subtitle ? `<br/>${subtitle}` : "") +
                `</div>`;

            L.marker([lat, lng], { icon }).addTo(map).bindPopup(popupHtml).openPopup();
        })
        .catch(() => { });

    function escapeHtml(s) {
        return String(s).replace(/[&<>"']/g, (m) => ({
            "&": "&amp;",
            "<": "&lt;",
            ">": "&gt;",
            "\"": "&quot;",
            "'": "&#039;"
        }[m]));
    }

    function buildMarkerHtml(iconKey) {

        const parts = String(iconKey || "").split("_");

        const usage = parts[0] || "sale";
        const category = parts[1] || "housing";
        const status = parts[2] || "active";

        const usageClass =
            usage === "rent" ? "fibi-rent" :
                usage === "both" ? "fibi-both" :
                    "fibi-sale";

        const statusClass =
            status === "passive" ? "fibi-passive" : "";

        const icon =
            category === "land" ? "fas fa-map-marked-alt" :
                category === "commercial" ? "fas fa-store" :
                    "fas fa-home";

        return `<div class="fibi-marker ${usageClass} ${statusClass}">
                <i class="${icon}"></i>
            </div>`;
    }
})();