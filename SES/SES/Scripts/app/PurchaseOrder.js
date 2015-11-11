document.title = "Tạo phiếu mua";
resetMenu()
$("ul#menuLeft").find('#ul_root_3').addClass('open');
$("ul#menuLeft").find('#ul_root_3').css('display', 'block');
$("ul#menuLeft").find('#ul_root_3 ul#ul_item_1').css('display', 'block');
$("#menu_PurchaseOrder").parent().addClass('active');


var keyAction;
var checkItemCode, checkGird;
if ($("#PONumber").val() == "") {
    checkItemCode = true;
}
if ($("#PONumber").val() != "") {
    checkItemCode = false;
}
$("#selectStatus_search").select2();
$("#s2id_selectStatus_search").css('width', '100%');

$("#seachPrinter").select2();
$("#s2id_seachPrinter").css('width', '100%');
$("#WHID").select2();
$("#s2id_WHID").css('width', '100%');
$("#WHLID").select2();
$("#s2id_WHLID").css('width', '100%');
function onDataboundHeader() {
    resizeGridHeader();
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

function resizeGridHeader() {
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

function onOpenPopupDetail(key, obj) {
    var row = $(obj).closest('tr');
    var PONumber = $(row).find("td:eq(1)").text();
    //onLoadPage(r + "/PurchaseOrder/PartialDetail/" + PONumber);
    if (key == 1) {
        keyAction = key;
        onLoadPage(r + "/PurchaseOrder/PartialDetail/" + PONumber);
    }
    if (key == 0) {
        keyAction = key;
        onLoadPage(r + "/PurchaseOrder/PartialDetail/");
    }
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

function onDataVendor() {
    return {
        text: $("#VendorID").val()
    };
}
function onDataItem() {
    return {
        text: $("#ItemCode").val()
    };
}
function onDataPONumber() {
    return {
        text: $("#PONumber").val()
    };
}
function CreateHeader() {
    $('#formHeader').submit();
}
$("#ItemCode, #Qty").keypress(function (e) {
    if (e.keyCode == 13) {
        $('#formHeader').submit();
    }
});
$("#formHeader").validate({
    // Rules for form validation
    rules: {
        PrinterID: {
            required: true,
            //alphanumeric: true
        },
        
        PODate: {
            required: true,
        },
        ItemCode: {
            required: checkItemCode,
        },
        Qty: {
            required: true,
            number: false,
            min: 1,
        },
        DeliveryDate: {
            required: true,
        },
    },

    // Messages for form validation
    messages: {
        PrinterID: {
            required: "Thông tin bắt buộc"
        },
        //WHLID: {
        //    required: "Thông tin bắt buộc"
        //},
        PODate: {
            required: "Thông tin bắt buộc",
        },
        ItemCode: {
            required: "Thông tin bắt buộc"
        },
        Qty: {
            required: "Thông tin bắt buộc",
            number: "Số lượng phải là số",
            min: "Số lượng phải lớn hơn 0"
        },
        DeliveryDate: {
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
                $("#divMaskPopup").hide();
                $("#gridDetail").data("kendoGrid").dataSource.read();
                if (data.success) {
                    if (keyAction == 0 || checkGird == true) {
                        onLoadPage(r + "/PurchaseOrder/PartialDetail/" + data.PONumber);
                    }
                    $('#ItemCode').val('');
                    $('#ItemCode').focus();
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
if ($("#PONumber").val() == "") {
    checkGird = true;
}
if ($("#PONumber").val() != "") {
    checkGird = false;
}

function GetDetail() {
    var PONumber = $("#PONumber").val();
    $.post(r + "/PurchaseOrder/GetByPONumber", { id: PONumber }, function (data) {
        if (!data.succsess) {
            if (data.PONumber != null && data.PONumber != "") {
                //$("#PONumber").val(data.PONumber);
                $("#VendorID").val(data.PrinterID);
                //$("#PrinterID").attr('readonly', true);
                //$('#PODate').attr('readonly', true);
                $("#Note").attr('readonly', true);
                //$('#DeliveryDate').attr('readonly', true);
                $("#Note").val(data.Note);
                $('#PODate').val(dateToString(data.PODate));
                $('#DeliveryDate').val(dateToString(data.DeliveryDate));
                $("#TotalQty").val((data.TotalQty + "").replace(/(\d)(?=(\d{3})+(?!\d))/g, "$1,"));
                $("#TotalAmt").val((data.TotalAmt + "").replace(/(\d)(?=(\d{3})+(?!\d))/g, "$1,"));
            }
        }
    });
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
$("#orderDate").kendoDatePicker();

function checkAll(e) {
    var x = $(e).prop('checked');
    $('#gridHeader').find(".checkrowid").each(function () {
        $(this).prop('checked', x);
    });
}
$('#popupTransaction').kendoWindow({
    width: "500px",
    actions: ["Close"],
    title: "Nhập kho",
    visible: false,
    resizable: false,
    close: function (e) {
        $("#divMaskPopup").hide();
    }
});
function UpdateWareHoues() {
    $("#WHLID").val('').trigger('change');
    $("#WHID").val('').trigger('change');
    $("#WHLID").prop("disabled", true);
    idPopup = ".k-window";
    var listrowid = "";
    $("#gridHeader").find(".checkrowid").each(function () {
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
        alertBox("Báo lỗi! ", "Vui lòng chọn dữ liệu để mua hàng.", false, 3000);
        return;
    }
}
$(".k-window span.k-i-close").click(function () {
    idPopup = ".k-window";
    $("#divMaskPopup").hide();
});

function Save() {
    var listrowid = "";
    var WHID = $('#WHID').val();
    var WHLID = $('#WHLID').val();
    $("#gridHeader").find(".checkrowid").each(function () {
        if ($(this).prop('checked') == true) {
            listrowid += $(this).attr("id") + '@@@@';
        }
    });
    if (listrowid != null && listrowid != "" && WHID != null && WHID != "" && WHLID != "" && WHLID !=null) {
        $.post(r + "/PurchaseOrder/WareHousing", { data: listrowid, WHID: WHID, WHLID: WHLID }, function (data) {
            $("#divMaskPopup").show();
            if (data.success) {
                alertBox("Thành công!", " Lưu thành công", true, 3000);
                $('#checkboxcheckAll').prop('checked', false);
                $("#divMaskPopup").hide();
                $("#popupTransaction").data("kendoWindow").close();
                $("#gridHeader").data("kendoGrid").dataSource.read();
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
function doSearch() {
    var grid = $("#gridHeader").data("kendoGrid");
    var filter = { logic: "and", filters: [] };
    var Name = $("#txtNumberPO").val();
    if (Name) {
        var filterOr = { logic: "or", filters: [] };
        filterOr.filters.push({ field: "PONumber", operator: "eq", value: Name });
        filter.filters.push(filterOr);
    }
    var text = $("#seachPrinter").val();
    if (text) {
        var filterOr = { logic: "or", filters: [] };
        filterOr.filters.push({ field: "PrinterID", operator: "eq", value: text });
        filter.filters.push(filterOr);
    }
    var text = $("#selectStatus_search").val();
    if (text) {
        var filterOr = { logic: "or", filters: [] };
        filterOr.filters.push({ field: "Status", operator: "contains", value: text });
        filter.filters.push(filterOr);
    }  
    grid.dataSource.filter(filter);
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
