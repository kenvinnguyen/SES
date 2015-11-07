
var numHeight = 380;
$(document).ready(function () {
   
    resetMenu();
    $("ul#menuLeft").find('li:eq(1)').addClass('open');
    $("ul#menuLeft").find('li:eq(1) ul#ul_root_2').css('display', 'block');
    $("ul#menuLeft").find('li:eq(1) ul#ul_root_2 ul#ul_item_2').css('display', 'block');
    //$("ul#menuLeft").find('li:eq(1) ul#ul_root_2 ul#ul_item_1 ul#ul_item2_5').css('display', 'block');
    $("#menu_AdminMasterTerritory").parent().addClass('active');

    document.title = "Vị trí địa lý";
    $("#txtTerritoryIDCountry").keypress(function (e) {

        if (e.keyCode == 13) {
            doSearchCountry();
            return false;
        }
    });
    $("#txtTerritoryIDRegion").keypress(function (e) {
        if (e.keyCode == 13) {

            doSearchRegion();
            return false;
        }
    });
    $("#txtTerritoryIDProvince").keypress(function (e) {
        if (e.keyCode == 13) {

            doSearchProvince();
            return false;
        }
    });
    $("#txtTerritoryIDDistrict").keypress(function (e) {
        if (e.keyCode == 13) {

            doSearchDistrict();
            return false;
        }
    });


    $("#txtTerritoryIDRegionParent").keypress(function (e) {

        if (e.keyCode == 13) {

            doSearchRegion();
            return false;
        }
    });
    $("#txtTerritoryIDProvinceParent").keypress(function (e) {
        if (e.keyCode == 13) {

            doSearchProvince();
            return false;
        }
    });
    $("#txtTerritoryIDDistrictParent").keypress(function (e) {

        if (e.keyCode == 13) {

            doSearchDistrict();
            return false;
        }
    });
    $("#txtTerritoryIDWard").keypress(function (e) {

        if (e.keyCode == 13) {

            doSearchWard();
            return false;
        }
    });
    $("#txtTerritoryIDWardParent").keypress(function (e) {

        if (e.keyCode == 13) {

            doSearchWard();
            return false;
        }
    });
});
//filter Country

function doSearchCountry() {
    debugger;
    var grid = $("#gridCountry").data("kendoGrid");
    var filter = { logic: "and", filters: [] };

    var text = $("#txtTerritoryIDCountry").val();
    if (text) {
        var filterOr = { logic: "or", filters: [] };
        filterOr.filters.push({ field: "TerritoryID", operator: "contains", value: text });
        filterOr.filters.push({ field: "TerritoryName", operator: "contains", value: text });

        filter.filters.push(filterOr);
    }
    grid.dataSource.filter(filter);
}

function doSearchRegion() {
    
    var grid = $("#gridRegion").data("kendoGrid");
    var filter = { logic: "and", filters: [] };

    var text = $("#txtTerritoryIDRegion").val();
    if (text) {
        var filterOr = { logic: "or", filters: [] };
        filterOr.filters.push({ field: "TerritoryID", operator: "contains", value: text });
        filterOr.filters.push({ field: "TerritoryName", operator: "contains", value: text });

        filter.filters.push(filterOr);
    }
    text = $("#txtTerritoryIDRegionParent").val();
    if (text) {
       
        filter.filters.push({ field: "ParentName", operator: "contains", value: text });
    }
    grid.dataSource.filter(filter);
}

function doSearchProvince() {
    
    var grid = $("#gridProvince").data("kendoGrid");
    var filter = { logic: "and", filters: [] };

    var text = $("#txtTerritoryIDProvince").val();
    if (text) {
        var filterOr = { logic: "or", filters: [] };
        filterOr.filters.push({ field: "TerritoryID", operator: "contains", value: text });
        filterOr.filters.push({ field: "TerritoryName", operator: "contains", value: text });

        filter.filters.push(filterOr);
    }
    text = $("#txtTerritoryIDProvinceParent").val();
    if (text) {
      
        filter.filters.push({ field: "ParentName", operator: "contains", value: text });
    }
    grid.dataSource.filter(filter);
}

function doSearchDistrict() {
    
    var grid = $("#gridDistrict").data("kendoGrid");
    var filter = { logic: "and", filters: [] };

    var text = $("#txtTerritoryIDDistrict").val();
    if (text) {
        var filterOr = { logic: "or", filters: [] };
        filterOr.filters.push({ field: "TerritoryID", operator: "contains", value: text });
        filterOr.filters.push({ field: "TerritoryName", operator: "contains", value: text });

        filter.filters.push(filterOr);
    }
    text = $("#txtTerritoryIDDistrictParent").val();
    if (text) {
        filter.filters.push({ field: "ParentName", operator: "contains", value: text });
    }
    grid.dataSource.filter(filter);
}
function doSearchWard() {

    var grid = $("#gridWard").data("kendoGrid");
    var filter = { logic: "and", filters: [] };

    var text = $("#txtTerritoryIDWard").val();
    if (text) {
        var filterOr = { logic: "or", filters: [] };
        filterOr.filters.push({ field: "TerritoryID", operator: "contains", value: text });
        filterOr.filters.push({ field: "TerritoryName", operator: "contains", value: text });

        filter.filters.push(filterOr);
    }
    text = $("#txtTerritoryIDWardParent").val();
    if (text) {
        filter.filters.push({ field: "ParentName", operator: "contains", value: text });
    }
    grid.dataSource.filter(filter);
}


//===========================================Databound ===========================================

function onDataboundCountry(e) {
    resizeOtherGrid(numHeight, $("#gridCountry"));
    var grid = $("#gridCountry").data("kendoGrid");
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

function onDataboundRegion(e) {
    resizeOtherGrid(numHeight, $("#gridRegion"));
    var grid = $("#gridRegion").data("kendoGrid");
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

function onDataboundProvince(e) {
    resizeOtherGrid(numHeight, $("#gridProvince"));
    var grid = $("#gridProvince").data("kendoGrid");
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

function onDataboundDistrict(e) {
    resizeOtherGrid(numHeight, $("#gridDistrict"));
    var grid = $("#gridDistrict").data("kendoGrid");
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
function onDataboundWard(e) {
    resizeOtherGrid(numHeight, $("#gridWard"));
    var grid = $("#gridWard").data("kendoGrid");
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


//===========================================Request===========================================

function onRequestStart(e) {
    blockUIFromUser(false);
    //$("#divLoading").show();
}

function onRequestEnd(e) {
    
    if (e.type == "update" || e.type == "create") {
        if (e.response.Errors == null) {
            alertBox("Thành công! ", "Lưu thành công", "", true, 3000);
        }
        else {
            alertBox("Báo lỗi", e.response.Errors.er.errors[0], false, 3000);
        }
    }

    else {
        arrExport = new Array();
        for (var i = 0; i < e.sender._view.length; i++) {
            var value = e.sender._view[i];
            arrExport.push(value.TerritoryID);
        }
    }
    
    //$("#divLoading").hide();
}