! function (a) {
    a(function () {
        var s = a("ul.navbar-nav:not([data-sm-skip])");
        s.each(function () {
            function s() {
                var a = e.getViewportWidth();
                a != o && (e.isCollapsible() ? (i.addClass("sm-collapsible"), i.is("[data-sm-skip-collapsible-behavior]") || n.addClass("navbar-toggle sub-arrow")) : (i.removeClass("sm-collapsible"), i.is("[data-sm-skip-collapsible-behavior]") || n.removeClass("navbar-toggle sub-arrow")), o = a)
            }
            var i = a(this);

            var isRightToLeft = false;
            //START persian-dnnsoftware
            if ($('body').hasClass('rtl') || $('html').attr("lang") == 'fa-IR')
                isRightToLeft = true;
            //END persian-dnnsoftware
            i.addClass("sm").smartmenus({
                subMenusSubOffsetX: 2,
                subMenusSubOffsetY: -6,
                subIndicators: !1,
                collapsibleShowFunction: null,
                collapsibleHideFunction: null,
                rightToLeftSubMenus: isRightToLeft,//i.hasClass("navbar-right"),//persian-dnnsoftware
                bottomToTopSubMenus: i.closest(".navbar").hasClass("navbar-fixed-bottom")
            }).bind({
                "show.smapi": function (s, i) {
                    var e = a(i),
                        o = e.dataSM("scroll-arrows");
                    o && o.css("background-color", a(document.body).css("background-color")), e.parent().addClass("open")
                },
                "hide.smapi": function (s, i) {
                    a(i).parent().removeClass("open")
                }
            }).find("a.current").parent().addClass("active");
            var e = i.data("smartmenus");
            i.is("[data-sm-skip-collapsible-behavior]") && i.bind({
                "click.smapi": function (s, i) {
                    if (e.isCollapsible()) {
                        var o = a(i),
                            n = o.parent().dataSM("sub");
                        if (n && n.dataSM("shown-before") && n.is(":visible")) return e.itemActivate(o), e.menuHide(n), !1
                    }
                }
            });
            var o, n = i.find(".caret");
            s(), a(window).bind("resize.smartmenus" + e.rootId, s)
        })
    }), a.SmartMenus.prototype.isCollapsible = function () {
        //START persian-dnnsoftware
        if ($('body').hasClass('rtl') || $('html').attr("lang") == 'fa-IR')
            return "right" != this.$firstLink.parent().css("float")
        else
            return "left" != this.$firstLink.parent().css("float")
        //END persian-dnnsoftware
    }
}(jQuery);