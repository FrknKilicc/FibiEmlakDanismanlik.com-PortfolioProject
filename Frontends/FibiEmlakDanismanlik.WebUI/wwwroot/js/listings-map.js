(function () {
    "use strict";

    // Sayfa hazır olduğunda başlat
    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", init);
    } else {
        init();
    }

    function init() {
        const root = document.getElementById("listingsMapRoot");
        if (!root) return;

        if (typeof L === "undefined") {
            const listEl = document.getElementById("lmList");
            if (listEl) {
                listEl.textContent = "";
                const err = document.createElement("div");
                err.className = "lm-error";
                err.textContent = "Harita kütüphanesi yüklenemedi.";
                listEl.appendChild(err);
            }
            return;
        }

        const citySelect = document.getElementById("lmCity");
        const districtSelect = document.getElementById("lmDistrict");
        const listEl = document.getElementById("lmList");
        const countEl = document.getElementById("lmCount");

        // Durum yönetimi (öncelik URL sorgu parametrelerinde, ardından kök veri özniteliklerinde)
        const state = {
            usage: "",
            category: "",
            city: "",
            district: ""
        };

        readStateFromUrlOrDataset();

        // Harita ve Kümeleme Grubu Kurulumu
        const map = L.map("lmMap", {
            scrollWheelZoom: true
        }).setView([39.0, 35.0], 6);

        L.tileLayer("https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png", {
            attribution: "&copy; OpenStreetMap contributors"
        }).addTo(map);

        const clusterGroup = L.markerClusterGroup({
            showCoverageOnHover: false,
            spiderfyOnMaxZoom: true,
            maxClusterRadius: 45
        });
        map.addLayer(clusterGroup);

        // Veri depoları
        let allListings = [];
        let currentFilteredListings = [];
        let updateListTimer = null;

        // Harita hareket ettiğinde görünen ilanları güncelle (debounce)
        map.on("moveend", scheduleUpdateVisibleList);

        // Olay dinleyicileri (Filtre çubuğu)
        setupToolbarEvents();

        // API'den ilanları yükle
        loadListings();

        /* ---------------------------------------------------------
         * Veri Yükleme ve Şehir Listesi
         * --------------------------------------------------------- */
        function loadListings() {
            let api = root.getAttribute("data-api") || "";
            if (api && !api.endsWith("/")) {
                api += "/";
            }
            const url = api + "Map/listings";

            fetch(url)
                .then(res => {
                    if (!res.ok) {
                        throw new Error("HTTP " + res.status);
                    }
                    return res.json();
                })
                .then(data => {
                    if (!Array.isArray(data)) {
                        throw new Error("Geçersiz API yanıtı");
                    }
                    allListings = data;
                    populateCities(allListings);
                    populateDistricts();
                    applyFilters({ fitBounds: true });
                })
                .catch(() => {
                    if (listEl) {
                        listEl.textContent = "";
                        const errDiv = document.createElement("div");
                        errDiv.className = "lm-error";
                        errDiv.textContent = "İlanlar yüklenemedi.";
                        listEl.appendChild(errDiv);
                    }
                    if (countEl) {
                        countEl.textContent = "0 ilan";
                    }
                });
        }

        function populateCities(listings) {
            if (!citySelect) return;

            const citySet = new Set();
            listings.forEach(item => {
                const c = (item.city || "").trim();
                if (c) citySet.add(c);
            });

            const cities = Array.from(citySet);
            cities.sort((a, b) => a.localeCompare(b, "tr"));

            // Mevcut seçenekleri temizle ("Tüm İller" hariç)
            citySelect.textContent = "";
            const defaultOpt = document.createElement("option");
            defaultOpt.value = "";
            defaultOpt.textContent = "Tüm İller";
            citySelect.appendChild(defaultOpt);

            cities.forEach(cityName => {
                const opt = document.createElement("option");
                opt.value = cityName;
                opt.textContent = cityName;
                citySelect.appendChild(opt);
            });

            if (state.city) {
                citySelect.value = state.city;
                if (citySelect.value !== state.city) {
                    state.city = "";
                    state.district = "";
                    updateUrl();
                }
            }
        }

        function populateDistricts() {
            if (!districtSelect) return;

            if (!state.city) {
                districtSelect.textContent = "";
                const defaultOpt = document.createElement("option");
                defaultOpt.value = "";
                defaultOpt.textContent = "Tüm İlçeler";
                districtSelect.appendChild(defaultOpt);
                districtSelect.value = "";
                districtSelect.disabled = true;
                if (state.district) {
                    state.district = "";
                    updateUrl();
                }
                return;
            }

            const districtSet = new Set();
            allListings.forEach(item => {
                if ((item.city || "").trim() === state.city) {
                    const d = (item.district || "").trim();
                    if (d) districtSet.add(d);
                }
            });

            const districts = Array.from(districtSet);
            districts.sort((a, b) => a.localeCompare(b, "tr"));

            districtSelect.textContent = "";
            const defaultOpt = document.createElement("option");
            defaultOpt.value = "";
            defaultOpt.textContent = "Tüm İlçeler";
            districtSelect.appendChild(defaultOpt);

            districts.forEach(districtName => {
                const opt = document.createElement("option");
                opt.value = districtName;
                opt.textContent = districtName;
                districtSelect.appendChild(opt);
            });

            districtSelect.disabled = false;

            if (state.district && districts.includes(state.district)) {
                districtSelect.value = state.district;
            } else {
                const hadDistrict = !!state.district;
                state.district = "";
                districtSelect.value = "";
                if (hadDistrict) {
                    updateUrl();
                }
            }
        }

        /* ---------------------------------------------------------
         * Durum ve Filtreleme İşlemleri
         * --------------------------------------------------------- */
        function readStateFromUrlOrDataset() {
            const params = new URLSearchParams(window.location.search);
            const initialUsage = root.getAttribute("data-initial-usage") || "";
            const initialCategory = root.getAttribute("data-initial-category") || "";

            let usage = params.has("usage") ? (params.get("usage") || "") : initialUsage;
            if (usage !== "sale" && usage !== "rent") {
                usage = "";
            }

            let category = params.has("category") ? (params.get("category") || "") : initialCategory;
            if (category !== "housing" && category !== "land" && category !== "commercial") {
                category = "";
            }

            const city = params.has("city") ? (params.get("city") || "").trim() : "";
            const district = params.has("district") ? (params.get("district") || "").trim() : "";

            state.usage = usage;
            state.category = category;
            state.city = city;
            state.district = city ? district : "";

            updateUsageButtons();
            updateCategoryButtons();
            if (citySelect) {
                citySelect.value = state.city;
            }
        }

        function updateUrl() {
            const sp = new URLSearchParams();
            if (state.usage) sp.set("usage", state.usage);
            if (state.category) sp.set("category", state.category);
            if (state.city) {
                sp.set("city", state.city);
                if (state.district) sp.set("district", state.district);
            }
            const qs = sp.toString();
            const newUrl = window.location.pathname + (qs ? "?" + qs : "");
            window.history.replaceState(null, "", newUrl);
        }

        function setupToolbarEvents() {
            const toolbar = root.querySelector(".lm-toolbar");
            if (toolbar) {
                toolbar.addEventListener("click", e => {
                    const usageBtn = e.target.closest("button[data-usage]");
                    if (usageBtn) {
                        const val = usageBtn.getAttribute("data-usage") || "";
                        if (state.usage !== val) {
                            state.usage = val;
                            updateUsageButtons();
                            updateUrl();
                            applyFilters({ fitBounds: true });
                        }
                        return;
                    }

                    const catBtn = e.target.closest("button[data-category]");
                    if (catBtn) {
                        const val = catBtn.getAttribute("data-category") || "";
                        if (state.category !== val) {
                            state.category = val;
                            updateCategoryButtons();
                            updateUrl();
                            applyFilters({ fitBounds: true });
                        }
                        return;
                    }
                });
            }

            if (citySelect) {
                citySelect.addEventListener("change", () => {
                    state.city = (citySelect.value || "").trim();
                    state.district = "";
                    populateDistricts();
                    updateUrl();
                    applyFilters({ fitBounds: true });
                });
            }

            if (districtSelect) {
                districtSelect.addEventListener("change", () => {
                    state.district = (districtSelect.value || "").trim();
                    updateUrl();
                    applyFilters({ fitBounds: true });
                });
            }

            window.addEventListener("popstate", () => {
                readStateFromUrlOrDataset();
                populateDistricts();
                applyFilters({ fitBounds: true });
            });
        }

        function updateUsageButtons() {
            const buttons = root.querySelectorAll("button[data-usage]");
            buttons.forEach(btn => {
                const val = btn.getAttribute("data-usage") || "";
                const active = val === state.usage;
                btn.classList.toggle("is-active", active);
                btn.setAttribute("aria-pressed", active ? "true" : "false");
            });
        }

        function updateCategoryButtons() {
            const buttons = root.querySelectorAll("button[data-category]");
            buttons.forEach(btn => {
                const val = btn.getAttribute("data-category") || "";
                const active = val === state.category;
                btn.classList.toggle("is-active", active);
                btn.setAttribute("aria-pressed", active ? "true" : "false");
            });
        }

        function applyFilters(options) {
            const fit = options && options.fitBounds;

            currentFilteredListings = allListings.filter(item => {
                // Kullanım türü (1: satılık, 2: kiralık)
                if (state.usage === "sale" && item.usageTypeId !== 1) return false;
                if (state.usage === "rent" && item.usageTypeId !== 2) return false;

                // Kategori
                if (state.category && item.categoryKey !== state.category) return false;

                // İl
                if (state.city && (item.city || "").trim() !== state.city) return false;

                // İlçe
                if (state.district && (item.district || "").trim() !== state.district) return false;

                return true;
            });

            clusterGroup.clearLayers();

            currentFilteredListings.forEach(item => {
                const lat = Number(item.latitude);
                const lng = Number(item.longitude);
                if (!Number.isFinite(lat) || !Number.isFinite(lng)) {
                    item._marker = null;
                    return;
                }

                const usageClass = item.usageTypeId === 2 ? "fibi-rent" : "fibi-sale";
                const iconClass = item.categoryKey === "land" ? "fas fa-map-marked-alt"
                    : item.categoryKey === "commercial" ? "fas fa-store"
                    : "fas fa-home";
                const approxClass = item.isApproximate ? " lm-approx" : "";

                const markerHtml = `<div class="fibi-marker ${usageClass}${approxClass}"><i class="${iconClass}"></i></div>`;

                const icon = L.divIcon({
                    className: "",
                    html: markerHtml,
                    iconSize: [42, 42],
                    iconAnchor: [21, 50],
                    popupAnchor: [0, -46]
                });

                const detailUrl = getDetailUrl(item);
                const marker = L.marker([lat, lng], { icon });
                marker.bindPopup(() => createPopupElement(item, detailUrl));

                item._marker = marker;
                clusterGroup.addLayer(marker);
            });

            if (fit && clusterGroup.getLayers().length > 0) {
                map.fitBounds(clusterGroup.getBounds(), {
                    padding: [40, 40],
                    maxZoom: 13
                });
            }

            updateVisibleList();
        }

        /* ---------------------------------------------------------
         * Haritada Görünen İlanların Listelenmesi
         * --------------------------------------------------------- */
        function scheduleUpdateVisibleList() {
            if (updateListTimer) {
                clearTimeout(updateListTimer);
            }
            updateListTimer = setTimeout(updateVisibleList, 60);
        }

        function updateVisibleList() {
            if (!listEl) return;

            listEl.textContent = "";

            const bounds = map.getBounds();
            const visibleListings = currentFilteredListings.filter(item => {
                const lat = Number(item.latitude);
                const lng = Number(item.longitude);
                if (!Number.isFinite(lat) || !Number.isFinite(lng)) return false;
                return bounds.contains([lat, lng]);
            });

            const saleVisible = visibleListings.filter(item => item.usageTypeId !== 2);
            const rentVisible = visibleListings.filter(item => item.usageTypeId === 2);

            if (countEl) {
                if (!state.usage) {
                    countEl.textContent = `${visibleListings.length} ilan haritada · ${saleVisible.length} satılık · ${rentVisible.length} kiralık`;
                } else {
                    countEl.textContent = `${visibleListings.length} ilan haritada · toplam ${currentFilteredListings.length}`;
                }
            }

            if (visibleListings.length === 0) {
                const emptyDiv = document.createElement("div");
                emptyDiv.className = "lm-empty";
                emptyDiv.textContent = "Bu alanda ilan yok. Haritayı uzaklaştırın veya filtreleri değiştirin.";
                listEl.appendChild(emptyDiv);
                return;
            }

            const fragment = document.createDocumentFragment();

            if (!state.usage) {
                // Tümü: Önce satılıklar, sonra kiralıklar
                if (saleVisible.length > 0) {
                    const headSale = document.createElement("div");
                    headSale.className = "lm-list-head lm-list-head--sale";
                    const titleSale = document.createElement("span");
                    titleSale.textContent = "Satılık İlanlar";
                    const countSale = document.createElement("span");
                    countSale.className = "lm-list-count";
                    countSale.textContent = String(saleVisible.length);
                    headSale.appendChild(titleSale);
                    headSale.appendChild(countSale);
                    fragment.appendChild(headSale);

                    saleVisible.forEach(item => {
                        const detailUrl = getDetailUrl(item);
                        const card = createCardElement(item, item._marker, detailUrl);
                        fragment.appendChild(card);
                    });
                }

                if (rentVisible.length > 0) {
                    const headRent = document.createElement("div");
                    headRent.className = "lm-list-head lm-list-head--rent";
                    const titleRent = document.createElement("span");
                    titleRent.textContent = "Kiralık İlanlar";
                    const countRent = document.createElement("span");
                    countRent.className = "lm-list-count";
                    countRent.textContent = String(rentVisible.length);
                    headRent.appendChild(titleRent);
                    headRent.appendChild(countRent);
                    fragment.appendChild(headRent);

                    rentVisible.forEach(item => {
                        const detailUrl = getDetailUrl(item);
                        const card = createCardElement(item, item._marker, detailUrl);
                        fragment.appendChild(card);
                    });
                }
            } else {
                // Tek liste: Başlık basmadan
                visibleListings.forEach(item => {
                    const detailUrl = getDetailUrl(item);
                    const card = createCardElement(item, item._marker, detailUrl);
                    fragment.appendChild(card);
                });
            }

            listEl.appendChild(fragment);
        }

        /* ---------------------------------------------------------
         * DOM Öğeleri Üreticileri (Popup ve Liste Kartı)
         * API metinleri ASLA innerHTML ile yazılmaz; textContent kullanılır.
         * --------------------------------------------------------- */
        function createPopupElement(item, detailUrl) {
            const popup = document.createElement("div");
            popup.className = "lm-popup";

            // Görsel
            const imgWrap = document.createElement("div");
            imgWrap.className = "lm-popup-img-wrap";
            const img = document.createElement("img");
            img.className = "lm-popup-img";
            img.alt = item.title || "İlan Görseli";
            const fallbackSrc = "/Realshed/assets/images/FibiLogo-footer.png";
            img.onerror = function () {
                this.onerror = null;
                this.src = fallbackSrc;
            };
            img.src = item.imageUrl || fallbackSrc;
            imgWrap.appendChild(img);
            popup.appendChild(imgWrap);

            // İçerik gövdesi
            const content = document.createElement("div");
            content.className = "lm-popup-content";

            // Rozet yerine belirgin etiket (lm-card-tag) + listingTypeName (lm-card-type)
            const isRent = item.usageTypeId === 2;
            const tag = document.createElement("span");
            tag.className = "lm-card-tag " + (isRent ? "lm-card-tag--rent" : "lm-card-tag--sale");
            tag.textContent = isRent ? "KİRALIK" : "SATILIK";
            content.appendChild(tag);

            if (item.listingTypeName) {
                const typeSpan = document.createElement("span");
                typeSpan.className = "lm-card-type";
                typeSpan.textContent = item.listingTypeName;
                content.appendChild(typeSpan);
            }

            // Başlık
            const title = document.createElement("h4");
            title.className = "lm-popup-title";
            title.textContent = item.title || "";
            content.appendChild(title);

            // Fiyat
            const price = document.createElement("div");
            price.className = "lm-popup-price";
            const priceNum = typeof item.price === "number" ? item.price : Number(item.price) || 0;
            const formattedPrice = new Intl.NumberFormat("tr-TR", { maximumFractionDigits: 0 }).format(priceNum) + " ₺";
            price.appendChild(document.createTextNode(formattedPrice));
            if (isRent) {
                const period = document.createElement("span");
                period.className = "lm-price-period";
                period.textContent = "/ ay";
                price.appendChild(period);
            }
            content.appendChild(price);

            // Alan & Oda özellikleri
            const features = [];
            if (item.area != null && item.area !== "") {
                features.push(`${item.area} m²`);
            }
            if (item.rooms != null && item.rooms !== "") {
                features.push(String(item.rooms));
            }
            if (features.length > 0) {
                const feat = document.createElement("div");
                feat.className = "lm-popup-features";
                feat.textContent = features.join(" · ");
                content.appendChild(feat);
            }

            // Konum ("Mahalle, İlçe / İl")
            const locText = formatLocation(item);
            if (locText) {
                const loc = document.createElement("div");
                loc.className = "lm-popup-location";
                loc.textContent = locText;
                content.appendChild(loc);
            }

            // Yaklaşık konum notu
            if (item.isApproximate) {
                const approx = document.createElement("div");
                approx.className = "lm-approx-note";
                approx.textContent = "Konum yaklaşıktır";
                content.appendChild(approx);
            }

            // İlanı İncele linki
            const link = document.createElement("a");
            link.className = "lm-popup-link";
            link.href = detailUrl;
            link.textContent = "İlanı İncele";
            content.appendChild(link);

            popup.appendChild(content);
            return popup;
        }

        function createCardElement(item, marker, detailUrl) {
            const isRent = item.usageTypeId === 2;
            const card = document.createElement("article");
            card.className = "lm-card " + (isRent ? "lm-card--rent" : "lm-card--sale");
            card.tabIndex = 0;
            card.setAttribute("role", "button");

            // Karta tıklayınca ilgili işaretçiye odaklan ve popup aç
            card.addEventListener("click", e => {
                if (e.target.closest("a")) return;
                if (marker) {
                    clusterGroup.zoomToShowLayer(marker, () => {
                        marker.openPopup();
                    });
                }
            });

            card.addEventListener("keydown", e => {
                if (e.key === "Enter" || e.key === " ") {
                    if (e.target.closest("a")) return;
                    e.preventDefault();
                    if (marker) {
                        clusterGroup.zoomToShowLayer(marker, () => {
                            marker.openPopup();
                        });
                    }
                }
            });

            // Küçük görsel
            const imgWrap = document.createElement("div");
            imgWrap.className = "lm-card-img-wrap";
            const img = document.createElement("img");
            img.className = "lm-card-img";
            img.alt = item.title || "İlan Görseli";
            const fallbackSrc = "/Realshed/assets/images/FibiLogo-footer.png";
            img.onerror = function () {
                this.onerror = null;
                this.src = fallbackSrc;
            };
            img.src = item.imageUrl || fallbackSrc;
            imgWrap.appendChild(img);
            card.appendChild(imgWrap);

            // Bilgi bölümü
            const info = document.createElement("div");
            info.className = "lm-card-info";

            // Üst satır (belirgin etiket + tür)
            const top = document.createElement("div");
            top.className = "lm-card-top";

            const tag = document.createElement("span");
            tag.className = "lm-card-tag " + (isRent ? "lm-card-tag--rent" : "lm-card-tag--sale");
            tag.textContent = isRent ? "KİRALIK" : "SATILIK";
            top.appendChild(tag);

            if (item.listingTypeName) {
                const typeSpan = document.createElement("span");
                typeSpan.className = "lm-card-type";
                typeSpan.textContent = item.listingTypeName;
                top.appendChild(typeSpan);
            }
            info.appendChild(top);

            // Başlık
            const title = document.createElement("h4");
            title.className = "lm-card-title";
            title.textContent = item.title || "";
            info.appendChild(title);

            // Fiyat etiketi: Satış fiyatı / Aylık kira
            const priceLabel = document.createElement("span");
            priceLabel.className = "lm-price-label";
            priceLabel.textContent = isRent ? "Aylık kira" : "Satış fiyatı";
            info.appendChild(priceLabel);

            // Fiyat
            const price = document.createElement("div");
            price.className = "lm-card-price";
            const priceNum = typeof item.price === "number" ? item.price : Number(item.price) || 0;
            const formattedPrice = new Intl.NumberFormat("tr-TR", { maximumFractionDigits: 0 }).format(priceNum) + " ₺";
            price.appendChild(document.createTextNode(formattedPrice));
            if (isRent) {
                const period = document.createElement("span");
                period.className = "lm-price-period";
                period.textContent = "/ ay";
                price.appendChild(period);
            }
            info.appendChild(price);

            // Alt satır: Konum ve İncele bağlantısı
            const foot = document.createElement("div");
            foot.className = "lm-card-foot";

            const locText = formatLocation(item);
            const loc = document.createElement("div");
            loc.className = "lm-card-loc";
            loc.textContent = locText;
            if (locText) {
                loc.title = locText;
            }
            foot.appendChild(loc);

            const link = document.createElement("a");
            link.className = "lm-card-link";
            link.href = detailUrl;
            link.textContent = "İncele";
            link.addEventListener("click", e => {
                e.stopPropagation();
            });
            foot.appendChild(link);

            info.appendChild(foot);
            card.appendChild(info);

            return card;
        }

        function formatLocation(item) {
            const neighborhood = (item.neighborhood || "").trim();
            const district = (item.district || "").trim();
            const city = (item.city || "").trim();

            let distCity = "";
            if (district && city) {
                distCity = `${district} / ${city}`;
            } else if (district) {
                distCity = district;
            } else if (city) {
                distCity = city;
            }

            const parts = [];
            if (neighborhood) parts.push(neighborhood);
            if (distCity) parts.push(distCity);

            return parts.join(", ");
        }

        function slugify(text, maxLength) {
            if (maxLength === undefined) {
                maxLength = 80;
            }
            if (!text || !text.trim()) {
                return "";
            }

            var cleaned = text.replace(/['’]/g, "");

            var trMap = {
                'ç': 'c', 'Ç': 'c',
                'ğ': 'g', 'Ğ': 'g',
                'ı': 'i', 'I': 'i', 'İ': 'i', 'i': 'i',
                'ö': 'o', 'Ö': 'o',
                'ş': 's', 'Ş': 's',
                'ü': 'u', 'Ü': 'u'
            };

            var mapped = "";
            for (var i = 0; i < cleaned.length; i++) {
                var ch = cleaned[i];
                mapped += trMap[ch] !== undefined ? trMap[ch] : ch;
            }

            var lower = mapped.toLowerCase();
            var slug = lower.replace(/[^a-z0-9]+/g, "-").replace(/^-+|-+$/g, "");

            if (slug.length > maxLength) {
                var truncated = slug.slice(0, maxLength);
                var lastDash = truncated.lastIndexOf("-");
                if (lastDash > 0) {
                    truncated = truncated.slice(0, lastDash);
                }
                slug = truncated.replace(/-+$/, "");
            }

            return slug;
        }

        function categorySlug(listingTypeId) {
            var id = Number(listingTypeId);
            if (id === 1 || id === 7) return "arsa";
            if (id === 2 || id === 8) return "konut";
            if (id === 3 || id === 9) return "isyeri";
            return "ilan";
        }

        function getDetailUrl(item) {
            var prefix = item.usageTypeId === 2 ? "kiralik" : "satilik";
            var ad = (item.title || "").trim();
            var suffixes = [" - Satılık", " - Kiralık", " - Satilik", " - Kiralik"];
            var removed;
            do {
                removed = false;
                for (var i = 0; i < suffixes.length; i++) {
                    var s = suffixes[i];
                    if (ad.toLowerCase().endsWith(s.toLowerCase())) {
                        ad = ad.slice(0, ad.length - s.length).trim();
                        removed = true;
                        break;
                    }
                }
            } while (removed);

            var rawSlugText = ((item.district || "") + " " + ad).trim();
            var slug = slugify(rawSlugText);
            var id = item.listingId;
            var cat = categorySlug(Number(item.listingTypeId));
            var lastPart = slug ? (slug + "-" + id) : String(id);
            return "/" + prefix + "/" + cat + "/" + lastPart;
        }
    }
})();