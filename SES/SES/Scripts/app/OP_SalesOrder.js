$(document).ready(function () {
    document.title = "Danh sách yêu cầu";
    resetMenu()
    $("#menu_SalesOrder").parent().addClass('active');
    $("#selectStatus_search").select2();
    $("#s2id_selectStatus_search").css('width', '100%');
    $("#WHID").select2();
    $("#s2id_WHID").css('width', '100%');
    $("#WHLID").select2();
    $("#s2id_WHLID").css('width', '100%');
    if ($("#btnInsertHeader").length > 0) {
        $(document).keypress(function (e) {
            if (e.keyCode == 43) {  // 43 : +
                Create();
            }
        });
    }
    $("#formHeader").validate({
        // Rules for form validation
        rules: {
            WHID: {
                required: true,
                //alphanumeric: true
            },
            WHLID: {
                required: true,
            },
            MerchantID: {
                required: true,
            },
            ItemCode: {
                required:true,
            },
            Qty: {
                required: true,
                number:true,
            },
            SODate: {
                required: true,
            },
        },

        // Messages for form validation
        messages: {
            WHID: {
                required: "Thông tin bắt buộc"
            },
            WHLID: {
                required: "Thông tin bắt buộc"
            },
            MerchantID: {
                required: "Thông tin bắt buộc"
            },
            ItemCode: {
                required: "Thông tin bắt buộc"
            },
            Qty: {
                required: "Thông tin bắt buộc",
                number: "Số lượng phải là số"
            },
            SODate: {
                required: "Thông tin bắt buộc",
            },
        },
        errorPlacement: function (error, element) {
            error.insertAfter(element);
        },
        submitHandler: function (form) {
            $(form).ajaxSubmit({
                beforeSend: function () { $("#loading").removeClass('hide'); },
                success: function (data) {
                    debugger;
                    $("#gridDetail").data("kendoGrid").dataSource.read();
                    $("#gridDetail").data("kendoGrid").dataSource.transport.options.read.url = r + "/SalesOrder/ReadDetail";
                    if (data.success) {
                        $('#ItemCode').val('');
                        
                        alertBox("Thành công!", " Lưu thành công", true, 3000);
                        $("#loading").addClass('hide');
                    }
                    else {
                        alertBox("Báo lỗi! ", data.message, false, 3000);
                        $("#loading").addClass('hide');
                        console.log(data.message);
                    }
                }
            });
            return false;
        }
    });
});
$('.input-mask-date').mask('99/99/9999');
function CreateHeader() {
    $('#formHeader').submit();
}
function onDataMerchant() {
    return {
        text: $("#MerchantID").val()
    };
}
function onDataItem() {
    return {
        text: $("#ItemCode").val()
    };
}
function GetDetail() {
    debugger;
    var SONumber = $("#SONumber").val();
    $.post(r + "/OP_SalesOrder/GetBySONumber", { id: SONumber }, function (data) {
        if (!data.succsess) {
            if (data.SONumber != null && data.SONumber != "") {
                $("#MerchantID").val(data.MerchantID);
                //$("#MerchantID").val(data.MerchantID);
                $("#MerchantID").attr('readonly', true);
                $('#SODate').attr('readonly', true);
                $("#Note").attr('readonly', true);
                $('#WHID').prop('disabled', true).trigger('change');
                $('#WHLID').prop('disabled', true).trigger('change');
                $("#Note").val(data.Note);
                $('#SODate').val(dateToString(data.SODate));
                $('#WHID').val(data.WHID).trigger('change');
                $('#WHLID').val(data.WHLID).trigger('change');
                $("#TotalQty").text((data.TotalQty + "").replace(/(\d)(?=(\d{3})+(?!\d))/g, "$1,"));
                $("#TotalAmt").text((data.TotalAmt + "").replace(/(\d)(?=(\d{3})+(?!\d))/g, "$1,"));
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
function onDataSONumber(){
    return {
        text: $("#SONumber").val()
    };
}
function onDataboundHeader() {
    resizeGrid();
    var grid = $("#gridHeader").data("kendoGrid");
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
function resizeGrid() {
    var offsetbottom = parseInt($(window).height()) - parseInt($('#gridHeader').offset().top);
    var gridElement = $("#gridHeader"),
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
function Create() {
    $.post(r + "/OP_SalesOrder/CreateSONew", function (data) {
        if (data.success) {
            onLoadPage(r + "/OP_SalesOrder/PartialDetail/" + data.SONumber);
        }
        else {
            alertBox("Báo lỗi! ", data.message, false, 3000);
            $("#loading").addClass('hide');
            console.log(data.message);
        }
    });
}
function onOpenPopupDetail(obj) {
    var row = $(obj).closest('tr');
    var SONumber = $(row).find("td:eq(1)").text();
    onLoadPage(r + "/OP_SalesOrder/PartialDetail/" + SONumber);
}