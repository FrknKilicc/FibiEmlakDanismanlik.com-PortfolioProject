(function () {
    'use strict';

    if (window.__fibiBestDealsInit) return;
    window.__fibiBestDealsInit = true;

    if (typeof window.jQuery === 'undefined' || !window.jQuery.fn) {
        console.error('[BestDeals] jQuery bulunamadı.');
        return;
    }

    var $ = window.jQuery;

    var isInitialized = false;

    function init() {
        if (isInitialized) return;
        isInitialized = true;

        try {
            var $root = $('#bestDealsRoot');
            if (!$root.length) return;

            $root.find('.best-deals-panel').each(function () {
                var $panel = $(this);
                var $carousel = $panel.find('.deals-carousel');
                if (!$carousel.length) return;

                var originalItems = [];
                var $rawItems = $carousel.find('.owl-item:not(.cloned) > .single-item');
                if (!$rawItems.length) {
                    $rawItems = $carousel.find('.single-item');
                }

                $rawItems.each(function () {
                    var $item = $(this);
                    var itemType = $item.attr('data-type') || '';
                    originalItems.push({
                        type: itemType,
                        html: this.outerHTML
                    });
                });

                $panel.on('click', '.best-deals-filters li', function (e) {
                    e.preventDefault();
                    try {
                        var $chip = $(this);
                        if ($chip.hasClass('active')) return;

                        var filterType = $chip.attr('data-type');
                        var matchedItems = [];

                        if (filterType === '*') {
                            matchedItems = originalItems;
                        } else {
                            matchedItems = originalItems.filter(function (it) {
                                return it.type === filterType;
                            });
                        }

                        if (!matchedItems.length) {
                            return;
                        }

                        $panel.find('.best-deals-filters li').removeClass('active');
                        $chip.addClass('active');

                        var htmlString = matchedItems.map(function (it) {
                            return it.html;
                        }).join('');

                        $carousel.trigger('replace.owl.carousel', [htmlString]).trigger('refresh.owl.carousel');
                    } catch (err) {
                        console.error('[BestDeals] Filtreleme hatası:', err);
                    }
                });
            });

            $(document).off('click.bestDeals').on('click.bestDeals', '.search-field .tab-btns .tab-btn', function () {
                try {
                    var usageType = $(this).attr('data-usagetype');
                    if (!usageType) return;

                    var $targetPanel = $root.find('.best-deals-panel[data-usagetype="' + usageType + '"]');
                    if (!$targetPanel.length) return;

                    $root.find('.best-deals-panel').not($targetPanel).hide();
                    $targetPanel.show();

                    var $targetCarousel = $targetPanel.find('.deals-carousel');
                    if ($targetCarousel.length) {
                        $targetCarousel.trigger('refresh.owl.carousel');
                    }
                } catch (err) {
                    console.error('[BestDeals] Sekme değişimi hatası:', err);
                }
            });
        } catch (err) {
            console.error('[BestDeals] Başlatma hatası:', err);
        }
    }

    if (document.readyState === 'complete') {
        init();
    } else {
        $(window).on('load', init);
    }
})();
