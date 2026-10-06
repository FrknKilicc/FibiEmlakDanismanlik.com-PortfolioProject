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

        function fetchJson(url) {
            return fetch(url).then(function (r) {
                if (!r.ok) throw new Error('HTTP ' + r.status);
                return r.json();
            });
        }

        function debounce(fn, ms) {
            var t;
            return function () {
                var c = this, a = arguments;
                clearTimeout(t);
                t = setTimeout(function () { fn.apply(c, a); }, ms);
            };
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

        function initSelect($el, ph, opts) {
            if (!$el || !$el.length || !$.fn.select2) return;
            if ($el.hasClass('select2-hidden-accessible')) {
                $el.select2('destroy');
            }
            var $advPanel = $el.closest('.advanced-search');
            var s2Opts = $.extend({
                placeholder: ph,
                allowClear: true,
                width: '100%',
                minimumResultsForSearch: 10
            }, opts || {});
            if ($advPanel.length) {
                s2Opts.dropdownParent = $advPanel;
            }
            $el.select2(s2Opts);
        }

        function fillSelect($el, data, ph, dis) {
            if (!$el || !$el.length) return;
            var el = $el[0];
            if ($el.hasClass('select2-hidden-accessible')) {
                $el.select2('destroy');
            }
            el.innerHTML = '';
            var opt0 = document.createElement('option');
            opt0.value = '';
            opt0.textContent = ph;
            el.appendChild(opt0);

            if (Array.isArray(data)) {
                data.forEach(function (item) {
                    var o = document.createElement('option');
                    o.value = item.id != null ? item.id : (item.agentId != null ? item.agentId : (item.value || ''));
                    o.textContent = item.text || item.name || item.agentName || '';
                    el.appendChild(o);
                });
            }
            $el.prop('disabled', !!dis);
            initSelect($el, ph);
            $el.trigger('change.select2');
        }

        function initSlider(sId, dId, min, max, step, vals, fmt) {
            var $s = $('#' + sId), $d = $('#' + dId);
            if (!$s.length || !$.fn.slider) return;
            if ($s.hasClass('ui-slider')) $s.slider('destroy');
            $s.slider({
                range: true,
                min: min,
                max: max,
                step: step,
                values: vals,
                slide: function (e, ui) {
                    $d.val(formatSliderText(ui.values[0], ui.values[1], fmt));
                }
            });
            $d.val(formatSliderText(vals[0], vals[1], fmt));
        }

        function createTabModule(cfg) {
            var ut = cfg.usageType, sfx = cfg.suffix, tabSel = cfg.tabSelector;
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
                bathContainer: 'bathFilterContainer-' + sfx,
                floor: 'floorDropdown-' + sfx,
                agent: 'agentDropdown-' + sfx,
                distance: 'distanceDropdown-' + sfx,
                zoning: 'zoningDropdown-' + sfx,
                notice: 'advTypeNotice-' + sfx,
                ps: 'priceSlider-' + sfx,
                pd: 'priceDisplay-' + sfx,
                as: 'areaSlider-' + sfx,
                ad: 'areaDisplay-' + sfx
            };

            var $sort = $('#' + ID.sort);
            var sOpts = [];
            $sort.find('option').each(function () {
                if (this.value) sOpts.push({ id: this.value, text: $(this).text() });
            });
            fillSelect($sort, sOpts, 'Sıralama Seçin', true);

            fillSelect($('#' + ID.lt), [], 'Yükleniyor...', true);
            fillSelect($('#' + ID.city), [], 'Önce ilan tipi seçin', true);
            fillSelect($('#' + ID.dist), [], 'Önce il seçin', true);
            fillSelect($('#' + ID.room), [], 'Önce ilan tipi seçin', true);
            fillSelect($('#' + ID.bath), [], 'Önce ilan tipi seçin', true);
            fillSelect($('#' + ID.floor), [], 'Önce ilan tipi seçin', true);
            fillSelect($('#' + ID.agent), [], 'Önce ilan tipi seçin', true);
            fillSelect($('#' + ID.zoning), [], 'Önce ilan tipi seçin', true);

            $('#' + ID.notice).show();

            var defPMax = ut === 1 ? 50000000 : 250000;
            var defPStep = ut === 1 ? 50000 : 1000;
            var defAMax = 5000;
            initSlider(ID.ps, ID.pd, 0, defPMax, defPStep, [0, defPMax], formatTL);
            initSlider(ID.as, ID.ad, 0, defAMax, 10, [0, defAMax], formatM2);
            $('#' + ID.ps).slider('disable');
            $('#' + ID.as).slider('disable');
            $('#' + ID.pd).val('');
            $('#' + ID.ad).val('');

            fetchJson(API + 'listingtypes?usageType=' + ut).then(function (d) {
                fillSelect($('#' + ID.lt), d, 'İlan Tipi Seçiniz', false);
            }).catch(function (err) {
                console.error('[HomeSearch] İlan tipleri yüklenemedi:', err);
                fillSelect($('#' + ID.lt), [], 'Yüklenemedi, tekrar deneyin', true);
            });

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

