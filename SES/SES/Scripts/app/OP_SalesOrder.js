
$(document).ready(function () {
    resetMenu();

    $("ul#menuLeft").find('#ul_root_3').addClass('open');
    $("ul#menuLeft").find('#ul_root_3').css('display', 'block');
    $("ul#menuLeft").find('#ul_root_3 ul#ul_item_1').css('display', 'block');
    $("#menu_OP_SalesOrder").parent().addClass('active');

    var SONumber = "";

    $("a.k-grid-cancel-changes").css({ 'background-color': '#a0a0a0', 'color': 'white' });
    document.title = "Tạo yêu cầu";

    $("#VendorID option").removeAttr('selected');
    generateSelect2("VendorID");
});

function onRequestStart(e) {
    blockUI(false);
}

function error_handler(e) {
    if (e.errors) {
        var message = "";
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
        alertBox("Thành công! ", "Cập nhật thành công", true, 3000);
        onLoadPage(r + "/OP_SalesOrder/PartialCreate/" + SONumber);
        //$("#grid").data("kendoGrid").dataSource.read();
    }
    if (e.type == "create" && !e.response.Errors) {
        alertBox("Thành công! ", "Lưu thành công", true, 3000);
        //$("#grid").data("kendoGrid").dataSource.read();

    }
}
function error_handlerDetail(e) {
    if (e.errors) {
        var message = "";
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
function onRequestEndDetail(e) {
    if (e.type == "update" && !e.response.Errors) {
        alertBox("Thành công! ", "Cập nhật thành công", true, 3000);
        $("#gridDetail").data("kendoGrid").dataSource.read();
    }
    if (e.type == "create" && !e.response.Errors) {
        alertBox("Thành công! ", "Lưu thành công", true, 3000);
        $("#gridDetail").data("kendoGrid").dataSource.read();

    }
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
function onRequestStartDetail(e) {
    GetDetail();
    blockUI(false);
}

function blockUI(isMark) {
    if (isMark) {
        $(document).ajaxStart($.blockUI({ message: '<i class="fa fa-spinner fa-3x fa-lg fa-spin txt-color-blueDark"></i>', theme: false })).ajaxStop($.unblockUI);
    }
    else {
        $(document).ajaxStart($.blockUI({ message: '<i class="fa fa-spinner fa-3x fa-lg fa-spin txt-color-blueDark"></i>', theme: false, overlayCSS: { backgroundColor: 'transparent' } })).ajaxStop($.unblockUI);
    }
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
function KeyCode(e) {
    if (window.event.keyCode > 31 || window.event.keyCode < 90) {
        return false;
    }
}
function SaveAll() {
    if ($('#grid').data('kendoGrid').dataSource.hasChanges() == true) {
        $('#formHeader').submit();
    }
    else {
        alertBox("Báo lỗi! ", "Vui lòng nhập số lượng để tạo.", false, 3000);
    }
}
function Cancel() {
    $('#grid').data('kendoGrid').cancelChanges();
}
function UpdateDetail() {
    if ($('#gridDetail').data('kendoGrid').dataSource.hasChanges() == true && $("#VendorID").val() != "") {
        $('#gridDetail').data('kendoGrid').saveChanges();
    }
    else {
        alertBox("Báo lỗi! ", "Bạn không có quyền cập nhật.", false, 3000);
    }
}

function CancelDetail() {
    $('#gridDetail').data('kendoGrid').cancelChanges();
}
$("#formHeader").validate({
    errorPlacement: function (error, element) {
        error.insertAfter(element);
    },
    submitHandler: function (form) {
        $(form).ajaxSubmit({
            beforeSend: function () { $("#loading").removeClass('hide'); },
            success: function (data) {
                if (data.success) {
                    SONumber = data.SONumber;
                    $("#CreatedAt").val(dateToString(data.CreatedAt));
                    $("#CreatedBy").val(data.CreatedBy);
                    $('#grid').data('kendoGrid').saveChanges();
                    //$('#grid').data('kendoGrid').dataSource.hasChanges() == false;
                    //onLoadPage(r + "/AD_CreateOrder/PartialCreate/" + data.SONumber);
                }
                else {
                    alertBox("Báo lỗi! ", data.message, false, 3000);
                    $("#loading").addClass('hide');
                    console.log(data.message);
                    console.log(checkGird);
                }
            }
        });
        return false;
    }
});

function GetSONumber() {
    return { SONumber: SONumber };
}

function GetSONumberDetail() {
    return { SONumber: $("#SONumber").val() };
}
function GetDetail() {
    debugger;
    var SONumber = $("#SONumber").val();
    $.post(r + "/OP_SalesOrder/GetBySONumber", { id: SONumber }, function (data) {
        if (!data.succsess) {
            if (data.SONumber != null && data.SONumber != "") {
                //$("#MerchantID").val(data.MerchantID);
                //$("#MerchantID").attr('readonly', true);
                $('#SODate').attr('readonly', true);
                $("#Note").attr('readonly', true);
                //$('#WHID').prop('disabled', true).trigger('change');
                // $('#WHLID').prop('disabled', true).trigger('change');
                $("#Note").val(data.Note);
                $('#SODate').val(dateToString(data.SODate));
                $('#WHID').val(data.WHID).trigger('change');
                $('#WHLID').val(data.WHLID).trigger('change');
                $("#TotalQty").val((data.TotalQty + "").replace(/(\d)(?=(\d{3})+(?!\d))/g, "$1,"));
                $("#TotalAmt").val((data.TotalAmt + "").replace(/(\d)(?=(\d{3})+(?!\d))/g, "$1,"));
                $("#CreatedAt").val(dateToString(data.CreatedAt));
                $("#CreatedBy").val(data.CreatedBy);
                $("#SONum").show();
            }
        }
    });
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

function checkAllDetail(e) {
    var x = $(e).prop('checked');
    $('#gridDetail').find(".checkrowid").each(function () {
        $(this).prop('checked', x);
    });
}
function DeleteDetail() {
    var listrowid = "";
    $("#gridDetail").find(".checkrowid").each(function () {
        if ($(this).prop('checked') == true) {
            listrowid += $(this).attr("id") + '@@@@';
        }
    });
    if (listrowid == "" || listrowid == null) {
        alertBox("Báo lỗi!", "Chọn dữ liệu để xóa.", false, 3000);
        return;
    }
    else if ($("#VendorID").val() == "") {
        alertBox("Báo lỗi!", "Bạn không có quyền xóa.", false, 3000);
        return;
    }
    else if (listrowid != null && listrowid != "") {
        var c = confirm("Bạn có chắc chắn muôn xóa.");
        if (c == true) {
            $.post(r + "/OP_SalesOrder/DeleteDetail", { data: listrowid, SONumber: $("#SONumber").val(), }, function (data) {
                if (data.success) {
                    alertBox("Thành công!", " Xóa thành công", true, 3000);
                    $("#gridDetail").data("kendoGrid").dataSource.read();
                    $('#checkboxcheckAllDetail').prop('checked', false);
                }
                else {
                    alertBox("Báo lỗi! ", data.message, false, 3000);
                    $("#gridDetail").data("kendoGrid").dataSource.read();
                }
            });
        }
        else {
            return false;
        }
    }

}