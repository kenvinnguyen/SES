var keyAction;
var checkItemCode, checkGird;
if ($("#SONumber").val() == "") {
    checkItemCode = true;
}
if ($("#SONumber").val() != "") {
    checkItemCode = false;
}
$(document).ready(function () {
    resetMenu();
    $("ul#menuLeft").find('#ul_root_3').addClass('open');
    $("ul#menuLeft").find('#ul_root_3').css('display', 'block');
    $("ul#menuLeft").find('#ul_root_3 ul#ul_item_1').css('display', 'block');
    $("#menu_AD_Order").parent().addClass('active');

    document.title = "Danh sách yêu cầu";
    $("#selectStatus_search").select2();
    $("#s2id_selectStatus_search").css('width', '100%');
    $("#WHID").select2();
    $("#seachWH").select2();
    $("#seachWHL").select2()
    $("#seachMerchant").select2()
   
    $("#s2id_seachWH").css('width', '100%');
    $("#s2id_seachWHL").css('width', '100%');
    $("#s2id_seachMerchant").css('width', '100%');
    $("#s2id_WHID").css('width', '100%');
   
    $("#WHLID").select2();
    $("#s2id_WHLID").css('width', '100%');
    $("#PrinterID").select2();
    $("#s2id_PrinterID").css('width', '100%');
    $("#PrinterID2").select2();
    $("#s2id_PrinterID2").css('width', '100%');
    if ($("#btnInsertHeader").length > 0) {
        $(document).keypress(function (e) {
            if (e.keyCode == 43) {  // 43 : +
                Create();
            }
        });
    } 
    $("#ItemCode, #Qty").keypress(function (e) {
        if (e.keyCode == 13) {
            $('#formHeader').submit();
        }
    });
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
                required: checkItemCode,
            },
            Qty: {
                required: true,
                number: true,
                min:1,
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
                number: "Số lượng phải là số",
                min:"Số lượng phải lớn hơn 0",
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
                    if (data.success) {
                       
                        if (keyAction == 0 || checkGird == true) {
                            onLoadPage(r + "/AD_Order/PartialDetail/" + data.SONumber);
                        }
                        $('#ItemCode').val('');
                        $('#ItemCode').focus();
                        alertBox("Thành công!", " Lưu thành công", true, 3000);
                        $("#loading").addClass('hide');
                        keyAction = 1;
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
});
$('#txtDataEnd').kendoDatePicker({
    format: "dd/MM/yyyy"
});
$('#StartDate').kendoDatePicker({
    format: "dd/MM/yyyy"
});
if ($("#SONumber").val() == "") {
    checkGird = true;
    $("#WHLID").prop("disabled", true);
}
if ($("#SONumber").val() != "") {
    checkGird = false;
    $("#WHLID").prop("disabled", false);
}
function CreateHeader() {
    //alert($("#WHID").val());
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

//function onDataWareHouse() {
//    return {
//        WHID: $("#WHID").val()
//    };
//}
function GetDetail() {
    debugger;
    var SONumber = $("#SONumber").val();
    $.post(r + "/AD_Order/GetBySONumber", { id: SONumber }, function (data) {
        if (!data.succsess) {
            if (data.SONumber != null && data.SONumber != "") {
                $("#MerchantID").val(data.MerchantID);
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
    //alert($("#SONumber").val());
    GetDetail();
}
function onOpenPopupDetail(key, obj) {
    var row = $(obj).closest('tr');
    var SONumber = $(row).find("td:eq(1)").text();
    if (key == 1) {
        keyAction = key;
        onLoadPage(r + "/AD_Order/PartialDetail/" + SONumber);
    }
    if (key == 0 ) {
        keyAction = key;

        onLoadPage(r + "/AD_Order/PartialDetail/");
    }
  
}

function checkAll(e) {
    var x = $(e).prop('checked');
    $('#gridHeader').find(".checkrowid").each(function () {
        $(this).prop('checked', x);
    });
}
$('#popupPO').kendoWindow({
    width: "500px",
    actions: ["Close"],
    title: "Ngày mua hàng",
    visible: false,
    resizable: false,
    close: function (e) {
        $("#divMaskPopup").hide();
    }
});
$('#popupPicking').kendoWindow({
    width: "500px",
    actions: ["Close"],
    title: "Tạo Picking",
    visible: false,
    resizable: false,
    close: function (e) {
        $("#divMaskPopup").hide();
    }
});

function CreatePO() {
    idPopup = ".k-window";
    var listrowid = "";
    $("#gridHeader").find(".checkrowid").each(function () {
        if ($(this).prop('checked') == true) {
            listrowid += $(this).attr("id") + '@@@@';
        }
    });
    if (listrowid != null && listrowid != "") {
        $("#divMaskPopup").show();
        var popup = $('#popupPO').data("kendoWindow");
        popup.center().open();
    }
    else {
        alertBox("Báo lỗi! ", "Vui lòng chọn dữ liệu để mua hàng.", false, 3000);
        return;
    }
}   
function CreatePicking() {
    idPopup = ".k-window";
    var listrowid = "";
    $("#gridHeader").find(".checkrowid").each(function () {
        if ($(this).prop('checked') == true) {
            listrowid += $(this).attr("id") + '@@@@';
        }
    });
    if (listrowid != null && listrowid != "") {
        $("#divMaskPopup").show();
        var popup = $('#popupPicking').data("kendoWindow");
        popup.center().open();
    }
    else {
        alertBox("Báo lỗi! ", "Vui lòng chọn dữ liệu tạo picking.", false, 3000);
        return;
    }
}
function Save() {
    var listrowid = "";
    var txtDataEnd = $('#txtDataEnd').val();
    var PrinterID = $('#PrinterID').val();

    $("#gridHeader").find(".checkrowid").each(function () {
        if ($(this).prop('checked') == true) {
            listrowid += $(this).attr("id") + '@@@@';
        }
    });
    if(txtDataEnd != null && txtDataEnd != "" &&  PrinterID != null && PrinterID != ""){
        $.post(r + "/AD_Order/CreatePO", { data: listrowid, printer: PrinterID, delivery: txtDataEnd }, function (data) {
            if(data.success){
                alertBox("Thành công!", " Lưu thành công", true, 3000);
                $('#checkboxcheckAll').prop('checked', false);
                $("#divMaskPopup").hide();
                $("#loading").addClass('hide');
                $("#txtDataEnd").val('');
                $("#PrinterID").val('').trigger('change');
                $("#popupPO").data("kendoWindow").close();
                $("#gridHeader").data("kendoGrid").dataSource.read();
            }
            else{
                alertBox("Báo lỗi! ", data.message, false, 3000);
                $("#loading").addClass('hide');
                $("#popupPO").data("kendoWindow").close();
                console.log(data.message);
            }
        });
    }
    else {
        alertBox("Báo lỗi! ", "Vui lòng chọn ngày giao và nhà in.", false, 3000);
        return;
    }
}

function SavePicking() {
    var listrowid = "";
    var StartDate = $('#StartDate').val();
    var PrinterID = $('#PrinterID2').val();

    $("#gridHeader").find(".checkrowid").each(function () {
        if ($(this).prop('checked') == true) {
            listrowid += $(this).attr("id") + '@@@@';
        }
    });
    if (txtDataEnd != null && txtDataEnd != "" && PrinterID != null && PrinterID != "") {
        $.post(r + "/AD_Order/CreatePicking", { data: listrowid, printer: PrinterID, pickingdate: StartDate }, function (data) {
            if (data.success) {
                alertBox("Thành công!", " Lưu thành công", true, 3000);
                $('#checkboxcheckAll').prop('checked', false);
                $("#divMaskPopup").hide();
                $("#loading").addClass('hide');
                $("#StartDate").val('');
                $("#PrinterID2").val('').trigger('change');
                $("#popupPicking").data("kendoWindow").close();
                $("#gridHeader").data("kendoGrid").dataSource.read();
            }
            else {
                alertBox("Báo lỗi! ", data.massege, false, 3000);
                $("#loading").addClass('hide');
                $("#divMaskPopup").hide();
                $("#popupPicking").data("kendoWindow").close();
                console.log(data.massege);
            }
        });
    }
    else {
        alertBox("Báo lỗi! ", "Vui lòng chọn ngày tạo và nhà in.", false, 3000);
        return;
    }
}

$("#seachWH").change(function () {
    if ($("#seachWH").val() != "" && $("#seachWH").val() != null) {
        //$("#WHLID").val('').trigger('change');
        $("#seachWHL").removeAttr("disabled");
        $.post(r + "/AD_Order/GetLocationhByWareHouseID", { WHID: $("#seachWH").val() }, function (data) {
            if (!data.success) {
                $("#seachWHL").html('');
                var html = "<option value =''>-- Chọn vị trí kho --</option>";
                for (var i = 0; i < data.length ; i++) {
                    html += "<option value ='" + data[i].WHLID + "'>" + data[i].WHLName + "</option>";
                }
                $("#seachWHL").html(html);
            }
        });
    }
    else {
        $("#seachWHL").val('').trigger('change');
        $("#seachWHL").prop("disabled", true);
    }
});

function doSearch() {
    var grid = $("#gridHeader").data("kendoGrid");
    var filter = { logic: "and", filters: [] };
    var Name = $("#txtNumberSO").val();
    if (Name) {
        var filterOr = { logic: "or", filters: [] };
        filterOr.filters.push({ field: "SONumber", operator: "eq", value: Name });
        filter.filters.push(filterOr);
    }
    var text = $("#seachWH").val();
    if (text) {
        var filterOr = { logic: "or", filters: [] };
        filterOr.filters.push({ field: "WHID", operator: "eq", value: text });
        filter.filters.push(filterOr);
    }
    var text = $("#seachWHL").val();
    if (text) {
        var filterOr = { logic: "or", filters: [] };
        filterOr.filters.push({ field: "WHLID", operator: "eq", value: text });
        filter.filters.push(filterOr);
    }
    var text = $("#seachMerchant").val();
    if (text) {
        var filterOr = { logic: "or", filters: [] };
        filterOr.filters.push({ field: "MerchantID", operator: "eq", value: text });
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

$(".k-window span.k-i-close").click(function () {
    $("#divMaskPopup").hide();
});

function CancelSO() {
    var listrowid ="";
    $("#gridHeader").find(".checkrowid").each(function () {
        if ($(this).prop('checked') == true) {
            listrowid += $(this).attr("id") + '@@@@';
        }
    });
    if (listrowid != null && listrowid != "") {
        $.post(r + "/AD_Order/CancelSalesOrder", { data: listrowid }, function (data) {
            if (data.success) {
                alertBox("Thành công", "Hủy thành công", true, 3000);
                $('#checkboxcheckAll').prop('checked', false);
                $("#gridHeader").data("kendoGrid").dataSource.read();
            }
            else {
                alertBox("Báo lỗi! ", data.message, false, 3000);
                $("#loading").addClass('hide');
                console.log(data.message);
            }
           
        });
    }
    else {
        alertBox("Báo lỗi! ", "Vui lòng chọn dữ liệu để hủy.", false, 3000);
        return;
    }
}
function ChangeLocation() {
    if ($("#WHID").val() != "" && $("#WHID").val() != null) {
        $("#WHLID").val('').trigger('change');
        $("#WHLID").removeAttr("disabled");
        $.post(r + "/AD_Order/GetLocationhByWareHouseID", { WHID: $("#WHID").val() }, function (data) {
            if (!data.success) {
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