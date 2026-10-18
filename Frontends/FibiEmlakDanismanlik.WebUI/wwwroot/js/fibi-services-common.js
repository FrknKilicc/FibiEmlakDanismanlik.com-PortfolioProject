(function () {
    'use strict';

    if (window.FibiServices) {
        return;
    }

    var cachedLocations = null;
    var locationPromise = null;
    var _leadContext = {
        neighborhoodId: null,
        summary: null
    };

    /**
     * Kök elemandan API taban adresini okur.
     */
    function apiUrl() {
        var root = document.getElementById('fibiServiceRoot');
        var url = root ? (root.getAttribute('data-api') || '') : '';
        if (url && !url.endsWith('/')) {
            url += '/';
        }
        return url;
    }

    /**
     * Kök elemandan aktif modül kimliğini okur.
     */
    function moduleId() {
        var root = document.getElementById('fibiServiceRoot');
        var raw = root ? root.getAttribute('data-module-id') : '0';
        var parsed = parseInt(raw, 10);
        return isNaN(parsed) ? 0 : parsed;
    }

    /**
     * DOM elemanı veya jQuery nesnesini saf HTMLElement'e dönüştürür.
     */
    function toElement(el) {
        if (!el) return null;
        if (typeof el === 'string') return document.querySelector(el);
        if (window.jQuery && el instanceof window.jQuery) return el[0];
        return el;
    }

    /**
     * Select üzerinde niceSelect eklentisini günceller.
     */
    function updateNiceSelect(el) {
        if (window.jQuery && window.jQuery.fn && window.jQuery.fn.niceSelect) {
            window.jQuery(el).niceSelect('update');
        }
    }

    /**
     * XSS önleme amaçlı metin kaçış yardımcısı.
     */
    function escapeHtml(str) {
        if (str == null) return '';
        var div = document.createElement('div');
        div.textContent = String(str);
        return div.innerHTML;
    }

    /**
     * Emsal verisi olan şehir, ilçe ve mahalle hiyerarşisini çeker ve önbellekler.
     */
    function loadLocations() {
        if (cachedLocations !== null) {
            return Promise.resolve(cachedLocations);
        }
        if (locationPromise !== null) {
            return locationPromise;
        }

        var url = apiUrl() + 'FibiServices/locations';
        locationPromise = fetch(url)
            .then(function (res) {
                if (!res.ok) {
                    throw new Error('HTTP ' + res.status);
                }
                return res.json();
            })
            .then(function (data) {
                cachedLocations = Array.isArray(data) ? data : [];
                locationPromise = null;
                return cachedLocations;
            })
            .catch(function (err) {
                locationPromise = null;
                console.error('[FibiServices] Lokasyonlar alınamadı:', err);
                throw err;
            });

        return locationPromise;
    }

    /**
     * Select elemanını sıfırlar ve ilk placeholder seçeneğini ekler.
     */
    function clearSelect(selectEl, placeholderText) {
        selectEl.options.length = 0;
        selectEl.add(new Option(placeholderText, ''));
        selectEl.value = '';
        updateNiceSelect(selectEl);
    }

    /**
     * İl, İlçe ve Mahalle seçimlerini birbirine bağlar (kademeli doldurma).
     */
    function bindLocationCascade(opts) {
        var cityEl = toElement(opts.city);
        var distEl = toElement(opts.district);
        var neighEl = toElement(opts.neighborhood);

        if (!cityEl) return Promise.resolve();

        clearSelect(cityEl, 'İl seçin');
        if (distEl) clearSelect(distEl, 'İlçe seçin');
        if (neighEl) clearSelect(neighEl, 'Mahalle seçin');

        return loadLocations().then(function (cities) {
            cityEl.options.length = 0;
            cityEl.add(new Option('İl seçin', ''));

            cities.forEach(function (c) {
                cityEl.add(new Option(c.cityName, String(c.cityId)));
            });

            if (cities.length === 1) {
                cityEl.value = String(cities[0].cityId);
                populateDistricts(cities[0]);
            }
            updateNiceSelect(cityEl);

            function onCityChanged() {
                var selectedCityId = cityEl.value;
                if (!selectedCityId) {
                    if (distEl) clearSelect(distEl, 'İlçe seçin');
                    if (neighEl) clearSelect(neighEl, 'Mahalle seçin');
                    return;
                }

                var cityObj = cities.find(function (c) {
                    return String(c.cityId) === String(selectedCityId);
                });
                populateDistricts(cityObj);
            }

            if (window.jQuery) {
                window.jQuery(cityEl).off('change.fibiCascade').on('change.fibiCascade', onCityChanged);
            } else {
                cityEl.addEventListener('change', onCityChanged);
            }

            function populateDistricts(cityObj) {
                if (!distEl) return;
                distEl.options.length = 0;
                distEl.add(new Option('İlçe seçin', ''));
                if (neighEl) clearSelect(neighEl, 'Mahalle seçin');

                var distList = (cityObj && Array.isArray(cityObj.districts)) ? cityObj.districts : [];
                distList.forEach(function (d) {
                    distEl.add(new Option(d.districtName, String(d.districtId)));
                });

                if (distList.length === 1) {
                    distEl.value = String(distList[0].districtId);
                    populateNeighborhoods(distList[0]);
                }
                updateNiceSelect(distEl);
            }

            function onDistrictChanged() {
                if (!distEl) return;
                var selectedDistId = distEl.value;
                if (!selectedDistId) {
                    if (neighEl) clearSelect(neighEl, 'Mahalle seçin');
                    return;
                }

                var selectedCityId = cityEl.value;
                var currentCity = cities.find(function (c) {
                    return String(c.cityId) === String(selectedCityId);
                });
                var distObj = currentCity && currentCity.districts
                    ? currentCity.districts.find(function (d) { return String(d.districtId) === String(selectedDistId); })
                    : null;

                populateNeighborhoods(distObj);
            }

            if (distEl) {
                if (window.jQuery) {
                    window.jQuery(distEl).off('change.fibiCascade').on('change.fibiCascade', onDistrictChanged);
                } else {
                    distEl.addEventListener('change', onDistrictChanged);
                }
            }

            function populateNeighborhoods(distObj) {
                if (!neighEl) return;
                neighEl.options.length = 0;
                neighEl.add(new Option('Mahalle seçin', ''));

                var neighList = (distObj && Array.isArray(distObj.neighborhoods)) ? distObj.neighborhoods : [];
                neighList.forEach(function (n) {
                    neighEl.add(new Option(n.neighborhoodName, String(n.neighborhoodId)));
                });
                updateNiceSelect(neighEl);
            }
        });
    }

    /**
     * Tüm mahalleleri ilçe bazlı optgroup ile doldurur (karşılaştırma modu için).
     */
    function fillNeighborhoodSelect(select, opts) {
        var selectEl = toElement(select);
        if (!selectEl) return Promise.resolve();

        opts = opts || {};
        var exclude = opts.excludeId != null ? String(opts.excludeId) : null;
        var ph = opts.placeholder || 'Mahalle seçin';

        selectEl.options.length = 0;
        selectEl.add(new Option(ph, ''));

        return loadLocations().then(function (cities) {
            cities.forEach(function (city) {
                if (!Array.isArray(city.districts)) return;
                city.districts.forEach(function (dist) {
                    if (!Array.isArray(dist.neighborhoods)) return;
                    var eligible = dist.neighborhoods.filter(function (n) {
                        return exclude === null || String(n.neighborhoodId) !== exclude;
                    });
                    if (eligible.length === 0) return;

                    var optgroup = document.createElement('optgroup');
                    optgroup.label = dist.districtName;

                    eligible.forEach(function (n) {
                        var opt = new Option(n.neighborhoodName, String(n.neighborhoodId));
                        optgroup.appendChild(opt);
                    });

                    selectEl.appendChild(optgroup);
                });
            });
            updateNiceSelect(selectEl);
        });
    }

    /**
     * Mahalle kimliğine göre il ve ilçe bilgilerini bulur.
     */
    function findLocation(neighborhoodId) {
        if (neighborhoodId == null || !cachedLocations) return null;
        var targetId = String(neighborhoodId);

        for (var i = 0; i < cachedLocations.length; i++) {
            var city = cachedLocations[i];
            if (!city.districts) continue;
            for (var j = 0; j < city.districts.length; j++) {
                var dist = city.districts[j];
                if (!dist.neighborhoods) continue;
                for (var k = 0; k < dist.neighborhoods.length; k++) {
                    var neigh = dist.neighborhoods[k];
                    if (String(neigh.neighborhoodId) === targetId) {
                        return {
                            cityId: city.cityId,
                            cityName: city.cityName,
                            districtId: dist.districtId,
                            districtName: dist.districtName,
                            neighborhoodId: neigh.neighborhoodId,
                            neighborhoodName: neigh.neighborhoodName
                        };
                    }
                }
            }
        }
        return null;
    }

    /**
     * API hata durumlarını kullanıcıya toastr ile Türkçe gösterir.
     */
    function handleApiError(status, data) {
        if (!window.toastr) return;

        if (status === 400) {
            var msgs = [];
            if (data && typeof data === 'object' && data.errors) {
                for (var key in data.errors) {
                    if (Object.prototype.hasOwnProperty.call(data.errors, key)) {
                        var val = data.errors[key];
                        if (Array.isArray(val)) {
                            for (var i = 0; i < val.length; i++) {
                                msgs.push(escapeHtml(val[i]));
                            }
                        } else if (val) {
                            msgs.push(escapeHtml(val));
                        }
                    }
                }
            }

            if (msgs.length > 0) {
                window.toastr.error(msgs.join('<br>'));
            } else {
                var title = (data && (data.title || data.detail)) || 'Geçersiz istek gönderildi.';
                window.toastr.error(escapeHtml(title));
            }
        } else if (status === 404) {
            var msg404 = typeof data === 'string'
                ? data
                : ((data && (data.title || data.detail)) || 'İstenen kayıt bulunamadı.');
            window.toastr.error(escapeHtml(msg404));
        } else if (status === 429) {
            window.toastr.error('Çok fazla talep gönderdiniz, lütfen birkaç dakika sonra tekrar deneyin.');
        } else {
            var generalMsg = typeof data === 'string'
                ? data
                : ((data && (data.title || data.detail)) || 'İşlem sırasında beklenmeyen bir hata oluştu.');
            window.toastr.error(escapeHtml(generalMsg));
        }
    }

    /**
     * JSON gövdeli POST isteği gönderir ve yanıtı işler.
     */
    function postJson(path, body) {
        var base = apiUrl();
        var cleanPath = path.startsWith('/') ? path.substring(1) : path;
        var url = path.indexOf('http') === 0 ? path : (base + cleanPath);

        return fetch(url, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Accept': 'application/json, text/plain, */*'
            },
            body: JSON.stringify(body)
        })
        .then(function (res) {
            var contentType = res.headers.get('content-type') || '';
            var parsePromise = contentType.indexOf('application/json') !== -1 || contentType.indexOf('application/problem+json') !== -1
                ? res.json().catch(function () { return null; })
                : res.text();

            return parsePromise.then(function (data) {
                var result = {
                    ok: res.ok,
                    status: res.status,
                    data: data
                };

                if (!res.ok) {
                    handleApiError(res.status, data);
                }

                return result;
            });
        })
        .catch(function (err) {
            console.error('[FibiServices] Ağ hatası:', err);
            if (window.toastr) {
                window.toastr.error('Sunucuya ulaşılamadı. Lütfen daha sonra tekrar deneyin.');
            }
            return {
                ok: false,
                status: 0,
                data: null,
                error: err
            };
        });
    }

    /**
     * TL para birimi formatı: "1.250.000 ₺"
     */
    function formatTL(value) {
        var num = Number(value);
        if (isNaN(num)) return '0 ₺';
        return new Intl.NumberFormat('tr-TR', { maximumFractionDigits: 0 }).format(num) + ' ₺';
    }

    /**
     * Ondalık basamaklı sayı formatı
     */
    function formatNumber(value, digits) {
        var num = Number(value);
        if (isNaN(num)) return '0';
        var d = digits != null ? digits : 0;
        return new Intl.NumberFormat('tr-TR', { minimumFractionDigits: d, maximumFractionDigits: d }).format(num);
    }

    /**
     * Yüzde formatı: "%36,0"
     */
    function formatPercent(value) {
        var num = Number(value);
        if (isNaN(num)) return '%0,0';
        return '%' + new Intl.NumberFormat('tr-TR', { minimumFractionDigits: 1, maximumFractionDigits: 1 }).format(num);
    }

    /**
     * Mahalle/ilçe lokasyonuna ait ilanlar sayfasının URL'ini üretir.
     */
    function buildListingsUrl(location) {
        if (!location) {
            return '/AllForSalePage';
        }
        var params = new URLSearchParams();
        if (location.cityId != null) params.set('cityId', String(location.cityId));
        if (location.cityName) params.set('city', String(location.cityName));
        if (location.districtId != null) params.set('districtId', String(location.districtId));
        if (location.districtName) params.set('district', String(location.districtName));

        var qs = params.toString();
        return '/AllForSalePage' + (qs ? ('?' + qs) : '');
    }

    /**
     * Analiz sonucunu lead formu bağlamına aktarır ve formu görünür kılar.
     */
    function setLeadContext(neighborhoodId, summaryObject) {
        _leadContext.neighborhoodId = neighborhoodId != null ? parseInt(neighborhoodId, 10) : null;
        _leadContext.summary = summaryObject != null ? summaryObject : null;

        var leadSec = document.getElementById('fibiLeadSection');
        if (leadSec) {
            leadSec.removeAttribute('hidden');
            leadSec.hidden = false;
            leadSec.style.display = 'block';
        }
    }

    /**
     * Sayfayı lead formuna kaydırır ve ilk alana odaklanır.
     */
    function scrollToLead() {
        var leadSec = document.getElementById('fibiLeadSection');
        if (leadSec) {
            leadSec.scrollIntoView({ behavior: 'smooth', block: 'start' });
            setTimeout(function () {
                var firstInput = leadSec.querySelector('input:not([type="hidden"]):not([disabled]), textarea, select');
                if (firstInput) {
                    firstInput.focus();
                }
            }, 400);
        }
    }

    /**
     * Seçim kartlarını (:has fallback ve is-checked yönetimi) başlatır.
     */
    function initChoiceCards(container) {
        var root = container || document;
        var cards = root.querySelectorAll('.fibi-radio-card, .fibi-toggle-card');

        function syncState(card) {
            var input = card.querySelector('input[type="radio"], input[type="checkbox"]');
            if (!input) return;
            if (input.checked) {
                card.classList.add('is-checked');
            } else {
                card.classList.remove('is-checked');
            }
        }

        cards.forEach(function (card) {
            syncState(card);

            var input = card.querySelector('input[type="radio"], input[type="checkbox"]');
            if (!input || input.__fibiChoiceBound) return;
            input.__fibiChoiceBound = true;

            input.addEventListener('change', function () {
                if (input.type === 'radio' && input.name) {
                    var groupRadios = (card.form || document).querySelectorAll('input[type="radio"][name="' + input.name + '"]');
                    groupRadios.forEach(function (r) {
                        var parentCard = r.closest('.fibi-radio-card');
                        if (parentCard) {
                            if (r.checked) parentCard.classList.add('is-checked');
                            else parentCard.classList.remove('is-checked');
                        }
                    });
                } else {
                    syncState(card);
                }
            });
        });
    }

    /**
     * Range input değerini rozet elemanına canlı yazar.
     */
    function initRange(input, output, formatter) {
        var inputEl = toElement(input);
        var outputEl = toElement(output);
        if (!inputEl || !outputEl) return;

        function update() {
            var val = inputEl.value;
            if (typeof formatter === 'function') {
                outputEl.textContent = formatter(val);
            } else {
                outputEl.textContent = val;
            }
        }

        inputEl.addEventListener('input', update);
        inputEl.addEventListener('change', update);
        update();
    }

    /**
     * Lead talep formunun gönderim olayını bağlar.
     */
    function bindLeadForm() {
        var form = document.getElementById('fibiLeadForm');
        if (!form || form.__fibiBound) return;
        form.__fibiBound = true;

        form.addEventListener('submit', function (e) {
            e.preventDefault();

            if (typeof form.reportValidity === 'function' && !form.reportValidity()) {
                return;
            }

            var submitBtn = form.querySelector('button[type="submit"]');
            var fullNameVal = form.elements['fullName'] ? form.elements['fullName'].value.trim() : '';
            var phoneVal = form.elements['phoneNumber'] ? form.elements['phoneNumber'].value.trim() : '';
            var emailRaw = form.elements['email'] ? form.elements['email'].value.trim() : '';
            var emailVal = emailRaw.length > 0 ? emailRaw : null;
            var noteRaw = form.elements['customerNote'] ? form.elements['customerNote'].value.trim() : '';
            var noteVal = noteRaw.length > 0 ? noteRaw : null;
            var kvkkVal = form.elements['kvkkConsent'] ? form.elements['kvkkConsent'].checked : false;

            var payload = {
                sourceModule: moduleId(),
                fullName: fullNameVal,
                phoneNumber: phoneVal,
                email: emailVal,
                neighborhoodId: _leadContext.neighborhoodId,
                calculationSummaryJson: _leadContext.summary ? JSON.stringify(_leadContext.summary) : null,
                customerNote: noteVal,
                kvkkConsent: kvkkVal
            };

            if (submitBtn) {
                submitBtn.classList.add('fibi-loading');
                submitBtn.disabled = true;
            }

            postJson('FibiServices/leads', payload)
                .then(function (res) {
                    if (res.ok) {
                        var successMsg = typeof res.data === 'string' && res.data.length > 0
                            ? res.data
                            : 'Talebiniz başarıyla alınmıştır. Uzmanımız en kısa sürede sizinle iletişime geçecektir.';
                        if (window.toastr) {
                            window.toastr.success(successMsg);
                        }
                        form.reset();
                    }
                })
                .finally(function () {
                    if (submitBtn) {
                        submitBtn.classList.remove('fibi-loading');
                        submitBtn.disabled = false;
                    }
                });
        });
    }

    // Sayfa hazır olduğunda genel seçim kartlarını ve lead formunu başlat
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', function () {
            initChoiceCards(document);
            bindLeadForm();
        });
    } else {
        initChoiceCards(document);
        bindLeadForm();
    }

    // Ortak modül API'sini pencere nesnesine aç
    window.FibiServices = {
        apiUrl: apiUrl,
        moduleId: moduleId,
        loadLocations: loadLocations,
        bindLocationCascade: bindLocationCascade,
        fillNeighborhoodSelect: fillNeighborhoodSelect,
        findLocation: findLocation,
        postJson: postJson,
        formatTL: formatTL,
        formatNumber: formatNumber,
        formatPercent: formatPercent,
        buildListingsUrl: buildListingsUrl,
        setLeadContext: setLeadContext,
        scrollToLead: scrollToLead,
        initChoiceCards: initChoiceCards,
        initRange: initRange
    };
})();
