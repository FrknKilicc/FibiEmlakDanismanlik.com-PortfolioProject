(function () {
    'use strict';

    if (window.__fibiHomeSearchInit) return;
    window.__fibiHomeSearchInit = true;

    if (typeof window.jQuery === 'undefined' || !window.jQuery.fn) {
        console.error('[HomeSearch] jQuery bulunamadı.');
        return;
    }

    var $ = window.jQuery;

    $(function () {
        var $root = $('#homeSearchRoot');
        if (!$root.length) return;

        var API = $root.attr('data-api') || '';

        var SORT_OPTIONS = [
            { id: 'CreatedDate|desc', text: 'En Yeni' },
            { id: 'CreatedDate|asc', text: 'En Eski' },
            { id: 'Price|asc', text: 'Fiyat Artan' },
            { id: 'Price|desc', text: 'Fiyat Azalan' }
        ];

        function fetchJson(url) {
            return fetch(url).then(function (r) {
                if (!r.ok) throw new Error('HTTP ' + r.status);
                return r.json();
            });
        }

        function debounce(fn, ms) {
            var t;
            var debounced = function () {
                var c = this, a = arguments;
                clearTimeout(t);
                t = setTimeout(function () { fn.apply(c, a); }, ms);
            };
            debounced.cancel = function () {
                clearTimeout(t);
            };
            return debounced;
        }

        function formatTL(n) {
            return Number(n).toLocaleString('tr-TR') + ' \u20BA';
        }

        function formatM2(n) {
            return Number(n).toLocaleString('tr-TR') + ' m\u00B2';
        }

        function formatSliderText(val1, val2, formatter) {
            var vMin = Math.min(val1, val2);
            var vMax = Math.max(val1, val2);
            return formatter(vMin) + ' - ' + formatter(vMax);
        }

        // Tek Select2 yönetim helper'ı (.empty().append() yasak)
        function setSelectState($select, state) {
            if (!$select || !$select.length) return;
            var opts = state && Array.isArray(state.options) ? state.options : [];
            var ph = (state && state.placeholder != null) ? String(state.placeholder) : '';
            var dis = !!(state && state.disabled);
            var selectedVal = (state && state.value != null) ? String(state.value) : '';

            if ($select.hasClass('select2-hidden-accessible') || $select.data('select2')) {
                $select.select2('destroy');
            }

            var el = $select[0];
            el.innerHTML = '';

            var opt0 = document.createElement('option');
            opt0.value = '';
            opt0.textContent = ph;
            el.appendChild(opt0);

            opts.forEach(function (item) {
                var o = document.createElement('option');
                var val = item.id != null ? item.id : (item.agentId != null ? item.agentId : (item.value != null ? item.value : ''));
                var txt = item.text != null ? item.text : (item.name != null ? item.name : (item.agentName != null ? item.agentName : ''));
                o.value = String(val);
                o.textContent = String(txt);
                el.appendChild(o);
            });

            el.value = selectedVal;
            $select.prop('disabled', dis);

            if ($.fn.select2) {
                var $advPanel = $select.closest('.advanced-search');
                var s2Config = {
                    placeholder: ph,
                    allowClear: true,
                    width: '100%',
                    minimumResultsForSearch: 10
                };
                if ($advPanel.length) {
                    s2Config.dropdownParent = $advPanel;
                }
                $select.select2(s2Config);
                $select.trigger('change.select2');
            }
        }

        function initSlider(sId, dId, min, max, step, vals, fmt, onSlideChange) {
            var $s = $('#' + sId), $d = $('#' + dId);
            if (!$s.length || !$.fn.slider) return;
            if ($s.hasClass('ui-slider')) $s.slider('destroy');
            var safeMin = Number(min) || 0;
            var safeStep = Number(step) || 1;
            var safeMax = Number(max) || 0;
            if (safeMax <= safeMin) {
                safeMax = safeMin + safeStep;
            }
            var v0 = Array.isArray(vals) ? vals[0] : safeMin;
            var v1 = Array.isArray(vals) ? vals[1] : safeMax;
            if (v1 <= v0) v1 = safeMax;

            $s.slider({
                range: true,
                min: safeMin,
                max: safeMax,
                step: safeStep,
                values: [v0, v1],
                slide: function (e, ui) {
                    if (typeof onSlideChange === 'function') onSlideChange();
                    $d.val(formatSliderText(ui.values[0], ui.values[1], fmt));
                }
            });
            $d.val(formatSliderText(v0, v1, fmt));
        }

        function createTabModule(cfg) {
            var ut = cfg.usageType, sfx = cfg.suffix, tabSel = cfg.tabSelector;
            var $tab = $(tabSel);
            var ID = {
                lt: 'listingTypeDropdown-' + sfx,
                city: 'cityDropdown-' + sfx,
                dist: 'districtDropdown-' + sfx,
                search: 'searchInput-' + sfx,
                sugg: 'suggestions-' + sfx,
                sort: 'sortDropdown-' + sfx,
                room: 'roomDropdown-' + sfx,
                roomLabel: 'roomLabel-' + sfx,
                roomContainer: 'roomFilterContainer-' + sfx,
                bath: 'bathDropdown-' + sfx,
                bathLabel: 'bathLabel-' + sfx,
                bathContainer: 'bathFilterContainer-' + sfx,
                floor: 'floorDropdown-' + sfx,
                floorContainer: 'floorFilterContainer-' + sfx,
                agent: 'agentDropdown-' + sfx,
                agentContainer: 'agencyFilterContainer-' + sfx,
                distance: 'distanceDropdown-' + sfx,
                zoning: 'zoningDropdown-' + sfx,
                zoningContainer: 'zoningFilterContainer-' + sfx,
                notice: 'advTypeNotice-' + sfx,
                ps: 'priceSlider-' + sfx,
                pd: 'priceDisplay-' + sfx,
                as: 'areaSlider-' + sfx,
                ad: 'areaDisplay-' + sfx
            };

            // Sekme bazlı artan istek sayaçları (yarış koşulu önleme)
            var reqSeq = {
                listingTypes: 0,
                cities: 0,
                districts: 0,
                filters: 0
            };

            var sliderTouched = {
                price: false,
                area: false
            };

            var defPMax = ut === 1 ? 50000000 : 250000;
            var defPStep = ut === 1 ? 50000 : 1000;
            var defAMax = 5000;

            function resetAdvancedFiltersToS0() {
                $('#' + ID.notice).show();
                $('#' + ID.roomLabel).text('Oda Sayısı');
                $('#' + ID.bathLabel).text('Banyo Sayısı');
                $tab.find('.adv-filter-room').show();
                $tab.find('.adv-filter-bath').show();
                $tab.find('.adv-filter-floor').show();
                $tab.find('.adv-filter-zoning').hide();
                $tab.find('.adv-filter-distance').hide();
                $tab.find('.adv-filter-price').show();
                $tab.find('.adv-filter-area').show();

                setSelectState($('#' + ID.sort), { options: SORT_OPTIONS, placeholder: 'Önce ilan tipi seçin', disabled: true });
                setSelectState($('#' + ID.room), { options: [], placeholder: 'Önce ilan tipi seçin', disabled: true });
                setSelectState($('#' + ID.bath), { options: [], placeholder: 'Önce ilan tipi seçin', disabled: true });
                setSelectState($('#' + ID.floor), { options: [], placeholder: 'Önce ilan tipi seçin', disabled: true });
                setSelectState($('#' + ID.agent), { options: [], placeholder: 'Önce ilan tipi seçin', disabled: true });
                setSelectState($('#' + ID.zoning), { options: [], placeholder: 'Önce ilan tipi seçin', disabled: true });

                sliderTouched.price = false;
                sliderTouched.area = false;
                initSlider(ID.ps, ID.pd, 0, defPMax, defPStep, [0, defPMax], formatTL, function () { sliderTouched.price = true; });
                initSlider(ID.as, ID.ad, 0, defAMax, 10, [0, defAMax], formatM2, function () { sliderTouched.area = true; });
                if ($('#' + ID.ps).hasClass('ui-slider')) $('#' + ID.ps).slider('disable');
                if ($('#' + ID.as).hasClass('ui-slider')) $('#' + ID.as).slider('disable');
                $('#' + ID.pd).val('');
                $('#' + ID.ad).val('');
            }

            function resetToS0() {
                reqSeq.cities++;
                reqSeq.districts++;
                reqSeq.filters++;
                debouncedLoadFilters.cancel();

                setSelectState($('#' + ID.city), { options: [], placeholder: 'Önce ilan tipi seçin', disabled: true });
                setSelectState($('#' + ID.dist), { options: [], placeholder: 'Önce il seçin', disabled: true });
                resetAdvancedFiltersToS0();
            }

            function setAdvancedFiltersLoading() {
                $('#' + ID.notice).hide();
                setSelectState($('#' + ID.sort), { options: SORT_OPTIONS, placeholder: 'Sıralama Seçiniz', disabled: false });
                setSelectState($('#' + ID.room), { options: [], placeholder: 'Yükleniyor...', disabled: true });
                setSelectState($('#' + ID.bath), { options: [], placeholder: 'Yükleniyor...', disabled: true });
                setSelectState($('#' + ID.floor), { options: [], placeholder: 'Yükleniyor...', disabled: true });
                setSelectState($('#' + ID.agent), { options: [], placeholder: 'Yükleniyor...', disabled: true });
                setSelectState($('#' + ID.zoning), { options: [], placeholder: 'Yükleniyor...', disabled: true });
            }

            function loadAdvancedFilters(ltId, cityId, distId) {
                if (!ltId) {
                    resetAdvancedFiltersToS0();
                    return;
                }
                var seq = ++reqSeq.filters;
                setAdvancedFiltersLoading();

                var url = API + 'listingtypes/advanced-filters?listingTypeId=' + encodeURIComponent(ltId) + '&usageType=' + encodeURIComponent(ut);
                if (cityId) url += '&cityId=' + encodeURIComponent(cityId);
                if (distId) url += '&districtId=' + encodeURIComponent(distId);

                fetchJson(url).then(function (f) {
                    if (seq !== reqSeq.filters) return;
                    if (!f) return;

                    var rLabel = f.roomLabel || 'Oda Sayısı';
                    var bLabel = f.bathLabel || 'Banyo Sayısı';
                    $('#' + ID.roomLabel).text(rLabel);
                    $('#' + ID.bathLabel).text(bLabel);

                    $tab.find('.adv-filter-room').toggle(!!f.showRooms);
                    $tab.find('.adv-filter-bath').toggle(!!f.showBath);
                    $tab.find('.adv-filter-floor').toggle(!!f.showFloor);
                    $tab.find('.adv-filter-zoning').toggle(!!f.showZoning);
                    $tab.find('.adv-filter-price').toggle(f.showPrice !== false);
                    $tab.find('.adv-filter-area').toggle(f.showArea !== false);
                    $tab.find('.adv-filter-distance').hide();

                    if (f.showRooms) {
                        var rOpts = Array.isArray(f.roomOptions) ? f.roomOptions : [];
                        setSelectState($('#' + ID.room), {
                            options: rOpts,
                            placeholder: rOpts.length > 0 ? (rLabel + ' Seçiniz') : 'Bu seçimde aktif ilan yok',
                            disabled: rOpts.length === 0
                        });
                    }

                    if (f.showBath) {
                        var bOpts = Array.isArray(f.bathOptions) ? f.bathOptions : [];
                        setSelectState($('#' + ID.bath), {
                            options: bOpts,
                            placeholder: bOpts.length > 0 ? (bLabel + ' Seçiniz') : 'Bu seçimde aktif ilan yok',
                            disabled: bOpts.length === 0
                        });
                    }

                    if (f.showFloor) {
                        var flOpts = Array.isArray(f.floorOptions) ? f.floorOptions : [];
                        setSelectState($('#' + ID.floor), {
                            options: flOpts,
                            placeholder: flOpts.length > 0 ? 'Kat Seçiniz' : 'Bu seçimde aktif ilan yok',
                            disabled: flOpts.length === 0
                        });
                    }

                    if (f.showZoning) {
                        var zOpts = Array.isArray(f.zoningOptions) ? f.zoningOptions : [];
                        setSelectState($('#' + ID.zoning), {
                            options: zOpts,
                            placeholder: zOpts.length > 0 ? 'İmar Durumu Seçiniz' : 'Bu seçimde aktif ilan yok',
                            disabled: zOpts.length === 0
                        });
                    }

                    if (Array.isArray(f.agentOptions)) {
                        setSelectState($('#' + ID.agent), {
                            options: f.agentOptions,
                            placeholder: f.agentOptions.length > 0 ? 'Danışman Seçiniz' : 'Bu seçimde aktif ilan yok',
                            disabled: f.agentOptions.length === 0
                        });
                    } else {
                        fetchJson(API + 'Agent').then(function (agents) {
                            if (seq !== reqSeq.filters) return;
                            var aList = Array.isArray(agents) ? agents : [];
                            setSelectState($('#' + ID.agent), {
                                options: aList,
                                placeholder: aList.length > 0 ? 'Danışman Seçiniz' : 'Bu seçimde aktif ilan yok',
                                disabled: aList.length === 0
                            });
                        }).catch(function (err) {
                            if (seq !== reqSeq.filters) return;
                            console.error('[HomeSearch] Danışman listesi yüklenemedi:', err);
                            setSelectState($('#' + ID.agent), {
                                options: [],
                                placeholder: 'Yüklenemedi, tekrar deneyin',
                                disabled: true
                            });
                        });
                    }

                    sliderTouched.price = false;
                    sliderTouched.area = false;

                    var minP = Number(f.minPrice) || 0;
                    var maxP = Number(f.maxPrice) || 0;
                    var pStep = maxP > 1000000 ? 50000 : (maxP > 10000 ? 1000 : 100);
                    if (maxP <= minP) maxP = minP + pStep;
                    initSlider(ID.ps, ID.pd, minP, maxP, pStep, [minP, maxP], formatTL, function () { sliderTouched.price = true; });
                    if ($('#' + ID.ps).hasClass('ui-slider')) $('#' + ID.ps).slider('enable');

                    var minA = Number(f.minArea) || 0;
                    var maxA = Number(f.maxArea) || 0;
                    var aStep = maxA > 1000 ? 25 : 10;
                    if (maxA <= minA) maxA = minA + aStep;
                    initSlider(ID.as, ID.ad, minA, maxA, aStep, [minA, maxA], formatM2, function () { sliderTouched.area = true; });
                    if ($('#' + ID.as).hasClass('ui-slider')) $('#' + ID.as).slider('enable');
                }).catch(function (err) {
                    if (seq !== reqSeq.filters) return;
                    console.error('[HomeSearch] Gelişmiş filtreler yüklenemedi:', err);
                    setSelectState($('#' + ID.room), { options: [], placeholder: 'Yüklenemedi, tekrar deneyin', disabled: true });
                    setSelectState($('#' + ID.bath), { options: [], placeholder: 'Yüklenemedi, tekrar deneyin', disabled: true });
                    setSelectState($('#' + ID.floor), { options: [], placeholder: 'Yüklenemedi, tekrar deneyin', disabled: true });
                    setSelectState($('#' + ID.agent), { options: [], placeholder: 'Yüklenemedi, tekrar deneyin', disabled: true });
                    setSelectState($('#' + ID.zoning), { options: [], placeholder: 'Yüklenemedi, tekrar deneyin', disabled: true });
                });
            }

            var debouncedLoadFilters = debounce(function (ltId, cityId, distId) {
                loadAdvancedFilters(ltId, cityId, distId);
            }, 180);

            // Başlangıç durumu: S0
            setSelectState($('#' + ID.lt), { options: [], placeholder: 'Yükleniyor...', disabled: true });
            resetToS0();

            // 1) İlan tiplerini yükle
            var ltSeq = ++reqSeq.listingTypes;
            fetchJson(API + 'listingtypes?usageType=' + ut + '&onlyWithActiveListings=true').then(function (d) {
                if (ltSeq !== reqSeq.listingTypes) return;
                var list = Array.isArray(d) ? d : [];
                if (list.length > 0) {
                    setSelectState($('#' + ID.lt), { options: list, placeholder: 'İlan Tipi Seçiniz', disabled: false });
                } else {
                    setSelectState($('#' + ID.lt), { options: [], placeholder: 'Bu seçimde aktif ilan yok', disabled: true });
                }
            }).catch(function (err) {
                if (ltSeq !== reqSeq.listingTypes) return;
                console.error('[HomeSearch] İlan tipleri yüklenemedi:', err);
                setSelectState($('#' + ID.lt), { options: [], placeholder: 'Yüklenemedi, tekrar deneyin', disabled: true });
            });

            // 2) İlan Tipi seçildi / temizlendi (Yalnızca select2:select ve select2:clear)
            $('#' + ID.lt)
                .off('select2:select.homeSearch select2:clear.homeSearch')
                .on('select2:select.homeSearch', function () {
                    var ltId = $(this).val();
                    if (!ltId) {
                        resetToS0();
                        return;
                    }

                    reqSeq.districts++;
                    debouncedLoadFilters.cancel();

                    setSelectState($('#' + ID.city), { options: [], placeholder: 'Yükleniyor...', disabled: true });
                    setSelectState($('#' + ID.dist), { options: [], placeholder: 'Önce il seçin', disabled: true });

                    var cSeq = ++reqSeq.cities;
                    fetchJson(API + 'Location/available-cities?usageType=' + encodeURIComponent(ut) + '&listingTypeId=' + encodeURIComponent(ltId))
                        .then(function (cities) {
                            if (cSeq !== reqSeq.cities) return;
                            var cList = Array.isArray(cities) ? cities : [];
                            if (cList.length > 0) {
                                setSelectState($('#' + ID.city), { options: cList, placeholder: 'İl seçiniz', disabled: false });
                            } else {
                                setSelectState($('#' + ID.city), { options: [], placeholder: 'Bu seçimde aktif ilan yok', disabled: true });
                            }
                        })
                        .catch(function (err) {
                            if (cSeq !== reqSeq.cities) return;
                            console.error('[HomeSearch] İller yüklenemedi:', err);
                            setSelectState($('#' + ID.city), { options: [], placeholder: 'Yüklenemedi, tekrar deneyin', disabled: true });
                        });

                    loadAdvancedFilters(ltId, null, null);
                })
                .on('select2:clear.homeSearch', function () {
                    $(this).val('').trigger('change.select2');
                    resetToS0();
                });

            // 3) İl seçildi / temizlendi (Yalnızca select2:select ve select2:clear)
            $('#' + ID.city)
                .off('select2:select.homeSearch select2:clear.homeSearch')
                .on('select2:select.homeSearch', function () {
                    var cityId = $(this).val();
                    var ltId = $('#' + ID.lt).val();
                    if (!ltId) {
                        resetToS0();
                        return;
                    }
                    if (!cityId) {
                        reqSeq.districts++;
                        setSelectState($('#' + ID.dist), { options: [], placeholder: 'Önce il seçin', disabled: true });
                        debouncedLoadFilters(ltId, null, null);
                        return;
                    }

                    setSelectState($('#' + ID.dist), { options: [], placeholder: 'Yükleniyor...', disabled: true });
                    var dSeq = ++reqSeq.districts;

                    fetchJson(API + 'Location/available-districts?cityId=' + encodeURIComponent(cityId) + '&usageType=' + encodeURIComponent(ut) + '&listingTypeId=' + encodeURIComponent(ltId))
                        .then(function (districts) {
                            if (dSeq !== reqSeq.districts) return;
                            var dList = Array.isArray(districts) ? districts : [];
                            if (dList.length > 0) {
                                setSelectState($('#' + ID.dist), { options: dList, placeholder: 'İlçe seçiniz', disabled: false });
                            } else {
                                // Karar §3.3: İlçesi eşleşmeyen il için disabled + "Tüm ilçeler"
                                setSelectState($('#' + ID.dist), { options: [], placeholder: 'Tüm ilçeler', disabled: true });
                            }
                        })
                        .catch(function (err) {
                            if (dSeq !== reqSeq.districts) return;
                            console.error('[HomeSearch] İlçeler yüklenemedi:', err);
                            setSelectState($('#' + ID.dist), { options: [], placeholder: 'Yüklenemedi, tekrar deneyin', disabled: true });
                        });

                    debouncedLoadFilters(ltId, cityId, null);
                })
                .on('select2:clear.homeSearch', function () {
                    $(this).val('').trigger('change.select2');
                    var ltId = $('#' + ID.lt).val();
                    reqSeq.districts++;
                    setSelectState($('#' + ID.dist), { options: [], placeholder: 'Önce il seçin', disabled: true });
                    if (ltId) {
                        debouncedLoadFilters(ltId, null, null);
                    }
                });

            // 4) İlçe seçildi / temizlendi (Yalnızca select2:select ve select2:clear)
            $('#' + ID.dist)
                .off('select2:select.homeSearch select2:clear.homeSearch')
                .on('select2:select.homeSearch', function () {
                    var distId = $(this).val();
                    var ltId = $('#' + ID.lt).val();
                    var cityId = $('#' + ID.city).val();
                    if (!ltId) return;
                    debouncedLoadFilters(ltId, cityId || null, distId || null);
                })
                .on('select2:clear.homeSearch', function () {
                    $(this).val('').trigger('change.select2');
                    var ltId = $('#' + ID.lt).val();
                    var cityId = $('#' + ID.city).val();
                    if (!ltId) return;
                    debouncedLoadFilters(ltId, cityId || null, null);
                });

            // 5) Mülk Ara - Auto-Suggest (Debounced)
            var hs = debounce(function (e) {
                var $i = $(e.target);
                if (!$i.closest('.tab').hasClass('active-tab')) return;
                var t = $i.val(), $l = $('#' + ID.sugg), lt = $('#' + ID.lt).val() || '';
                if (t.length < 3) { $l.hide().empty(); return; }
                var u = API + 'search/suggestions?usageType=' + ut + '&searchTerm=' + encodeURIComponent(t);
                if (lt) u += '&listingTypeId=' + lt;
                fetchJson(u).then(function (r) {
                    $l.empty();
                    if (Array.isArray(r) && r.length > 0) {
                        r.forEach(function (x) {
                            var $li = $('<li></li>').text(x.title || x.text || x.name);
                            $li.on('click.homeSearch', function () {
                                $i.val(x.title || x.text || x.name);
                                $l.hide();
                                $i.siblings('.clear-search').show();
                            });
                            $l.append($li);
                        });
                        $l.show();
                    } else {
                        $l.hide();
                    }
                }).catch(function (err) {
                    console.error('[HomeSearch] Öneri hatası:', err);
                    $l.hide();
                });
            }, 300);

            $(document)
                .off('keyup.homeSearch_' + sfx, '#' + ID.search)
                .on('keyup.homeSearch_' + sfx, '#' + ID.search, hs);
            $(document)
                .off('input.homeSearch_' + sfx, '#' + ID.search)
                .on('input.homeSearch_' + sfx, '#' + ID.search, function () {
                    $(this).siblings('.clear-search').toggle($(this).val().length > 0);
                });
        }

        // ── Clear Search Input ──
        $(document)
            .off('click.homeSearch', '.clear-search')
            .on('click.homeSearch', '.clear-search', function () {
                var $inp = $(this).siblings('.search-field-input');
                $inp.val('');
                $(this).hide();
                $(this).closest('.form-group').find('.suggestions-list').hide().empty();
            });

        // ── Close suggestions on outside click ──
        $(document)
            .off('click.homeSearchSuggest')
            .on('click.homeSearchSuggest', function (e) {
                if (!$(e.target).closest('.form-group').length) {
                    $('.suggestions-list').hide();
                }
            });

        // ── Gelişmiş Arama Panel Toggle (Tek Handler) ──
        $(document)
            .off('click.homeSearch', '.search__toggler')
            .on('click.homeSearch', '.search__toggler', function (e) {
                e.preventDefault();
                e.stopPropagation();
                var $btn = $(this);
                var $wrapper = $btn.closest('.switch_btn_one');
                var $tab = $btn.closest('.tab');
                var isOpen = $wrapper.hasClass('active');

                $('.switch_btn_one').not($wrapper).removeClass('active')
                    .find('.search__toggler i').removeClass('fa-angle-up').addClass('fa-angle-down');

                if (isOpen) {
                    $wrapper.removeClass('active');
                    $btn.find('i').removeClass('fa-angle-up').addClass('fa-angle-down');
                } else {
                    var $lt = $tab.find('select[id^="listingTypeDropdown-"]');
                    var hasLt = $lt.length && !!$lt.val();
                    $tab.find('.adv-type-notice').toggle(!hasLt);
                    $wrapper.addClass('active');
                    $btn.find('i').removeClass('fa-angle-down').addClass('fa-angle-up');
                }
            });

        // ── Panel X Kapatma ──
        $(document)
            .off('click.homeSearch', '.switch_btn_one .close-side-widget, .switch_btn_one .close-btn')
            .on('click.homeSearch', '.switch_btn_one .close-side-widget, .switch_btn_one .close-btn', function (e) {
                e.preventDefault();
                var $wrapper = $(this).closest('.switch_btn_one');
                $wrapper.removeClass('active');
                $wrapper.find('.search__toggler i').removeClass('fa-angle-up').addClass('fa-angle-down');
            });

        // ── Sekme Değiştiğinde Açık Gelişmiş Arama Panelini Kapat ──
        $(document)
            .off('click.homeSearch', '.search-field .tab-btns .tab-btn')
            .on('click.homeSearch', '.search-field .tab-btns .tab-btn', function () {
                $('.search-field .switch_btn_one').removeClass('active')
                    .find('.search__toggler i').removeClass('fa-angle-up').addClass('fa-angle-down');
            });

        createTabModule({ usageType: 1, suffix: 'forsale', tabSelector: '#tab-1' });
        createTabModule({ usageType: 2, suffix: 'forrent', tabSelector: '#tab-2' });
    });
})();

