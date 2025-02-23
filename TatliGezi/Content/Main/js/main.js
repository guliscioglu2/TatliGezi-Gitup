/*=================================================
* Template Name: Fiore - Blog & Shop HTML Template
* Version: 1.0 (Initial Release)
* Author Name: Jomin Muskaj (Eagle-Themes)
* Author URI: eagle-themes.com
===================================================*/

 (function ($) {
     "use strict";

     /*========== LOADING PAGE ==========*/
     $(window).on('load', function () {
         $(".preloader").fadeOut(500);
     });

     /* Document is Raedy */
     $(document).ready(function () {

         /*========== MENU ==========*/
         $(window).on("scroll", function () {
             var header = $('header');
             var topmenu = $('.top-menu');
             var biglogo = $('.big-logo');
             var windowheight = $(this).scrollTop();
             var menuheight = header.outerHeight();
             var topmenuheight = 0;
             var biglogoheight = 0;
             if (topmenu.length > 0) {
                 var topmenuheight = topmenu.outerHeight();
             }
             
             if (biglogo.length > 0) {
                 var biglogoheight = biglogo.outerHeight();
             }
             var fixedheight = topmenuheight + biglogoheight;
             if (header.length > 0) {
                 if ((windowheight > fixedheight) && header.hasClass('fixed')) {
                     header.addClass('navbar-fixed-top');
                     if (!header.hasClass("transparent")) {
                         header.next("*").css("margin-top", menuheight);
                     }
                     if (header.hasClass("fixed")) {
                         header.addClass("scroll");
                     }
                 } else {
                     header.removeClass("navbar-fixed-top");
                     if (!header.hasClass("transparent")) {
                         header.next("*").css("margin-top", "0");
                     }
                     if (header.hasClass("fixed")) {
                         header.removeClass("scroll");
                     }
                 }
             }
         });
         $(function () {
             function toggleNavbarMethod() {
                 var caretup = "caret caret-up";
                 if ($(window).width() > 992) {
                     $('.dropdown')
                         .on('mouseover', function () {
                             $(this).addClass('open');
                             $('b', this).toggleClass(caretup);
                         })

                         .on('mouseout', function () {
                             $(this).removeClass('open');
                             $('b', this).toggleClass(caretup);
                         });
                 } else {
                     $('.dropdown').off('mouseover').off('mouseout');
                     $('.dropdown-toggle')

                         .on('click', function (e) {
                             $('b', this).toggleClass(caretup);
                         });
                 }
             }
             toggleNavbarMethod();
             $(window).on("resize", (toggleNavbarMethod));

             $(".navbar-toggle").on("click", function () {
                 $(this).toggleClass("active");
             });
         });

         /* ==========  MOBILE MENU ========== */
         $('.mobile-menu-btn').jPushMenu({
             closeOnClickLink: false
         });
         $('.dropdown-toggle').dropdown();

         /* ==========  SWIPER SLIDER - HOME PAGE 3, 4 ========== */
         var swiper = new Swiper('.swiper-slider', {
             slidesPerView: 1,
             nextButton: '.swiper-button-next',
             prevButton: '.swiper-button-prev',
             loop: true,
             autoplay: 5000,
         });

         /* ==========  SWIPER SLIDER - HOME PAGE 5 ========== */
         var swiper = new Swiper('.swiper-carousel', {
             slidesPerView: 3,
             loop: true,
             spaceBetween: 30,

             breakpoints: {
                 1024: {
                     slidesPerView: 3,
                     spaceBetween: 30
                 },
                 768: {
                     slidesPerView: 2,
                     spaceBetween: 30
                 },
                 640: {
                     slidesPerView: 1,
                     spaceBetween: 20
                 },
                 320: {
                     slidesPerView: 1,
                     spaceBetween: 10
                 }
             }

         });

         /*========== BLOG POST GALLERY ==========*/
         $('.blog-post-gallery-slider').owlCarousel({
             loop: true,
             margin: 5,
             items: 1,
             nav: true,
             navText: [
                "<i class='flaticon-back'></i>",
                "<i class='flaticon-next'></i>"
            ],
         })

         /*========== BLOG SLIDER POST ==========*/
         $('.blog-slider-post').owlCarousel({
             loop: true,
             margin: 0,
             items: 1,
             nav: true,
             navText: [
                "<i class='flaticon-back'></i>",
                "<i class='flaticon-next'></i>"
            ],
         })

         /*========== BLOG POST REALATED POSTS ==========*/
         $('.related-posts').owlCarousel({
             loop: true,
             margin: 30,
             nav: false,

             autoHeight: true,
             responsive: {
                 0: {
                     items: 1
                 },
                 600: {
                     items: 3
                 },
                 1000: {
                     items: 5
                 }
             }
         })

         /*========== POP OVER & TOOLTIP ==========*/
         $('[data-toggle="popover"]').popover();
         $('[data-toggle="tooltip"]').tooltip({
             animated: 'fade',
             container: 'body'
         });

         /*========== SELECT PICKER ==========*/
         $('select').selectpicker({
             style: 'btn-select',
             container: 'body',
         });
         if (/Android|webOS|iPhone|iPad|iPod|BlackBerry/i.test(navigator.userAgent)) {
             $('select').selectpicker('mobile');
         }

         /*========== SHOP IMAGE HOVER ==========*/
         $(".add-to-cart").on({
             mouseenter: function () {
                 $(this).parent().addClass("active");
             },
             mouseleave: function () {
                 $(this).parent().removeClass("active");
             }
         });

         /*========== PRODUCT QUANTITY ==========*/
         $(function () {
             var spinner = $('.spinner');
             var spinnerplus = $('.spinner .btn:first-of-type')
             var spinnerminus = $('.spinner .btn:last-of-type');
             $(spinnerplus).on('click', function () {
                 var btn = $(this);
                 var input = btn.closest(spinner).find('input');
                 if (input.attr('max') == undefined || parseInt(input.val()) < parseInt(input.attr('max'))) {
                     input.val(parseInt(input.val(), 10) + 1, 10);
                 } else {
                     btn.next("disabled", true);
                 }
             });
             $(spinnerminus).on('click', function () {
                 var btn = $(this);
                 var input = btn.closest(spinner).find('input');
                 if (input.attr('min') == undefined || parseInt(input.val()) > parseInt(input.attr('min'))) {
                     input.val(parseInt(input.val(), 10) - 1, 10);
                 } else {
                     btn.prev("disabled", true);
                 }
             });
         })

         /*========== PRODUCT RATING ==========*/
         $('.user-rating input').on('change', function () {
             var $radio = $(this);
             $('.user-rating .selected').removeClass('selected');
             $radio.closest('label').addClass('selected');
         });

         /* ==========  SHARE BOX  ========== */
         $('.share-btns .open-btn').on('click', function () {
             $(this).parent().parent().addClass("clicked");
         });

         $('.share-btns .close-btn').on('click', function (e) {
             $(this).parent().parent().removeClass('clicked');
             e.stopPropagation();
         });

         /* ==========  BLOG POST EFFECT ========== */
         var posthero = $('.blog-post-image-effect');
         var range = 600;
         if (posthero.length) {
             $(window).on('scroll', function () {
                 var scrollTop = $(this).scrollTop();
                 var offset = posthero.offset().top;
                 var height = posthero.outerHeight();
                 offset = offset + height / 2;
                 var calc = 1.45 - (scrollTop - offset + range) / range;
                 posthero.css({
                     'opacity': calc
                 });
                 if (calc > '1') {
                     posthero.css({
                         'opacity': '1'
                     });
                 } else if (calc < '0') {
                     posthero.css({
                         'opacity': '0'
                     });
                 }
             });
         }
         $(window).on('scroll', function () {
             var scaleFactor = 1800,
                 scaleMax = 1.3;
             var scale = Math.min(($(window).scrollTop() + scaleFactor) / scaleFactor, scaleMax);
             $('.blog-post-image-effect').css('transform', 'scale(' + scale + ')');
         });

         /* ==========  OWL SLIDER - SHOP DETAILS PAGE ========== */
         var $sync1 = $(".big-images"),
             $sync2 = $(".thumbs"),
             duration = 300;
         $sync1.owlCarousel({
                 items: 1,
                 dots: false,
             })
             .on('changed.owl.carousel', function (e) {
                 var syncedPosition = syncPosition(e.item.index);

                 if (syncedPosition != "stayStill") {
                     $sync2.trigger('to.owl.carousel', [syncedPosition, duration, true]);
                 }
             });
         $sync2
             .on('initialized.owl.carousel', function () {
                 addClassCurrent(0);
             })
             .owlCarousel({
                 dots: false,
                 margin: 10,
                 responsive: {
                     0: {
                         items: 3
                     },
                     600: {
                         items: 3
                     },
                     960: {
                         items: 4
                     },
                     1200: {
                         items: 4
                     }
                 }
             })
             .on('click', '.owl-item', function () {
                 $sync1.trigger('to.owl.carousel', [$(this).index(), duration, true]);
             });

         function addClassCurrent(index) {
             $sync2
                 .find(".owl-item")
                 .removeClass("active-item")
                 .eq(index).addClass("active-item");
         }

         function syncPosition(index) {
             addClassCurrent(index);
             var itemsNo = $sync2.find(".owl-item").length; //total items
             var visibleItemsNo = $sync2.find(".owl-item.active").length; //visible items

             if (itemsNo === visibleItemsNo) {
                 return "stayStill";
             }
             var visibleCurrentIndex = $sync2.find(".owl-item.active").index($sync2.find(".owl-item.active-item"));

             if (visibleCurrentIndex == 0 && index != 0) {
                 return index - 1;
             }
             if (visibleCurrentIndex == (visibleItemsNo - 1) && index != (itemsNo - 1)) {
                 return index - visibleItemsNo + 2;
             }
             return "stayStill";
         }

         /*========== ISOTOPE ==========*/
         $(function () {
             var $grid = $('.grid').isotope({
                 itemSelector: '.g-item'
             });
             // filters
             $('.grid-filters').on('click', 'a', function (e) {
                 e.preventDefault();
                 var filterValue = $(this).attr('data-filter');
                 $grid.isotope({
                     filter: filterValue
                 });
             });
             // active class
             $('.grid-filters').each(function (i, buttonGroup) {
                 var $buttonGroup = $(buttonGroup);
                 $buttonGroup.on('click', 'a', function () {
                     $buttonGroup.find('.active').removeClass('active');
                     $(this).addClass('active');
                 });
             });
         });

         /*========== COMMING SOON PAGE ==========*/
         $('#countdown').each(function () {
             var $this = $(this),
                 finalDate = $(this).data('countdown');
             $this.countdown(finalDate, function (event) {
                 $this.html(event.strftime(
                     '<div class="count-box"><div class="inner"><div class="count-number">%D</div><div class="count-text">Days</div></div></div> ' + '<div class="count-box"><div class="inner"><div class="count-number">%H</div><div class="count-text">Hours</div></div></div> ' + '<div class="count-box"><div class="inner"><div class="count-number">%M</div><div class="count-text">Minutes</div></div></div> ' + '<div class="count-box"><div class="inner"><div class="count-number">%S</div><div class="count-text">Seconds</div><div></div>'));
             });
         });

         /*========== MAGNIFIC POPUP ==========*/
         $(".image-popup, a[data-rel^='image-popup']").magnificPopup({
             type: 'image',
             mainClass: 'mfp-with-zoom', // this class is for CSS animation below
             zoom: {
                 enabled: true,
                 duration: 300,
                 easing: 'ease-in-out',
                 fixedContentPos: true,
                 opener: function (openerElement) {
                     return openerElement.is('img') ? openerElement : openerElement.find('img');
                 }
             },
             retina: {
                 ratio: 1, // Increase this number to enable retina image support.
                 replaceSrc: function (item, ratio) {
                     return item.src.replace(/\.\w+$/, function (m) {
                         return '@2x' + m;
                     });
                 }
             }

         });
         $('.image-gallery').magnificPopup({
             delegate: 'a',
             type: 'image',
             fixedContentPos: true,
             gallery: {
                 enabled: true
             },
             removalDelay: 300,
             mainClass: 'mfp-fade',
             retina: {
                 ratio: 1,
                 replaceSrc: function (item, ratio) {
                     return item.src.replace(/\.\w+$/, function (m) {
                         return '@2x' + m;
                     });
                 }
             }

         });
         $('.popup-youtube, .popup-vimeo, .popup-gmaps').magnificPopup({
             type: 'iframe',
             mainClass: 'mfp-fade',
             removalDelay: 300,
             preloader: false,
             fixedContentPos: true,
         });

         /*========== GOOGLE MAP ==========*/
         function initialize() {
             var map;
             var panorama;
             var var_latitude = -37.817061; // Google Map Latitude
             var var_longitude = 144.955957; // Google Map Longitude
             var pin = 'images/icons/pin.svg';

             //Map pin-window details
             var title = "Fiore - Click to see";
             var hotel_name = "Fiore";
             var hotel_address = "25, Lorem Ipsum";
             var hotel_desc = "Blog & Shop HTML Template";
             var hotel_more_desc = "Lorem ipsum dolor sit amet, consectetur.";
             var hotel_location = new google.maps.LatLng(var_latitude, var_longitude);
             var mapOptions = {
                 center: hotel_location,
                 zoom: 15,
                 scrollwheel: false,
                 streetViewControl: false
             };
             map = new google.maps.Map(document.getElementById('map-canvas'),
                 mapOptions);
             var contentString =
                 '<div id="infowindow_content">' +
                 '<p><strong>' + hotel_name + '</strong><br>' +
                 hotel_address + '<br>' +
                 hotel_desc + '<br>' +
                 hotel_more_desc + '</p>' +
                 '</div>';
             var var_infowindow = new google.maps.InfoWindow({
                 content: contentString
             });
             var marker = new google.maps.Marker({
                 position: hotel_location,
                 map: map,
                 icon: pin,
                 title: title,
                 maxWidth: 500,
                 optimized: false,
             });
             google.maps.event.addListener(marker, 'click', function () {
                 var_infowindow.open(map, marker);
             });
             panorama = map.getStreetView();
             panorama.setPosition(hotel_location);
             panorama.setPov( /** @type {google.maps.StreetViewPov} */ ({
                 heading: 265,
                 pitch: 0
             }));
             var openStreet = document.getElementById('openStreetView');
             if (openStreet) {
                 document.getElementById("openStreetView").onclick = function () {
                     toggleStreetView()
                 };
             }

             function toggleStreetView() {
                 var toggle = panorama.getVisible();
                 if (toggle == false) {
                     panorama.setVisible(true);
                 } else {
                     panorama.setVisible(false);
                 }
             }
         }
         //Check if google map div exist
         if ($("#map-canvas").length > 0) {
             google.maps.event.addDomListener(window, 'load', initialize);
         }

         /*========== FULL SCREEN SEARCH ==========*/
         var fullscreansearch = $(".fullscreen-search");
         var searchfield = $(".field");
         var istype = "is-type";
         var isfocus = "is-focus";
         $(".btn-header-search").on("click", function (event) {
             event.preventDefault();
             $(fullscreansearch).addClass("open");
             //$("body").addClass("overflow-hidden");
         });
         $(".search-close").on("click", function (event) {
             event.preventDefault();
             $(fullscreansearch).removeClass("open");
             //$("body").removeClass("overflistype");
         });
         $(searchfield).on('focus', function () {
             $(fullscreansearch).addClass('focus');
         });
         $(searchfield).on('blur', function () {
             $(fullscreansearch).removeClass('focus is-type');
         });
         $(searchfield).on('keydown', function (event) {
             $(fullscreansearch).addClass(istype);
             if ((event.which === 8) && $(this).val() === '') {
                 $(fullscreansearch).removeClass(istype);
             }
         });
         $(searchfield).on('focus', function () {
             $(fullscreansearch).addClass(isfocus);
         });
         $(searchfield).on('blur', function () {
             $(fullscreansearch).removeClass(isfocus, istype);
         });
         $(searchfield).on('keydown', function (event) {
             $(fullscreansearch).addClass(istype);
             if ((event.which === 8) && $(this).val() === '') {
                 $(fullscreansearch).removeClass(istype);
             }
         });

         /*========== BACK TO TOP ==========*/
         var amountScrolled = 500;
         var backtotop = $('#back-to-top');
         $(window).on('scroll', function () {
             if ($(window).scrollTop() > amountScrolled) {
                 backtotop.addClass('active');
             } else {
                 backtotop.removeClass('active');
             }
         });
         backtotop.on('click', function () {
             $('html, body').animate({
                 scrollTop: 0
             }, 500);
             return false;
         });

     });
     
 })(jQuery);