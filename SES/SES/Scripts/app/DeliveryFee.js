var numHeight = 380;
var keyAction;
var indexTabstripActive = -1;
var contentTab;
$(document).ready(function () {

    //active menu
    resetMenu();
    activeMenu(1, 2, 1, '2_1', 'menu_DeliveryFee');

    document.title = "Biểu phí vận chuyển";

    //fillter & form popup
    $("#selectIsActive_search").select2();
    $("#selectIsActive_search").css('width', '100%');
 
    

    $(window).resize(function () {
        resizeGrid(numHeight);
    });

    //goi ham search khi nhan enter
    $("#txtDeliveryFeeID").keypress(function (e) {
        if (e.keyCode == 13) {
            doSearchpackage();
        }
    });
    $("#txtDVVCID").keypress(function (e) {
        if (e.keyCode == 13) {
            doSearchpackage();
        }
    });
    $("#txtDeliveryFeeTID").keypress(function (e) {
        if (e.keyCode == 13) {
            doSearchterritory();
        }
    });
    $("#txtProvince").keypress(function (e) {
        if (e.keyCode == 13) {
            doSearchterritory();
        }
    });
    $("#txtDistrict").keypress(function (e) {
        if (e.keyCode == 13) {
            doSearchterritory();
        }
    });

   
    //Ctr + S luu form
    $(document).bind('keydown', function (event) {
        if (eventHotKey) {
            if (event.ctrlKey || event.metaKey) {
                switch (String.fromCharCode(event.which).toLowerCase()) {
                    case 's':
                        
                        event.preventDefault();
                        if ($("#btnInsert").length > 0 ) {
                            $("#formPopupRe").submit();
                        }
                        break;
                }
            }
        }        
    });
});

//========================================== code co ban ====================================

//search
function doSearchpackage() {
    var grid = $("#gridpackage").data("kendoGrid");
    var filter = { logic: "and", filters: [] };    
    var text = $("#txtDeliveryFeeID").val();
    if (text) {

        var filterOr = { logic: "or", filters: [] };
        if ($.isNumeric(text)) {
            filterOr.filters.push({ field: "DeliveryFeeID", operator: "eq", value: text });
        }
        filterOr.filters.push({ field: "Name", operator: "contains", value: text });
        filter.filters.push(filterOr);
    }
    var text = $("#txtDVVCID").val();
    if (text) {
        var filterOr = { logic: "or", filters: [] };
        filterOr.filters.push({ field: "TransporterID", operator: "contains", value: text });
        filterOr.filters.push({ field: "DeliveryName", operator: "contains", value: text });
        filter.filters.push(filterOr);
    }
    var text = $("#selectIsActive_search").val();
    if (text) {
        filter.filters.push({ field: "Status", operator: "eq", value: text });
    }
    grid.dataSource.filter(filter);
}
function doSearchterritory() {
    var grid = $("#gridterritory").data("kendoGrid");
    var filter = { logic: "and", filters: [] };
    var text = $("#txtDeliveryFeeID").val();
    if (text) {
        var filterOr = { logic: "or", filters: [] };
        if ($.isNumeric(text)) {
            filterOr.filters.push({ field: "DeliveryFeeID", operator: "eq", value: text });
        }
        filterOr.filters.push({ field: "Name", operator: "contains", value: text });
        filter.filters.push(filterOr);
    }
    text = $("#txtProvince").val();
    if (text) {
        var filterOr = { logic: "or", filters: [] };
        filterOr.filters.push({ field: "ProvinceID", operator: "contains", value: text });
        filterOr.filters.push({ field: "ProvinceName", operator: "contains", value: text });
        filterOr.filters.push({ field: "PickingProvinceID", operator: "contains", value: text });
        filterOr.filters.push({ field: "PickingProvinceName", operator: "contains", value: text });
        filter.filters.push(filterOr);
    }
    text = $("#txtDistrict").val();
    if (text) {
        var filterOr = { logic: "or", filters: [] };
        filterOr.filters.push({ field: "DistrictID", operator: "contains", value: text });
        filterOr.filters.push({ field: "DistrictName", operator: "contains", value: text });
        filterOr.filters.push({ field: "PickingDistrictName", operator: "contains", value: text });
        filterOr.filters.push({ field: "PickingDistrictID", operator: "contains", value: text });
        filter.filters.push(filterOr);
    }
   
    grid.dataSource.filter(filter);
}
//grid 
function onDataboundPackage(e) {
    resizeOtherGrid(numHeight, $("#gridpackage"));
    var grid = $("#gridpackage").data("kendoGrid");

    // ask the parameterMap to create the request object for you
    var requestObject = (new kendo.data.transports["aspnetmvc-server"]({ prefix: "" }))
    .options.parameterMap({
        page: grid.dataSource.page(),
        sort: grid.dataSource.sort(),
        filter: grid.dataSource.filter()
    });

    var $exportLink = grid.element.find('.export');

    var href = $exportLink.attr('href');
    if (href) {
        href = href.replace(/sort=([^&]*)/, 'sort=' + requestObject.sort || '~');

        href = href.replace(/filter=([^&]*)/, 'filter=' + (requestObject.filter || '~'));

        $exportLink.attr('href', href);
    }
    changeStatusGrid('gridpackage', 6);
    $("#divLoading").hide();
    
}
function onDataboundTerritory(e) {
    resizeOtherGrid(numHeight, $("#gridterritory"));
    var grid = $("#gridterritory").data("kendoGrid");

    // ask the parameterMap to create the request object for you
    var requestObject = (new kendo.data.transports["aspnetmvc-server"]({ prefix: "" }))
    .options.parameterMap({
        page: grid.dataSource.page(),
        sort: grid.dataSource.sort(),
        filter: grid.dataSource.filter()
    });

    var $exportLink = grid.element.find('.export');

    var href = $exportLink.attr('href');
    if (href) {
        href = href.replace(/sort=([^&]*)/, 'sort=' + requestObject.sort || '~');

        href = href.replace(/filter=([^&]*)/, 'filter=' + (requestObject.filter || '~'));

        $exportLink.attr('href', href);
    }
   
}

function readHeaderInfo() {

    contentTab = setContentTab(["DeliveryFeeID", "Name", "Status"], "30");

}
function activeMenu(li, ulRoot, ulItem, ulItem2, idMenu) {
    $("ul#menuLeft").find('li:eq(' + li + ')').addClass('open');
    $("ul#menuLeft").find('li:eq(' + li + ') ul#ul_root_' + ulRoot).css('display', 'block');
    $("ul#menuLeft").find('li:eq(' + li + ') ul#ul_root_' + ulRoot + ' ul#ul_item_' + ulItem).css('display', 'block');
    $("ul#menuLeft").find('li:eq(' + li + ') ul#ul_root_' + ulRoot + ' ul#ul_item_' + ulItem + ' ul#ul_item' + ulItem2).css('display', 'block');
    $("#" + idMenu).parent().addClass('active');

    $("ul#menuLeft").find('li:eq(' + li + ') > a > b > em').removeClass('fa-plus-square-o').addClass('fa-minus-square-o');
    $("ul#menuLeft").find('li:eq(' + li + ') ul#ul_root_' + ulRoot + ' > li:eq(0) > a > b > em').removeClass('fa-plus-square-o').addClass('fa-minus-square-o');
    $("ul#menuLeft").find('li:eq(' + li + ') ul#ul_root_' + ulRoot + ' ul#ul_item_' + ulItem + ' > li:eq(0) > a > b > em').removeClass('fa-plus-square-o').addClass('fa-minus-square-o');
}
function resizeGrid(number) {
    var h_search = $(".divSearch").height();
    var h = parseInt($(window).height()) - h_search;
    var content = $("#grid").find(".k-grid-content");
    content.height(h - number);
}






//========================================== code khac neu co ====================================

