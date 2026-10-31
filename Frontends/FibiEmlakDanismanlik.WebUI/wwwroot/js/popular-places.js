(function () {
    'use strict';

    if (window.__fibiPopularPlacesInit) return;
    window.__fibiPopularPlacesInit = true;

    if (typeof window.jQuery === 'undefined' || !window.jQuery.fn) {
        console.error('[PopularPlaces] jQuery bulunamadı.');
        return;
    }

    var $ = window.jQuery;

    var isInitialized = false;

    function syncToUsageType(usageType) {
        try {
            var $root = $('#popularPlacesRoot');
            if (!$root.length) return;

            $root.find('[data-usagetype]').each(function () {
                var $el = $(this);
                if ($el.attr('data-usagetype') === usageType) {
                    $el.show();
                } else {
                    $el.hide();
                }
            });
        } catch (err) {
            console.error('[PopularPlaces] Senkronizasyon hatası:', err);
        }
    }

    function init() {
        if (isInitialized) return;
        isInitialized = true;

        try {
            var $root = $('#popularPlacesRoot');
            if (!$root.length) return;

            // Sekme tıklaması: satın al / kirala
            $(document).off('click.popularPlaces').on('click.popularPlaces', '.search-field .tab-btns .tab-btn', function () {
                try {
                    var usageType = $(this).attr('data-usagetype');
                    if (!usageType) return;
                    syncToUsageType(usageType);
                } catch (err) {
                    console.error('[PopularPlaces] Sekme değişimi hatası:', err);
                }
            });

            // Sayfa yüklenince aktif sekmenin usageType'ına göre ilk senkronizasyon
            var $activeBtn = $('.search-field .tab-btns .tab-btn.active-btn');
            if ($activeBtn.length) {
                var initialType = $activeBtn.attr('data-usagetype');
                if (initialType) {
                    syncToUsageType(initialType);
                }
            }
        } catch (err) {
            console.error('[PopularPlaces] Başlatma hatası:', err);
        }
    }

    if (document.readyState === 'complete') {
        init();
    } else {
        $(window).on('load', init);
    }
})();
