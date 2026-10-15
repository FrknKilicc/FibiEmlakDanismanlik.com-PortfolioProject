(function($) {
	
	"use strict";
	
	//Hide Loading Box (Preloader)
	function handlePreloader() {
		if($('.loader-wrap').length){
			$('.loader-wrap').delay(1000).fadeOut(500);
		}
	}

	if ($(".preloader-close").length) {
        $(".preloader-close").on("click", function(){
            $('.loader-wrap').delay(200).fadeOut(500);
        })
    }

    // Advanced search toggle is handled by the search component in Default.cshtml
	
	//Update Header Style and Scroll to Top
	function headerStyle() {
		if($('.main-header').length){
			var windowpos = $(window).scrollTop();
			var siteHeader = $('.main-header');
			var scrollLink = $('.scroll-top');
			if (windowpos >= 110) {
				siteHeader.addClass('fixed-header');
				scrollLink.addClass('open');
			} else {
				siteHeader.removeClass('fixed-header');
				scrollLink.removeClass('open');
			}
		}
	}
	
	headerStyle();


	//Submenu Dropdown Toggle
	if($('.main-header li.dropdown ul').length){
		$('.main-header .navigation li.dropdown').append('<div class="dropdown-btn"><span class="fas fa-angle-down"></span></div>');
		
	}

	//Mobile Nav Hide Show
	if($('.mobile-menu').length){
		
		$('.mobile-menu .menu-box').mCustomScrollbar();
		
		var mobileMenuContent = $('.main-header .menu-area .main-menu').html();
		$('.mobile-menu .menu-box .menu-outer').append(mobileMenuContent);
		$('.sticky-header .main-menu').append(mobileMenuContent);
		
		//Dropdown Button
		$('.mobile-menu li.dropdown .dropdown-btn').on('click', function() {
			$(this).toggleClass('open');
			$(this).prev('ul').slideToggle(500);
		});
		//Dropdown Button
		$('.mobile-menu li.dropdown .dropdown-btn').on('click', function() {
			$(this).prev('.megamenu').slideToggle(900);
		});
		//Menu Toggle Btn
		$('.mobile-nav-toggler').on('click', function() {
			$('body').addClass('mobile-menu-visible');
		});

		//Menu Toggle Btn
		$('.mobile-menu .menu-backdrop,.mobile-menu .close-btn').on('click', function() {
			$('body').removeClass('mobile-menu-visible');
		});
	}


	// Scroll to a Specific Div
	if($('.scroll-to-target').length){
		$(".scroll-to-target").on('click', function() {
			var target = $(this).attr('data-target');
		   // animate
		   $('html, body').animate({
			   scrollTop: $(target).offset().top
			 }, 1000);
	
		});
	}

	// Elements Animation
	if($('.wow').length){
		var wow = new WOW({
		mobile:       false
		});
		wow.init();
	}

	//Contact Form Validation
	if($('#contact-form').length){
		$('#contact-form').validate({
			rules: {
				username: {
					required: true
				},
				email: {
					required: true,
					email: true
				},
				phone: {
					required: true
				},
				subject: {
					required: true
				},
				message: {
					required: true
				}
			}
		});
	}

	//Fact Counter + Text Count
	if($('.count-box').length){
		$('.count-box').appear(function(){
	
			var $t = $(this),
				n = $t.find(".count-text").attr("data-stop"),
				r = parseInt($t.find(".count-text").attr("data-speed"), 10);
				
			if (!$t.hasClass("counted")) {
				$t.addClass("counted");
				$({
					countNum: $t.find(".count-text").text()
				}).animate({
					countNum: n
				}, {
					duration: r,
					easing: "linear",
					step: function() {
						$t.find(".count-text").text(Math.floor(this.countNum));
					},
					complete: function() {
						$t.find(".count-text").text(this.countNum);
					}
				});
			}
			
		},{accY: 0});
	}


	//Tabs Box
	if($('.tabs-box').length){
		$('.tabs-box .tab-buttons .tab-btn').on('click', function(e) {
			e.preventDefault();
			var target = $($(this).attr('data-tab'));
			
			if ($(target).is(':visible')){
				return false;
			}else{
				target.parents('.tabs-box').find('.tab-buttons').find('.tab-btn').removeClass('active-btn');
				$(this).addClass('active-btn');
				target.parents('.tabs-box').find('.tabs-content').find('.tab').fadeOut(0);
				target.parents('.tabs-box').find('.tabs-content').find('.tab').removeClass('active-tab');
				$(target).fadeIn(300);
				$(target).addClass('active-tab');
			}
		});
	}
	//Accordion Box
	if($('.accordion-box').length){
		$(".accordion-box").on('click', '.acc-btn', function() {
			
			var outerBox = $(this).parents('.accordion-box');
			var target = $(this).parents('.accordion');
			
			if($(this).hasClass('active')!==true){
				$(outerBox).find('.accordion .acc-btn').removeClass('active');
			}
			
			if ($(this).next('.acc-content').is(':visible')){
				return false;
			}else{
				$(this).addClass('active');
				$(outerBox).children('.accordion').removeClass('active-block');
				$(outerBox).find('.accordion').children('.acc-content').slideUp(300);
				target.addClass('active-block');
				$(this).next('.acc-content').slideDown(300);	
			}
		});	
	}
    //two-column-carousel
	if ($('.two-column-carousel').length) {
		$('.two-column-carousel').owlCarousel({
			loop:true,
			margin:30,
			nav:true,
			smartSpeed: 1000,
			autoplay: false,
			fibiDeferredAutoplay: true,
			autoplayTimeout: 5000,
			navText: [ '<span class="fas fa-algle-left"></span>', '<span class="fas fa-algle-left-right"></span>' ],
			responsive:{
				0:{
					items:1
				},
				480:{
					items:1
				},
				600:{
					items:1
				},
				800:{
					items:2
				},
				1024:{
					items:2
				}
			}
		});    		
	}

    //three-item-carousel
	if ($('.three-item-carousel').length) {
		$('.three-item-carousel').owlCarousel({
			loop:true,
			margin:30,
			nav:true,
			smartSpeed: 1000,
			autoplay: false,
			fibiDeferredAutoplay: true,
			autoplayTimeout: 5000,
			navText: [ '<span class="far fa-angle-left"></span>', '<span class="far fa-angle-right"></span>' ],
			responsive:{
				0:{
					items:1
				},
				480:{
					items:1
				},
				600:{
					items:2
				},
				800:{
					items:2
				},
				1024:{
					items:3
				}
			}
		});    		
	}
	// Ana sayfa "Son eklenenler" (4'lü, eşit yükseklikli kartlar)
	if ($('.latest-listings-carousel').length) {
		$('.latest-listings-carousel').owlCarousel({
			loop: $('.latest-listings-carousel').children().length > 4,
			margin:30,
			nav:true,
			smartSpeed: 800,
			autoplay: false,
			fibiDeferredAutoplay: true,
			autoplayTimeout: 5000,
			navText: [ '<span class="far fa-angle-left"></span>', '<span class="far fa-angle-right"></span>' ],
			responsive:{
				0:{ items:1 },
				768:{ items:2 },
				992:{ items:3 },
				1200:{ items:4 }
			}
		});
	}
	// Five Item Carousel
	if ($('.five-item-carousel').length) {
		$('.five-item-carousel').owlCarousel({
			loop:true,
			margin:30,
			nav:true,
			smartSpeed: 500,
			autoplay: false,
			fibiDeferredAutoplay: true,
			autoplayTimeout: 5000,
			navText: [ '<span class="fas fa-angle-left"></span>', '<span class="fas fa-angle-right"></span>' ],
			responsive:{
				0:{
					items:1
				},
				600:{
					items:2
				},
				800:{
					items:3
				},
				1024:{
					items:4
				},
				1200:{
					items:5
				}
			}
		});    		
	}
	// Four Item Carousel
	if ($('.four-item-carousel').length) {
		$('.four-item-carousel').owlCarousel({
			loop:true,
			margin:30,
			nav:true,
			smartSpeed: 500,
			autoplay: false,
			fibiDeferredAutoplay: true,
			autoplayTimeout: 5000,
			navText: [ '<span class="fas fa-angle-left"></span>', '<span class="fas fa-angle-right"></span>' ],
			responsive:{
				0:{
					items:1
				},
				600:{
					items:2
				},
				800:{
					items:3
				},
				1024:{
					items:4
				},
				1200:{
					items:4
				}
			}
		});    		
	}

	$(function () {
		// Owl init
		if ($.fn.owlCarousel) {
			$('.single-item-carousel').each(function () {
				var $el = $(this);
				if (!$el.hasClass('owl-loaded')) {
					$el.owlCarousel({
						items: 1,
						loop: true,
						nav: true,
						dots: false
					});
				}
			});
		} else {
			console.warn('owlCarousel plugin yuklenmedi (dosya eksik veya 404).');
		}

		// Fancybox init (genel)
		if ($.fn.fancybox) {
			$('[data-fancybox]').fancybox({
				buttons: ["zoom", "slideShow", "thumbs", "close"],
				loop: true,
				protect: true
			});
		}
	});
	$(document)
		.off('click.fibiLightbox2')
		.on('click.fibiLightbox2', '.single-item-carousel .image-box, .single-item-carousel .image-box img', function (e) {

			e.preventDefault();
			e.stopPropagation();

			var $a = $(this).find('a.lightbox-image').first();
			if (!$a.length) return;

			var group = $a.attr('data-fancybox');
			var $items = $('[data-fancybox="' + group + '"]');
			var index = $items.index($a);

			$.fancybox.open($items, {
				buttons: ["zoom", "slideShow", "thumbs", "close"],
				loop: true,
				protect: true
			}, index);
		});
	// deals Carousel
	if ($('.deals-carousel').length) {
		$('.deals-carousel').owlCarousel({
			loop:true,
			margin:50,
			nav:true,
			smartSpeed: 500,
			autoplay: false,
			fibiDeferredAutoplay: true,
			autoplayTimeout: 5000,
			navText: [ '<span class="far fa-angle-left"></span>', '<span class="far fa-angle-right"></span>' ],
			responsive:{
				0:{
					items:1
				},
				600:{
					items:1
				},
				800:{
					items:1
				},
				1024:{
					items:1
				},
				1200:{
					items:1
				}
			}
		});    		
	}
	// banner-carousel
	if ($('.banner-carousel').length) {
        $('.banner-carousel').owlCarousel({
            loop:true,
			margin:0,
			nav:true,
			animateOut: 'fadeOut',
    		animateIn: 'fadeIn',
    		active: true,
			smartSpeed: 1000,
			autoplay: false,
			fibiDeferredAutoplay: true,
			autoplayTimeout: 6000,
            navText: [ '<span class="far fa-angle-left"></span>', '<span class="far fa-angle-right"></span>' ],
            responsive:{
                0:{
                    items:1
                },
                600:{
                    items:1
                },
                800:{
                    items:1
                },
                1024:{
                    items:1
                }
            }
        });
    }
	//Add One Page nav
	if($('.scroll-nav').length) {
		$('.scroll-nav').onePageNav();
	}
	//Sortable Masonary with Filters
	function enableMasonry() {
		if($('.sortable-masonry').length){
	
			var winDow = $(window);
			// Needed variables
			var $container=$('.sortable-masonry .items-container');
			var $filter=$('.filter-btns');
	
			$container.isotope({
				filter:'*',
				 masonry: {
					columnWidth : '.masonry-item.small-column'
				 },
				animationOptions:{
					duration:500,
					easing:'linear'
				}
			});
	
			// Isotope Filter 
			$filter.find('li').on('click', function(){
				var selector = $(this).attr('data-filter');
	
				try {
					$container.isotope({ 
						filter	: selector,
						animationOptions: {
							duration: 500,
							easing	: 'linear',
							queue	: false
						}
					});
				} catch(err) {
	
				}
				return false;
			});
			winDow.on('resize', function(){
				var selector = $filter.find('li.active').attr('data-filter');

				$container.isotope({ 
					filter	: selector,
					animationOptions: {
						duration: 500,
						easing	: 'linear',
						queue	: false
					}
				});
			});
			var filterItemA	= $('.filter-btns li');
	
			filterItemA.on('click', function(){
				var $this = $(this);
				if ( !$this.hasClass('active')) {
					filterItemA.removeClass('active');
					$this.addClass('active');
				}
			});
		}
	}
	
	enableMasonry();
	//Price Range Slider — Only init for NON-search-module sliders (listing pages etc.)
	if($('.price-range-slider').length && !$('#priceSlider-forsale').length){
		$( ".price-range-slider" ).slider({
			range: true,
			min: 0,
			max: 50000000,
			step: 50000,
			values: [ 500000, 5000000 ],
			slide: function( event, ui ) {
				var $slider = $(this);
				$slider.closest('.price-range').find('input.property-amount').val( ui.values[0].toLocaleString('tr-TR') + ' ₺ - ' + ui.values[1].toLocaleString('tr-TR') + ' ₺' );
			}
		});
		
		$('.price-range-slider').each(function(){
			var $s = $(this);
			$s.closest('.price-range').find('input.property-amount').val( $s.slider('values', 0).toLocaleString('tr-TR') + ' ₺ - ' + $s.slider('values', 1).toLocaleString('tr-TR') + ' ₺' );
		});
	}

	//Area Range Slider — Only init for NON-search-module sliders
	if($('.area-range-slider').length && !$('#areaSlider-forsale').length){
		$( ".area-range-slider" ).slider({
			range: true,
			min: 0,
			max: 5000,
			step: 10,
			values: [ 50, 500 ],
			slide: function( event, ui ) {
				var $slider = $(this);
				$slider.closest('.area-range').find('input.area-range').val( ui.values[0] + ' m² - ' + ui.values[1] + ' m²' );
			}
		});
		
		$('.area-range-slider').each(function(){
			var $s = $(this);
			$s.closest('.area-range').find('input.area-range').val( $s.slider('values', 0) + ' m² - ' + $s.slider('values', 1) + ' m²' );
		});
	}

    // Progress Bar
	if ($('.count-bar').length) {
		$('.count-bar').appear(function(){
			var el = $(this);
			var percent = el.data('percent');
			$(el).css('width',percent).addClass('counted');
		},{accY: -50});

	}

	$(document).ready(function() {
      $('select:not(.ignore)').niceSelect();

    });


    // color switcher
	function swithcerMenu() {
	  	if ($('.switch_menu').length) {

	    	$('.switch_btn button').on('click', function(){
	      	$('.switch_menu').toggle(500)
	    	});

	    	$('#styleOptions').styleSwitcher({
	        hasPreview: true,
	        fullPath: 'assets/css/color/',
	         	cookie: {
	          	expires: 30,
	          	isManagingLoad: true
	      		}
	    	});

	  	};
	}

	// page direction
	function directionswitch() {
	  	if ($('.page_direction').length) {

	    	$('.direction_switch button').on('click', function() {
			   $('body').toggleClass(function(){
			      return $(this).is('.rtl, .ltr') ? 'rtl ltr' : 'rtl';
			  })
			});
	  	};
	}

	if($('.paroller').length){
		$('.paroller').paroller({
			  factor: 0.1,            // multiplier for scrolling speed and offset, +- values for direction control  
			  factorLg: 0.1,          // multiplier for scrolling speed and offset if window width is less than 1200px, +- values for direction control  
			  type: 'foreground',     // background, foreground  
			  direction: 'vertical' // vertical, horizontal  
		});
	}

	if($('.paroller-2').length){
		$('.paroller-2').paroller({
			  factor: -0.1,            // multiplier for scrolling speed and offset, +- values for direction control  
			  factorLg: -0.1,          // multiplier for scrolling speed and offset if window width is less than 1200px, +- values for direction control  
			  type: 'foreground',     // background, foreground  
			  direction: 'vertical' // vertical, horizontal  
		});
	}

	// Date picker
	function datepicker () {
	    if ($('#datepicker').length) {
	        $('#datepicker').datepicker();
	    };
	}

	// Time picker
	function timepicker() {
		if ($.fn.ptTimeSelect && $('input[name="time"]').length) {
			$('input[name="time"]').ptTimeSelect();
		}
	}


	if ($('.property-details .bxslider').length) {
		$('.property-details .bxslider').bxSlider({
			auto:true,
	        nextSelector: '.property-details #slider-next',
	        prevSelector: '.property-details #slider-prev',
	        nextText: '<i class="fa fa-angle-right"></i>',
	        prevText: '<i class="fa fa-angle-left"></i>',
	        mode: 'fade',
	        auto: 'true',
	        speed: '700',
	        pagerCustom: '.property-details .slider-pager .thumb-box'
	    });
	};


	/*	=========================================================================
	When document is Scrollig, do
	========================================================================== */

	jQuery(document).on('ready', function () {
		(function ($) {
			// add your functions
			directionswitch();
			swithcerMenu();
			datepicker ();
			timepicker ();
		})(jQuery);
	});

	/* ==========================================================================
   When document is Scrollig, do
   ========================================================================== */
	
	$(window).on('scroll', function() {
		headerStyle();
	});
	
	/* ==========================================================================
   When document is loaded, do
   ========================================================================== */
	
	// Autoplay sayfa (ve görseller) yüklenince başlar; DOM hazır olduğunda başlarsa
	// görsel geç gelen ilk slayt ekranda bir an görünüp hemen geçiyordu.
	function startDeferredAutoplay() {
		$('.owl-carousel').each(function () {
			var owl = $(this).data('owl.carousel');
			if (!owl || !owl.options.fibiDeferredAutoplay || owl.options.autoplay) return;
			owl.options.autoplay = true;
			$(this).trigger('refresh.owl.carousel');
		});
	}

	$(window).on('load', function() {
		handlePreloader();
		enableMasonry();
		startDeferredAutoplay();
	});

})(window.jQuery);

