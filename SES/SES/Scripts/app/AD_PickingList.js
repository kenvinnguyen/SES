resetMenu();
$("ul#menuLeft").find('#ul_root_3').addClass('open');
$("ul#menuLeft").find('#ul_root_3').css('display', 'block');
$("ul#menuLeft").find('#ul_root_3 ul#ul_item_2').css('display', 'block');
$("#menu_AD_PickingList").parent().addClass('active');
document.title = "Danh sách picking";
$("a.k-grid-cancel-changes").css({ 'background-color': '#a0a0a0', 'color': 'white' });
function doSearch() {
    var grid = $("#grid").data("kendoGrid");
    var filter = { logic: "and", filters: [] };
    var Name = $("#txtPickingID").val();
    if (Name) {
        var filterOr = { logic: "or", filters: [] };
        filterOr.filters.push({ field: "PickingID", operator: "eq", value: Name });
        filter.filters.push(filterOr);
    }
    //var orderDate = $('#orderDate').val();
    //var dates = kendo.toString(new Date(orderDate), "yyyy-MM-dd");
    //if (dates) {
    //    var filterOr = { logic: "or", filters: [] };
    //    filterOr.filters.push({ field: "SODate", operator: "eq", value: dates });
    //    filter.filters.push(filterOr);
    //    //alert(contains);
    //    //var startdate = orderDate.split('-')[0].trim();
    //    //var enddate = orderDate.split('-')[1].trim();
    //    //var todate = new Date(enddate);
    //    //todate.setDate(todate.getDate() + 1);
    //    //filter.filters.push({ field: "SODate", operator: "gte", value: startdate });
    //    //filter.filters.push({ field: "SODate", operator: "lt", value: todate });
    //}
    grid.dataSource.filter(filter);
}
function onDatabound() {
    resizeGrid();
    var grid = $("#grid").data("kendoGrid");
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
    $("#divLoading").hide();
    
    //GetDetail();
}
function resizeGrid() {
    var offsetbottom = parseInt($(window).height()) - parseInt($('#grid').offset().top);
    var gridElement = $("#grid"),
    dataArea = gridElement.find(".k-grid-content"),
    otherElements = gridElement.children().not(".k-grid-content"),
    otherElementsHeight = 0;
    otherElements.each(function () {
        otherElementsHeight += $(this).outerHeight();
    });
    dataArea.height(offsetbottom - otherElementsHeight - 40);
}
function alertBox(title, content, flag, timeout) {
    //var icon = flag ? "fa-thumbs-up" : "fa-thumbs-down";
    $.smallBox({
        title: title,
        content: content,
        color: flag ? "#e0efd8" : "#f2dedf",
        //iconSmall: "fa " + icon + " bounce animated",
        timeout: timeout
    });
}

function GetDetail() {
    debugger;
    var PickingNumber = $("#PickingNumber").val();
    //alert(PickingNumber);
    $.post(r + "/AD_PickingList/GetByPickingNumber", { id: PickingNumber }, function (data) {
        if (!data.succsess) {
            if (data.PickingNumber != null && data.PickingNumber != "") {
                $("#PrinterName").val(data.PrinterName);
                $('#PickingDate').val(dateToString(data.PickingDate));
                $('#Status').val(data.Status);
                $("#TotalQty").val((data.TotalQty + "").replace(/(\d)(?=(\d{3})+(?!\d))/g, "$1,"));
                $("#TotalAmt").val((data.TotalAmt + "").replace(/(\d)(?=(\d{3})+(?!\d))/g, "$1,"));
            }
        }
    });
}
function onOpenPopupDetail(obj) {
    var row = $(obj).closest('tr');
    var PickingNumber = $(row).find("td:eq(1)").text();
   
    onLoadPage(r + "/AD_PickingList/PartialDetail/" + PickingNumber);
}

function dateToString(date) {

    if (date != null) {
        date = new Date(date.match(/\d+/)[0] * 1);
        var day = date.getDate() < 10 ? "0" + date.getDate() : date.getDate();
        var month = (date.getMonth() + 1) < 10 ? "0" + (date.getMonth() + 1) : (date.getMonth() + 1);
        return day + '/' + month + '/' + date.getFullYear();
    }
    else {
        return "";
    }
}

function onDataPickingNumber() {
    return {
        text: $("#PickingNumber").val()
    };
}
function onDataboundDetail() {
    resizeGridDetail();
    var grid = $("#gridDetail").data("kendoGrid");
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
    $("#divLoading").hide();

    GetDetail();
}
function resizeGridDetail() {
    var offsetbottom = parseInt($(window).height()) - parseInt($('#gridDetail').offset().top);
    var gridElement = $("#gridDetail"),
    dataArea = gridElement.find(".k-grid-content"),
    otherElements = gridElement.children().not(".k-grid-content"),
    otherElementsHeight = 0;
    otherElements.each(function () {
        otherElementsHeight += $(this).outerHeight();
    });
    dataArea.height(offsetbottom - otherElementsHeight - 40);
}
function error_handler(e) {
    if (e.errors) {
        var message = "Errors:\n";
        $.each(e.errors, function (key, value) {
            if ('errors' in value) {
                $.each(value.errors, function () {
                    message += this + "\n";
                });
            }
        });
        alertBox("Báo lỗi! ", message, false, 3000);
    } else {
        alertBox("Thành công! ", "Lưu thành công", true, 3000);
    }
}
function onRequestEnd(e) {
    if (e.type == "update" && !e.response.Errors) {
        alertBox("Thành công! ", "Lưu thành công", true, 3000);
        $("#gridDetail").data("kendoGrid").dataSource.read();
    }
    if (e.type == "create" && !e.response.Errors) {
        alertBox("Thành công! ", "Lưu thành công", true, 3000);
        $("#gridDetail").data("kendoGrid").dataSource.read();
       
    }
}
function checkAll(e) {
    var x = $(e).prop('checked');
    $('#grid').find(".checkrowid").each(function () {
        $(this).prop('checked', x);
    });
}
function PackingComplete() {
    var listrowid = "";
    $("#grid").find(".checkrowid").each(function () {
        if ($(this).prop('checked') == true) {
            listrowid += $(this).attr("id") + '@@@@';
        }
    });
    if (listrowid != null && listrowid != "") {
        if (listrowid != null && listrowid != "") {
            $.post(r + "/AD_PickingList/CompletePicking", { data: listrowid }, function (data) {
                if (data.success) {
                    alertBox("Thành công", "Cập nhật thành công", true, 3000);
                    $('#checkboxcheckAll').prop('checked', false);
                    $("#grid").data("kendoGrid").dataSource.read();
                }
                else {
                    alertBox("Báo lỗi! ", data.message, false, 3000);
                    $("#loading").addClass('hide');
                    console.log(data.message);
                }

            });
        }
    }
    else {
        alertBox("Báo lỗi! ", "Vui lòng chọn dữ liệu để hoàn thành Picking.", false, 3000);
        return;
    }
}

function ChangeLocation() {
    if ($("#WHID").val() != "" && $("#WHID").val() != null) {
        $("#WHLID").val('').trigger('change');
        $.post(r + "/AD_Order/GetLocationhByWareHouseID", { WHID: $("#WHID").val() }, function (data) {
            if (!data.success) {
                $("#WHLID").removeAttr("disabled");
                $("#WHLID").html('');
                var html = "<option value =''>Chọn vị trí kho</option>";
                for (var i = 0; i < data.length ; i++) {
                    html += "<option value ='" + data[i].WHLID + "'>" + data[i].WHLName + "</option>";
                }
                $("#WHLID").html(html);
            }
        });
    }
    else {
        $("#WHLID").val('').trigger('change');
        $("#WHLID").prop("disabled", true);
    }
}

$("#WHID").select2();
$("#s2id_WHID").css('width', '100%');
$("#WHLID").select2();
$("#s2id_WHLID").css('width', '100%');
$('#popupTransaction').kendoWindow({
    width: "500px",
    actions: ["Close"],
    title: "Nhập kho",
    visible: false,
    resizable: false
});
function PickingOut() {
    $("#WHLID").val('').trigger('change');
    $("#WHID").val('').trigger('change');
    $("#WHLID").prop("disabled", true);
    idPopup = ".k-window";
    var listrowid = "";
    $("#grid").find(".checkrowid").each(function () {
        if ($(this).prop('checked') == true) {
            listrowid += $(this).attr("id") + '@@@@';
        }
    });
    if (listrowid != null && listrowid != "") {
        $("#divMaskPopup").show();
        var popup = $('#popupTransaction').data("kendoWindow");
        popup.center().open();
    }
    else {
        alertBox("Báo lỗi! ", "Vui lòng chọn dữ liệu để xuất kho.", false, 3000);
        return;
    }
}

function Save() {
    var listrowid = "";
    var WHID = $('#WHID').val();
    var WHLID = $('#WHLID').val();
    $("#grid").find(".checkrowid").each(function () {
        if ($(this).prop('checked') == true) {
            listrowid += $(this).attr("id") + '@@@@';
        }
    });
    if (listrowid != null && listrowid != "" && WHID != null && WHID != "" && WHLID != "" && WHLID != null) {
        $.post(r + "/AD_PickingList/PickingOut", { data: listrowid, WHID: WHID, WHLID: WHLID }, function (data) {
            $("#divMaskPopup").show();
            if (data.success) {
                alertBox("Thành công!", " Lưu thành công", true, 3000);
                $('#checkboxcheckAll').prop('checked', false);
                $("#divMaskPopup").hide();
                $("#popupTransaction").data("kendoWindow").close();
                $("#grid").data("kendoGrid").dataSource.read();
                $("#WHID").val('').trigger('change');
                $("#WHLID").val('').trigger('change');
            }
            else {
                alertBox("Báo lỗi! ", data.message, false, 3000);
                $("#loading").addClass('hide');
                $("#divMaskPopup").hide();
                $("#popupTransaction").data("kendoWindow").close();
                console.log(data.message);
            }

        });

    }
    else {
        alertBox("Báo lỗi! ", "Vui lòng chọn dữ liệu để nhập kho.", false, 3000);
        return;
    }
}
