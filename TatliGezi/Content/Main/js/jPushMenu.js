/*!
 * jPushMenu.js
 * 1.1.1
 * @author: takien
 * http://takien.com
 * Original version (pure JS) is created by Mary Lou http://tympanus.net/
 */

jQuery(document).ready(function($){

(function ($) {

    $.fn.jPushMenu = function (customOptions) {
        var o = $.extend({}, $.fn.jPushMenu.defaultOptions, customOptions);


        $(this).click(function () {

            $(this).toggleClass('mobile-menu-active');
            $('.mobile-menu').toggleClass('mobile-menu-open');

            /* mobile menu button class */
            if ($(this).is('.mobile-menu-btn')) {
                $('body').toggleClass('push-mobile');
            }

            /* disable all other button */
            $('.jPushMenuBtn').not($(this)).toggleClass('disabled');

            return false;
        });
        var jPushMenu = {
            close: function (o) {
                $('.jPushMenuBtn,body,.mobile-menu-btn,.mobile-menu').removeClass('disabled active mobile-menu-active mobile-menu-open push-mobile');
            }
        };

        if (o.closeOnClickOutside) {
            $(document).click(function () {
                jPushMenu.close();
            });

            $(document).on('click touchstart', function () {
                jPushMenu.close();
            });

            $('.mobile-menu,.mobile-menu-btn').click(function (e) {
                e.stopPropagation();
            });

            $('.mobile-menu,.mobile-menu-btn').on('click touchstart', function (e) {
                e.stopPropagation();
            });
        }

        // On Click Link - Mobile menu link
        if (o.closeOnClickLink) {
            $('.mobile-menu a').on('click', function () {
                jPushMenu.close();
            });
        }
    };

    /* in case you want to customize class name,
     *  do not directly edit here, use function parameter when call jPushMenu.
     */
    $.fn.jPushMenu.defaultOptions = {

        closeOnClickOutside: true,
        closeOnClickInside: true,
        closeOnClickLink: false
    };
})(jQuery);


/*
 * jQuery.v2p
 * Set css viewport units
 */
;(function( $ ){

  function viewportToPixel( val ) {
    var percent = val.match(/\d+/)[0] / 100,
        unit = val.match(/[vwh]+/)[0];
    return ( unit == 'vh'
      ? $(window).height() * percent
      : $(window).width() * percent ) + 'px';
  }

  function parseProps( props ) {
    var p, prop;
    for ( p in props ) {
      prop = props[ p ];
      if ( /[vwh]$/.test( prop ) ) {
        props[ p ] = viewportToPixel( prop );
      }
    }
    return props;
  }

  $.fn.v2p = function( props ) {
    return this.css( parseProps( props ) );
  };

}( jQuery ));

$(window).resize(function() {
  $('.cbp-spmenu').v2p({
    height:'100vh'
  });
});

$(window).resize(); // trigger event so it shows on load
    
});