/**
 * Fibi Emlak Danışmanlık - İlan Detay Gelişmiş Galeri Yönetimi
 * Owl Carousel ve Fancybox 3 entegrasyonu
 */
(function ($) {
    'use strict';

    $(function () {
        // Owl Carousel veya Fancybox kütüphaneleri mevcut değilse sessizce çık
        if (typeof $ === 'undefined' || !$.fn || !$.fn.owlCarousel || !$.fancybox) {
            return;
        }

        $('.listing-gallery').each(function () {
            var $gallery = $(this);
            var $dataScript = $gallery.find('.listing-gallery-data');
            if (!$dataScript.length) {
                return;
            }

            var imageUrls = [];
            try {
                imageUrls = JSON.parse($dataScript.html() || '[]');
            } catch (e) {
                imageUrls = [];
            }

            if (!imageUrls || !imageUrls.length) {
                return;
            }

            var totalCount = imageUrls.length;
            var galleryTitle = $gallery.data('gallery-title') || '';
            var $carousel = $gallery.find('.single-item-carousel');
            var $countBadge = $gallery.find('.listing-gallery-count');
            var $thumbs = $gallery.find('.listing-gallery-thumbs');
            var $thumbButtons = $thumbs.find('button');
            var currentIndex = 0;

            // Fancybox öğeleri listesi
            var fancyboxItems = imageUrls.map(function (url, i) {
                var caption = galleryTitle
                    ? galleryTitle + ' (' + (i + 1) + '/' + totalCount + ')'
                    : '(' + (i + 1) + '/' + totalCount + ')';

                return {
                    src: url,
                    opts: {
                        caption: caption
                    }
                };
            });

            function updateGalleryState(index) {
                currentIndex = index;

                // Rozeti "i+1 / N" güncelle
                if ($countBadge.length) {
                    $countBadge.html('<i class="fas fa-camera"></i> ' + (index + 1) + ' / ' + totalCount);
                }

                // Aktif küçük resmi işaretle ve şeridi görünür alana kaydır
                if ($thumbButtons.length) {
                    $thumbButtons.removeClass('is-active');
                    var $activeBtn = $thumbButtons.filter('[data-index="' + index + '"]');
                    $activeBtn.addClass('is-active');

                    var activeBtnEl = $activeBtn[0];
                    var thumbsEl = $thumbs[0];
                    if (activeBtnEl && thumbsEl) {
                        var targetLeft = activeBtnEl.offsetLeft - (thumbsEl.clientWidth / 2) + (activeBtnEl.offsetWidth / 2);
                        if (typeof thumbsEl.scrollTo === 'function') {
                            thumbsEl.scrollTo({
                                left: targetLeft,
                                behavior: 'smooth'
                            });
                        } else {
                            thumbsEl.scrollLeft = targetLeft;
                        }
                    }
                }
            }

            // Carousel başlatılmadan önce olay dinleyicisini bağla
            $carousel.on('changed.owl.carousel', function (event) {
                if (!event.relatedTarget || typeof event.item === 'undefined' || event.item === null) {
                    return;
                }
                var realIndex = event.relatedTarget.relative(event.item.index);
                if (typeof realIndex === 'number' && !isNaN(realIndex)) {
                    updateGalleryState(realIndex);
                }
            });

            // Küçük resme tıklayınca ilgili görsele geç
            $gallery.on('click', '.listing-gallery-thumbs button', function (e) {
                e.preventDefault();
                var idx = parseInt($(this).attr('data-index'), 10);
                if (!isNaN(idx)) {
                    $carousel.trigger('to.owl.carousel', [idx, 300]);
                }
            });

            function openFancybox(startIndex) {
                var targetIndex = (typeof startIndex === 'number' && !isNaN(startIndex)) ? startIndex : currentIndex;
                if (targetIndex < 0 || targetIndex >= totalCount) {
                    targetIndex = 0;
                }

                $.fancybox.open(fancyboxItems, {
                    index: targetIndex,
                    loop: true,
                    buttons: ["zoom", "slideShow", "thumbs", "close"]
                });
            }

            // Tam ekran butonu tıklaması
            $gallery.on('click', '.listing-gallery-fullscreen', function (e) {
                e.preventDefault();
                e.stopPropagation();
                openFancybox(currentIndex);
            });

            // Carousel içindeki görsele tıklama: Fancybox'ı aç ve tema olayının çift tetiklenmesini engelle
            $gallery.on('click', '.carousel-inner .lightbox-image', function (e) {
                e.preventDefault();
                e.stopPropagation();

                var clickedIndex = $(this).attr('data-gallery-index');
                var targetIndex = (typeof clickedIndex !== 'undefined' && clickedIndex !== null && clickedIndex !== '')
                    ? parseInt(clickedIndex, 10)
                    : currentIndex;

                openFancybox(isNaN(targetIndex) ? currentIndex : targetIndex);
            });
        });
    });
})(jQuery);
