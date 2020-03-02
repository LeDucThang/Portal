
(function ($) {
    "use strict";

    $(window).load(function () {
        $("[data-action='toggle-nav-cart']").click(function () {
            $(this).toggleClass('active');
            $(".block-nav-cart").toggleClass("has-open");
            $("body").toggleClass("menu-open");
            return false;
        });
        // $(".menu-mobile.close-btn").click(function () {
        //    $(this).toggleClass('active');
        //    $(".block-nav-cart").toggleClass("has-open");
        //    $("body").toggleClass("menu-open");
        //    return false;
        //});
    });

    $(document).ready(function () {

        var status1 = $("#callback-page1");
        var status2 = $("#callback-page2");
        var status3 = $("#callback-page3");
        function callback1(event) {

            var items = event.item.count;
            var item = event.item.index + 1;


            updateResult1(".currentItem", item);
            updateResult1(".owlItems", items);


        }

        $('.related-posts-carousel').owlCarousel({
            loop: false,
            margin: 15,
            nav: true,
            dots: false,
            navText: ['<a href="javascript:;" class="gem-button gem-button-style-outline gem-button-size-tiny related-posts-prev" style="display: inline-block;"><i class="gem-print-icon gem-icon-pack-thegem-icons gem-icon-prev"></i></a>', '<a href="javascript:;" class="gem-button gem-button-style-outline gem-button-size-tiny related-posts-next" style="display: inline-block;"><i class="gem-print-icon gem-icon-pack-thegem-icons gem-icon-next"></i></a>'],
            responsive: {
                0: {
                    items: 1
                },
                600: {
                    items: 3
                },
                1000: {
                    items: 4
                }
            }
        })

        function callback2(event) {

            var items = event.item.count;
            var item = event.item.index + 1;


            updateResult2(".currentItem", item);
            updateResult2(".owlItems", items);


        }
        function callback3(event) {

            var items = event.item.count;
            var item = event.item.index + 1;


            updateResult3(".currentItem", item);
            updateResult3(".owlItems", items);


        }
        function updateResult1(pos, value) {
            status1.find(pos).find(".result").text(value);
        }
        function updateResult2(pos, value) {
            status2.find(pos).find(".result").text(value);
        }
        function updateResult3(pos, value) {
            status3.find(pos).find(".result").text(value);
        }


        $(".owl-carousel").each(function (index, el) {
            var config = $(this).data();
            config.navText = ['', ''];
            config.smartSpeed = "800";

            if ($(this).hasClass('dotsData')) {
                config.dotsData = "true";
            }
            if ($(this).hasClass('callback-page1')) {
                config.onChanged = callback1;
            }
            if ($(this).hasClass('callback-page2')) {
                config.onChanged = callback2;
            }
            if ($(this).hasClass('callback-page3')) {
                config.onChanged = callback3;
            }
            if ($(this).parents("html").hasClass('cms-rtl')) {
                config.rtl = "true";
            }

            $(this).owlCarousel(config);

        });


        /*  [Mobile Search ]
        - - - - - - - - - - - - - - - - - - - - */


        $(".block-search .block-title").on('click', function () {
            $(this).parent().toggleClass('active');
            return false;
        });

        /*  [Mobile menu ]
        - - - - - - - - - - - - - - - - - - - - */
        $(".ui-menu .toggle-submenu").on('click', function () {

            $(this).parent().toggleClass('open-submenu');
            return false;
        });

        $("[data-action='toggle-nav']").on('click', function () {
            $(this).toggleClass('active');
            $(".block-nav-menu").toggleClass("has-open");
            $("body").toggleClass("menu-open");
            return false;

        });

        $("[data-action='close-nav']").on('click', function () {
            $("[data-action='toggle-nav']").removeClass('active');
            $(".block-nav-menu").removeClass("has-open");
            $("body").removeClass("menu-open");
            return false;

        });

        //$("[data-action='toggle-nav-cart']").click(function () {
        //    $(this).toggleClass('active');
        //    
        //    $(".block-nav-cart").toggleClass("has-open");
        //    $("body").toggleClass("menu-open");
        //    return false;

        //});

        /*  [Mobile categori ]
        - - - - - - - - - - - - - - - - - - - - */
        $(".block-nav-categori .block-title").on('click', function () {
            $(this).toggleClass('active');
            $(this).parent().toggleClass('has-open');
            $("body").toggleClass("categori-open");
            return false;
        });

        $(".ui-categori .toggle-submenu").on('click', function () {
            $(this).parent().toggleClass('open-submenu');
            return false;
        });

        /*  [Mobile click service ]
        - - - - - - - - - - - - - - - - - - - - */
        $(".service-opt-1 .block-title").on('click', function () {

            $(this).parent().toggleClass('active');
            return false;
        });


        /*  [animate click -floor ]
        - - - - - - - - - - - - - - - - - - - - */
        $(".block-title .action ").on('click', function (e) {

            // prevent default anchor click behavior
            e.preventDefault();

            // store hash
            var hash = this.hash;

            // animate
            $('html, body').animate({
                scrollTop: $(hash).offset().top
            }, 500, function () {

                // when done, add hash to url
                // (default click behaviour)
                window.location.hash = hash;
            });

        });

        /*  [COUNT DOWN ]
        - - - - - - - - - - - - - - - - - - - - */
        $('[data-countdown]').each(function () {
            var $this = $(this), finalDate = $(this).data('countdown');
            $this.countdown(finalDate, function (event) {
                var fomat = '<div class="box-count box-days"><div class="number">%D</div><div class="text">Days</div></div><div class="box-count box-hours"><div class="number">%H</div><div class="text">Hours</div></div><div class="box-count box-min"><div class="number">%M</div><div class="text">Mins</div></div><div class="box-count box-secs"><div class="number">%S</div><div class="text">Secs</div></div>';
                $this.html(event.strftime(fomat));
            });
        });

        /*  [Button Filter Products  ]
        - - - - - - - - - - - - - - - - - - - - */
        //open filter
        $(".btn-filter-products").on('click', function () {
            $(this).toggleClass('active');
            $("#layered-filter-block").toggleClass('active');
            $("body").toggleClass('filter-active');
            return false;
        });

        //Close filter
        $("#layered-filter-block .close-filter-products").on('click', function () {
            $(".btn-filter-products").removeClass('active');
            $("#layered-filter-block").removeClass('active');
            $("body").removeClass('filter-active');
            return false;
        });

        //toggle filter options
        $("#layered-filter-block .filter-options-title").on('click', function () {
            $(this).toggleClass('active');
            $(this).parent().toggleClass('active');
            return false;
        });



        var ResizeSlider = function () {
            var documentWidth = $(window).innerWidth();

            $('.slider-resized').css('height', (documentWidth * 760) / 1900 + 'px');
        }

        var ProductResize = function () {
            var productListHeight = $('.col-main').height();
            if (productListHeight > $('.col-sidebar').height()) {
                $('.col-sidebar').css('height', productListHeight + 'px');
            }
        }

        $(window).resize(function () {
            ResizeSlider();
            if (window.innerWidth > 991 && $('.col-main') != undefined) {
                ProductResize();
            }
        });

        ResizeSlider();
        ProductResize();
        /* ------------------------------------------------
                Arctic modal
        ------------------------------------------------ */

        if ($.arcticmodal) {
            $.arcticmodal('setDefault', {
                type: 'ajax',
                ajax: {
                    cache: false
                },
                afterOpen: function (obj) {

                    var mw = $('.modal_window');

                    mw.find('.custom_select').customSelect();

                    mw.find('.product_preview .owl_carousel').owlCarousel({
                        margin: 10,
                        stagePadding: 0,
                        themeClass: 'thumbnails_carousel',
                        nav: true,
                        navText: [],
                        rtl: window.ISRTL ? true : false
                    });

                    Core.events.productPreview();

                    addthis.toolbox('.addthis_toolbox');

                }
            });
        }

        /* ------------------------------------------------
                Fancybox
        ------------------------------------------------ */

        if ($.fancybox) {
            $.fancybox.defaults.direction = {
                next: 'left',
                prev: 'right'
            }
        }

        if ($('.fancybox_item').length) {
            $('.fancybox_item').fancybox({
                openEffect: 'elastic',
                closeEffect: 'elastic',
                helpers: {
                    overlay: {
                        css: {
                            'background': 'rgba(0,0,0, .6)'
                        }
                    },
                    thumbs: {
                        width: 50,
                        height: 50
                    }
                }
            });
        }

        if ($('.fancybox_item_media').length) {
            $('.fancybox_item_media').fancybox({
                openEffect: 'none',
                closeEffect: 'none',
                helpers: {
                    media: {}
                }
            });
        }

        /* ------------------------------------------------
                Elevate Zoom
        ------------------------------------------------ */

        if ($('#img_zoom').length) {
            $('#img_zoom').elevateZoom({
                zoomType: "inner",
                gallery: 'thumbnails',
                galleryActiveClass: 'active',
                cursor: "crosshair",
                responsive: true,
                easing: true,
                zoomWindowFadeIn: 500,
                zoomWindowFadeOut: 500,
                lensFadeIn: 500,
                lensFadeOut: 500
            });

            $(".open_qv").on("click", function (e) {
                var ez = $(this).siblings('img').data('elevateZoom');
                $.fancybox(ez.getGalleryList());
                e.preventDefault();
            });

        }

        /*  [ input number ]
        - - - - - - - - - - - - - - - - - - - - */
        $(".btn-number").on('click', function (e) {

            e.preventDefault();

            var fieldName = $(this).attr('data-field');
            var type = $(this).attr('data-type');
            var input = $("input[name='" + fieldName + "']");
            var currentVal = parseInt(input.val());
            if (!isNaN(currentVal)) {
                if (type == 'minus') {

                    if (currentVal > input.attr('minlength')) {
                        input.val(currentVal - 1).change();
                    }
                    if (parseInt(input.val()) == input.attr('minlength')) {
                        $(this).attr('disabled', true);
                    }

                } else if (type == 'plus') {

                    if (currentVal < input.attr('maxlength')) {
                        input.val(currentVal + 1).change();
                    }
                    if (parseInt(input.val()) == input.attr('maxlength')) {
                        $(this).attr('disabled', true);
                    }

                }
            } else {
                input.val(0);
            }
        });

        /*  [ tab detail ]
        - - - - - - - - - - - - - - - - - - - - */
        $(".product-info-detailed  .block-title").on('click', function () {

            $(this).parent().toggleClass('has-active');
            return false;
        });

        /*  [ Back to top ]
        - - - - - - - - - - - - - - - - - - - - */
        $(window).scroll(function () {
            if ($(this).scrollTop() > 50) {
                $('.back-to-top').fadeIn();
            } else {
                $('.back-to-top').fadeOut();
            }
        });
        $('.back-to-top').on('click', function (e) {
            e.preventDefault();
            $("html, body").animate({
                scrollTop: 0
            }, 500);
        });

        /*  [ All Categorie ]
        - - - - - - - - - - - - - - - - - - - - */
        $(document).on('click', '.open-cate', function () {
            $(this).closest('.block-nav-categori').find('li.cat-link-orther').each(function () {
                $(this).slideDown();
            });
            $(this).addClass('colse-cate').removeClass('open-cate').html('Close');
            return false;
        })
        /* Close Categorie */
        $(document).on('click', '.colse-cate', function () {
            $(this).closest('.block-nav-categori').find('li.cat-link-orther').each(function () {
                $(this).slideUp();
            });
            $(this).addClass('open-cate').removeClass('colse-cate').html('All Categories');
            return false;
        })

        /*  [ All Categorie ]
        - - - - - - - - - - - - - - - - - - - - */
        $(document).on('click', '.col-categori .btn-show-cat', function () {
            $(this).closest('.col-categori').find('li.cat-orther').each(function () {
                $(this).slideDown();
            });
            $(this).addClass('btn-close-cat').removeClass('btn-show-cat').html('Close <i class="fa fa-angle-double-right" aria-hidden="true"></i>');
            $(this).parent().addClass('open');
            return false;
        })
        /* Close Categorie */
        $(document).on('click', '.col-categori .btn-close-cat', function () {
            $(this).closest('.col-categori').find('li.cat-orther').each(function () {
                $(this).slideUp();
            });
            $(this).parent().removeClass('open');
            $(this).addClass('btn-show-cat').removeClass('btn-close-cat').html('All Categories <i class="fa fa-angle-double-right" aria-hidden="true"></i>');
            return false;
        })

        /*  [ All Categorie Sticky]
        - - - - - - - - - - - - - - - - - - - - */



        /*  [ Sticky Menu ]
         - - - - - - - - - - - - - - - - - - - - */
        //$('.mid-header ').sticky({ topSpacing: 0 });

        /*  [ Banner top ]
         - - - - - - - - - - - - - - - - - - - - */
        $('.qc-top-site  .close').on('click', function () {

            $(this).parents(".qc-top-site").slideUp("slow");
            $(this).parents(".qc-top-site").addClass('close-bn');
            $(".qc-top-site ").css({ "min-height": "0", "opacity": "0" });
            return false;
        });

        /*  [ Sticky Menu ]
         - - - - - - - - - - - - - - - - - - - - */


        if ($('.categori-search-option').length) {
            $(".categori-search-option").chosen({

            });
        }


        /** #brand-showcase */
        $(document).on('click', '.block-brand-tabs .nav-brand li', function () {
            var id = $(this).data('tab');
            $(this).closest('.block-brand-tabs').find('li').each(function () {
                $(this).removeClass('active');
            });
            $(this).closest('li').addClass('active');
            $('.block-brand-tabs').find('.tab-pane').each(function () {
                $(this).removeClass('active');
            })
            $('#' + id).addClass('active');
            return false;
        })

        $('.featured-news-slider').owlCarousel({
            loop: true,
            margin: 10,
            nav: true,
            responsive: {
                0: {
                    items: 1
                },
                600: {
                    items: 1
                },
                1000: {
                    items: 1
                }
            }
        })

        /*  [ popup - newsletter]
         - - - - - - - - - - - - - - - - - - - - */
        if ($('#popup-newsletter').length > 0) {
            $('#popup-newsletter').modal({
                keyboard: false
            })
        }

        //close js alert
        $(document).on('click', '.alert__close', function () {
            if ($(this).closest('.js-alert')) {
                $(this).closest('.js-alert').remove();
            }
            return false;
        })
        //load shopping cart

        $('#subcribe').click(function (e) {
            e.preventDefault();
            if ($('#register-email input[name="Email"]').val() == '') {
                $('#errorLogin1').html('Email không được để trống');
                return false;
            } else if (!validateEmail($('#register-email input[name="Email"]').val())) {
                $('#errorLogin1').html('Địa chỉ email không hợp lệ');
                return false;
            } else {
                $('#errorLogin1').html('');
            }

            var data = getFormData($('#register-email'));

            $.ajax({
                method: "POST",
                url: "/Home/Newsletter",
                data: {
                    email: data.Email
                }
            }).done(function (res) {
                if (res) {
                    if (!res.success) {
                        createJSAlert(res.message, 'error');
                    } else {
                        $('#errorLogin').html('');
                        createJSAlert(res.message, 'success');
                    }
                }
            });
        });

        jQuery('#select_ttp').change(function () {

            var province = $('#select_ttp').val();
            GetDistrictByProvince(province, null);
        });

        var getUrlParameter = function getUrlParameter(sParam) {
            var sPageURL = decodeURIComponent(window.location.search.substring(1)),
                sURLVariables = sPageURL.split('&'),
                sParameterName,
                i;

            for (i = 0; i < sURLVariables.length; i++) {
                sParameterName = sURLVariables[i].split('=');

                if (sParameterName[0] === sParam) {
                    return sParameterName[1] === undefined ? true : sParameterName[1];
                }
            }
        };

        var GetDistrictByProvince = function (province, district) {

            $.ajax({
                //url: '@Url.Action("SelectTTP", "Agency")',
                url: '/Agency/SelectTTP',
                data: { 'province': province },
                success: function (result, status, xhr) {
                    if (result.Data.length > 0) {
                        if (district != null && district != " ") {
                            $("#select_qh option:selected").removeAttr('selected');

                        }
                        var html = "";
                        var optionHtml = "";
                        var idx = 0;
                        for (var i in result.Data) {
                            idx++;
                            var checkSelected = district != null && district != " " && result.Data[i].Name.indexOf(district) > -1 ? 'selected="selected"' : "";
                            html += '<li data-original-index="' + idx + '" class="">'
                                + '<a tabindex="0" class="" data-tokens="null" role="option" aria-disabled="false" aria-selected="false">'
                                + '<span class="text">' + result.Data[i].Name + '</span>'
                                + '<span class="glyphicon glyphicon-ok check-mark"></span>'
                                + '</a>'
                                + '</li>';

                            optionHtml += '<option value="' + result.Data[i].Name + '">' + result.Data[i].Name + '</option>'
                        }
                        $(".select_qh .dropdown-menu.inner").append(html);
                        $("#select_qh").append(optionHtml);
                    }
                },
                error: function (xhr, status, error) {

                }
            });
        }

        if (getUrlParameter('province') != null) {
            var province = getUrlParameter('province').indexOf('+') ? getUrlParameter('province').replace('+', ' ') : getUrlParameter('province');
            var dis = getUrlParameter('district').indexOf('+') ? getUrlParameter('district').replace('+', ' ') : getUrlParameter('district');
            GetDistrictByProvince(province, dis);
        }

        $('.input-search').keypress(function (event) {

            var keycode = (event.keyCode ? event.keyCode : event.which);
            if (keycode == '13') {

                location.href = "/danh-muc?s=" + $(this).val();;
            }
        });
        $('.btn-search').click(function () {
            location.href = "/danh-muc?s=" + $(this).parent().find('.input-search').val();
            return;
        })

    });
})(jQuery);

function closeCart() {
    $(this).toggleClass('active');
    $(".block-nav-cart").toggleClass("has-open");
    $("body").toggleClass("menu-open");
    return false;
}
//custom js
function formatVND(num) {
    var p = parseFloat(num).toFixed(2).split(".");
    return p[0].split("").reverse().reduce(function (acc, num, i, orig) {
        return num == "-" ? acc : num + (i && !(i % 3) ? "," : "") + acc;
    }, "") + ' Đ';
}

function getFormData($form) {
    var unindexed_array = $form.serializeArray();
    var indexed_array = {};

    $.map(unindexed_array, function (n, i) {
        indexed_array[n['name']] = n['value'];
    });

    return indexed_array;
}

function LoadCart(shippingCartByUser, shippingAmount, rangeShippingAmount) {

    if (shippingCartByUser == null || shippingCartByUser == undefined || shippingCartByUser == '') {
        var shoppingcart = localStorage.getItem('CartMBF');
        if (shoppingcart == undefined || shoppingcart == '') {
            $('#smartcart').smartCart();
        } else {
            var shoppingcartObj = JSON.parse(shoppingcart);
            $('#smartcart').smartCart({ cart: shoppingcartObj, shippingAmount: shippingAmount, rangeShippingAmount: rangeShippingAmount });
        }
    } else {
        var shoppingcartObj = JSON.parse(shippingCartByUser)
        $('#smartcart').smartCart({ cart: shoppingcartObj, shippingAmount: shippingAmount, rangeShippingAmount: rangeShippingAmount });
    }
}

function validateEmail(email) {
    var re = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    return re.test(email);
}

function validatePhone(phone) {
    var re = /^(0|\+84|\(\+84\))\d{9,11}$/;
    return re.test(phone);
}

function validatePassword(password) {
    var re = /^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{6,}$/;
    return re.test(password);
}

function createJSAlert(message, type) {
    debugger;
    var alertId = 'alert' + new Date().getTime();
    var template = '<div data-alert="" class="alert-wrap text-center js-alert">'
        + '<div id="' + alertId + '" class="alert alert--' + type + '">'
        + '<div class="alert__inner">'
        + '<p class="alert__text">'
        + message
        + '<a class="alert__close float--right js-flash-close" href="javascript:;"><i>&nbsp;</i><i>&nbsp;</i></a>'
        + '</p>'
        + '</div>'
        + '</div>'
        + '</div>';
    $('.alerts-wrapper.js-alerts').prepend(template);
    setTimeout(function () {
        if ($('#' + alertId)) $('#' + alertId).remove();
    }, 5000);
}

function convertStringToDate(date, separate) {
    if (!date) return null;
    var comp = date.split(separate);
    if (comp.length < 3) return null;
    var d = parseInt(comp[0], 10);
    var m = parseInt(comp[1], 10);
    var y = parseInt(comp[2], 10);
    var dt = new Date(y, m - 1, d);

    if (dt.getFullYear() == y && dt.getMonth() + 1 == m && dt.getDate() == d) {
        return new Date(y, m, d);
    } else {
        return null;
    }
}

function scrollToElementById(el) {
    if (el && el.length > 0) {
        var $element = $('#' + el);
        if ($element) {
            $('html, body').animate({
                scrollTop: $element.offset().top - 57
            }, 1000);
        }
    }
}

//(function ($) {
//    "use strict";
//    $(".xzoom").xzoom({ tint: '#333', Xoffset: 15 });
//    LoadCart('', '0', '0');
//})(jQuery);