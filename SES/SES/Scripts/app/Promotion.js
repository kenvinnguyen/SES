var numHeight = 320;
var keyAction;
var indexTabstripActive = -1;
var contentTab;
$(document).ready(function () {

    //active menu
    resetMenu();
    $("ul#menuLeft").find('#ul_root_4').addClass('open');
    $("ul#menuLeft").find('#ul_root_4').css('display', 'block');
    $("ul#menuLeft").find('#ul_root_4 ul#ul_item_1').css('display', 'block');

    $("#menu_Promotion").parent().addClass('active');

    document.title = "Chương trình khuyến mãi";

    //fillter & form popup
    $("#selectIsActive_search").select2();
    $("#s2id_selectIsActive_search").css('width', '100%');

    $("#Status").select2();
    $("#s2id_Status").css('width', '240px');

    $("#IsAllMerchant").select2();
    $("#s2id_IsAllMerchant").css('width', '240px');

    $("#IsAllDistric").select2();
    $("#s2id_IsAllDistric").css('width', '240px');
    $("#selectTypePromotion").select2();
    $("#s2id_selectTypePromotion").css('width', '240px');
    //$("#selectTerritory").prop("disabled", true);
    generateSelect2("selectMerchant");
    generateSelect2("selectTerritory");
    generateSelect2("selectTransporter");
    generateSelect2("selectTerritoryProvince");
    $('#StartDate').daterangepicker({
        //ranges: {
        //    'Hôm nay': [moment(), moment()],
        //    'Hôm qua': [moment().subtract('days', 1), moment().subtract('days', 1)],
        //    '7 ngày trước': [moment().subtract('days', 6), moment()],
        //    '30 ngày trước': [moment().subtract('days', 29), moment()],
        //    'Tháng này': [moment().startOf('month'), moment().endOf('month')],
        //    'Tháng trươc': [moment().subtract('month', 1).startOf('month'), moment().subtract('month', 1).endOf('month')]
        //}, format: 'YYYY/MM/DD',
        locale: {
            applyLabel: 'Xác nhận',
            cancelLabel: 'Đóng lại',
            fromLabel: 'Từ',
            toLabel: 'Đến',
            customRangeLabel: 'Tuỳ chọn',
            daysOfWeek: ['T7', 'CN', 'T2', 'T3', 'T4', 'T5', 'T6'],
            monthNames: ['Tháng 1', 'Tháng 2', 'Tháng 3', 'Tháng 4', 'Tháng 5', 'Tháng 6', 'Tháng 7', 'Tháng 8', 'Tháng 9', 'Tháng 10', 'Tháng 11', 'Tháng 12'],
            firstDay: 1,
        }
    });
    //popup
    $('#popupImport').kendoWindow({
        width: "600px",
        actions: ["Close"],
        title: "Nhập Excel",
        visible: false,
        resizable: false
    });
    $('#selectTerritory').change(function () {
        var list = $(this).val();
        if (list == null)
        {
            return;
        }
        if (list.indexOf('0') > -1 && list.length > 1)
        {
            $("#selectTerritory").val([0]);
            setTimeout(function () {
                $("#selectTerritory").trigger("change");
            }, 100);
        }
    });
    $('#selectTerritory').change(function () {
        var list = $(this).val();
        if (list == null) {
            return;
        }
        if (list.indexOf('0') > -1 && list.length > 1) {
            $("#selectTerritory").val([0]);
            setTimeout(function () {
                $("#selectTerritory").trigger("change");
            }, 100);
        }
    });
    //$('#selectTerritoryProvince').change(function () {
    //    var list = $(this).val();
    //    if (list == null) {
    //        return;
    //    }
    //    $.post(r + "/Promotion/getDistrictbyProvince", { id: id, data:text.toString() }, function (data) {
    //        if (data.success) {
    //            $.each(data.data, function (val, text) {
    //                $('#selectTerritory').append(new Option(text, val));
    //            });
    //        }
    //        else {
    //            alertBox("Báo lỗi! ", data.message, false, 3000);
    //            console.log(data.message);
    //        }
    //    });
    //});
    var foo = [];
    $("#importform").ajaxForm({
        beforeSend: function () {
            $("#popupImport").data("kendoWindow").close();
        },
        uploadProgress: function (event, position, total, percentComplete) { },
        success: function (data) {

            if (data.success) {
                $("#grid").data("kendoGrid").dataSource.read();
                hideMaskPopup('#divMaskPopup');
                if (data.errorfile != null && data.errorfile != "") {
                    console.log(data.errorfile);
                    $.SmartMessageBox({
                        title: "Lỗi",
                        content: "Có dòng lỗi, tải về để sửa lại.",
                        buttons: '[Hủy][Tải]'
                    }, function (ButtonPressed) {
                        if (ButtonPressed === "Tải") {

                            //window.location.href = r + "/Error/Download?file=" + data.errorfile;
                            //window.location.href = r + data.errorfile;
                            var locationFileName = data.errorfile;
                            var urlFolder = "ExcelImport\\Error\\";
                            var strParam = "urlFolder=" + urlFolder + "&file=" + locationFileName;
                            window.open(r + "/Home/Download?" + strParam, "_blank");
                        }
                        if (ButtonPressed === "Hủy") {
                            return;
                        }
                    });
                }
                else {
                    alertBox("Thành công!", "Thành công!", true, 3000);
                }
            }
            else {
                alertBox("Chưa nhập được<br>" + data.message, "", false, 3000);
                console.log(data.message);
            }
            $("#divLoading").hide();
        },
        complete: function (xhr) { }
    });

    //form trong popup
    $('#popup').kendoWindow({
        width: "780",
        actions: ["Close"],
        visible: false,
        resizable: false,
    });
    //$("#form3").removeAttr("novalidate");
    
    $("#formPopupDe").validate({
        rules: {
            PromotionID: {
                required: true,
            },
            PromotionName: {
                required: true,
            },
            StartDate: {
                required: true,
            },

        },

        messages: {
            PromotionID: {
                required: "Thông tin bắt buộc",
            },
            PromotionName: {
                required: "Thông tin bắt buộc",
            },
            StartDate: {
                required: "Thông tin bắt buộc",
            },

        },

        errorPlacement: function (error, element) {
            error.insertAfter(element);
        },

        submitHandler: function (form) {
            $(form).ajaxSubmit({
                //clearForm: true,//To clear form after ajax submitting
                beforeSend: function () { $("#loading").removeClass('hide'); },
                success: function (data) {
                    if (data.success) {
                        if (keyAction == 0) {   // Create
                            $("#formPopupDe").find('fieldset:eq(0) section:eq(0)').empty().append('<div class="divlabel"><label class="label" style="float: right;">Mã CCKM: </label></div><div class="divfile"><label style="float: right;text-align: left;width: 240px;height: 19px;padding-top: 2px;"><strong style="color:red;">' + data.PromotionID + '</strong><input type="hidden" id="PromotionID" name="PromotionID" value="' + data.PromotionID + '" /></label> <div style="clear:both"></div>');
                            $("#CreatedAt").val(data.CreatedAt);
                            $("#CreatedBy").val(data.CreatedBy);
                            keyAction = -1;
                        }
                        readHeaderInfo();
                        //$("#popup").data("kendoWindow").close();
                        //$("#divMaskPopup").hide();
                        $("#grid").data("kendoGrid").dataSource.read();
                        alertBox("Thành công !", "Lưu thành công", true, 3000);

                    }
                    else {
                        //$("#divMaskPopup").hide();
                        $("#loading").addClass('hide');
                        alertBox("Báo lỗi", data.message, false, 3000);
                        console.log(data.message);
                    }
                    //$("#divMaskPopup").hide();
                    $("#loading").addClass('hide');
                }
            });
            return false;
        }

    });

    //goi ham search khi nhan enter

    $("#txtPromotionID").keypress(function (e) {
        if (e.keyCode == 13) {
            doSearch();
        }
    });

    //bam + de them moi
    if ($("#btnInsert").length > 0) {
        $(document).keypress(function (e) {
            if (e.keyCode == 43) {  // 43 : +
                onOpenPopup(0, null);
            }
        });
    }

    //Ctr + S luu form
    //$(document).bind('keydown', function (event) {
    //    if (eventHotKey) {
    //        if (event.ctrlKey || event.metaKey) {
    //            switch (String.fromCharCode(event.which).toLowerCase()) {
    //                case 's':

    //                    event.preventDefault();
    //                    if ($("#btnInsert").length > 0 ) {
    //                        $("#formPopupDe").submit();
    //                    }
    //                    break;
    //            }
    //        }
    //    }        
    //});
    //$("#StartDate").kendoDatePicker({format:"dd/MM/yyyy"});
    //$("#EndDate").kendoDatePicker({ format: "dd/MM/yyyy" });
});

//========================================== code co ban ====================================

//search
function doSearch() {
    var grid = $("#grid").data("kendoGrid");
    var filter = { logic: "and", filters: [] };
    var text = $("#txtPromotionID").val();
    if (text) {
        var filterOr = { logic: "or", filters: [] };
        filterOr.filters.push({ field: "PromotionID", operator: "contains", value: text });
        filterOr.filters.push({ field: "PromotionName", operator: "contains", value: text });
        filter.filters.push(filterOr);
    }
    text = $("#selectIsActive_search").val();
    if (text) {
        //var filterOr = { logic: "or", filters: [] };
        filter.filters.push({ field: "Status", operator: "eq", value: text });
    }

    grid.dataSource.filter(filter);
}
function resizeGrid(number) {
    var h_search = $(".divSearch").height();
    var h = parseInt($(window).height()) - h_search;
    var content = $("#grid").find(".k-grid-content");
    content.height(h - number);
}
//grid 
function onDatabound(e) {
    resizeGrid(numHeight);
    var grid = $("#grid").data("kendoGrid");

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
    changeStatusGridD('grid');
    $("#divLoading").hide();
}
function onDataboundD(e) {
    var grid = $("#grid2").data("kendoGrid");
    alert();
}

function onRequestStart(e) {
    $("#divLoading").show();
}
function loadTextById(listOrgRule) {
    var result = listOrgRule;
    var data = $('#listBrand option');
    for (var i = 0, length = data.length; i < length; i++) {
        result = result.replace(data[i].value, data[i].label);
    }
    return result;
}
function onRequestEnd(e) {
    if (e.type == "update" || e.type == "create" || e.type == "delete") {
        if (e.response.Errors == null) {
            alertBox("Thành công! ", "Lưu thành công", true, 3000);

        }
        else {
            alertBox("Báo lỗi!", e.response.Errors.er.errors[0], false, 3000);
        }
    }
    $("#divLoading").hide();
}
//popup
$("#dkck").click(function () {
    //$("#grid2").data("kendoGrid").dataSource.read();
});
function onOpenPopup(key, obj) {

    eventHotKey = true;
    $("#formPopupDe").find('section em').remove();
    $("#formPopupDe").find('section label').removeClass('state-error').removeClass('state-success');
    idPopup = ".k-window";
    $("#divMaskPopup").show();
    var popup = $('#popup').data("kendoWindow");
    popup.center().open();
    $(".k-window span.k-i-close").click(function () {
        eventHotKey = false;
        $("#divMaskPopup").hide();
    });
    //$("#selectMerchant").prop("readonly", true);
    //$("#selectTransporter").prop("readonly", true);
    //$("#selectTerritory").prop("readonly", true);
    if (key == 0) {     // Create
        keyAction = key;
        popup.title('Thêm');
        $("#CreatedBy").val('');
        $("#CreatedAt").val('');
        $("#StartDate").val('');
        $("#EndDate").val('');
        $("#PromotionName").val('');
        //$("#TransporterID").val('');
        //$("#TransporterID").trigger("change");
        $("#Note").val('');
        $("#Status option[value='True']").attr('selected', true);
        $("#selectTransporter option").removeAttr('selected');
        generateSelect2("selectTransporter");
        $("#selectTerritory option").removeAttr('selected');
        generateSelect2("selectTerritory");
        $("#selectMerchant option").removeAttr('selected');
        generateSelect2("selectMerchant");
        $("#DecaMinOrdAmt").val('');
        $("#DecaMaxOrdAmt").val('');
        $("#DecaPercent").val('');
        $("#MerchantMinOrdAmt").val('');
        $("#MerchantMaxOrdAmt").val('');
        $("#MerchantPercent").val('');
        $("#conNote").val('');
        $("#DecaMinOrdQty").val('');
        $("#DecaMaxOrdQty").val('');
        $("#DecaPercentQty").val('');
        $("#MerchantMinOrdQty").val('');
        $("#MerchantMaxOrdQty").val('');
        $("#MerchantPercentQty").val('');
        $("#conNoteQty").val('');
        setTimeout(function () {
            $("#ContractID").focus();
        }, 500);
        onRefreshPopup();
        setTimeout(function () {
            $("#PromotionID").focus();
        }, 500);
    }
    else {
        //$("#grid2").data("kendoGrid").dataSource.read();
        keyAction = -1;
        popup.title('Chỉnh sửa');
        var id = $(obj).data('id');
        $("#formPopupDe").find('fieldset:eq(0) section:eq(0)').empty().append('<div class="divlabel"><label class="label" style="float: right;">Mã CTKM: </label></div><div class="divfile"><label style="float: right;text-align: left;width: 240px;height: 19px;padding-top: 2px;"><strong style="color:red;">' + id + '</strong><input type="hidden" id="PromotionID" name="PromotionID" value="' + id + '"/></label> <div style="clear:both"></div>');
        var currentRow = $(obj).closest("tr");
        $("#selectMerchant option:selected").removeAttr('selected');
        $("#selectTransporter option:selected").removeAttr('selected');
        $("#selectTerritory option:selected").removeAttr('selected');
        var dataItem = $("#grid").data("kendoGrid").dataItem(currentRow);
        $("#divQty").hide();
        
        var listtran = dataItem.TransporterID.split(";");
        $("#selectTransporter option:selected").removeAttr('selected');
        for (var i = 0; i < listtran.length; i++) {
            $("#selectTransporter option[value='" + listtran[i].trim() + "']").attr('selected', true);
        }
        $("#selectTransporter").trigger("change");
        $("#selectMerchant option:selected").removeAttr('selected');
        if (dataItem.IsAllMerchant) {
            $("#selectMerchant option[value='" + "0" + "']").attr('selected', true);
            $("#selectMerchant").trigger("change");
        }
        else {
            var listtranmer = dataItem.MerchantID.split(";");
            $("#selectMerchant option:selected").removeAttr('selected');
            for (var i = 0; i < listtranmer.length; i++) {
                $("#selectMerchant option[value='" + listtranmer[i].trim() + "']").attr('selected', true);
            }
            $("#selectMerchant").trigger("change");
        }
        $("#selectTerritory option:selected").removeAttr('selected');
        if (dataItem.IsAllDistric) {
            $("#selectTerritory option[value='" + "0" + "']").attr('selected', true);
            $("#selectTerritory").trigger("change");
        }
        else {
            var listtranlo = dataItem.DistrictID.split(";");
            $("#selectTerritory option:selected").removeAttr('selected');
            for (var i = 0; i < listtranlo.length; i++) {
                $("#selectTerritory option[value='" + listtranlo[i].trim() + "']").attr('selected', true);
            }
            $("#selectTerritory").trigger("change");
        }
        var active = dataItem.Status;
        if (active) {
            $("#Status").val('True').trigger('change');

        } else {
            $("#Status").val('False').trigger('change');

        }
        //$("#Status").prop('readonly', true);
        var isAllmerchant1 = dataItem.IsAllMerchant;
        if (isAllmerchant1) {
            $("#IsAllMerchant").val('True').trigger('change');

        }
        else {
            $("#IsAllMerchant").val('False').trigger('change');

        }
        $("#IsAllMerchant").prop('readonly', true);
        var IsAllDistric1 = dataItem.IsAllDistric;
        if (IsAllDistric1) {
            $("#IsAllDistric").val('True').trigger('change');

        }
        else {
            $("#IsAllDistric").val('False').trigger('change');

        }
        $.post(r + "/Promotion/GetCondittionByCode", { id: dataItem.PromotionID, type: 2 }, function (data) {
            
            if (data.success) {
                var value = data.data;
                $("#DecaMinOrdQty").val(value.DecaMinOrdQty);
                $("#DecaMaxOrdQty").val(value.DecaMaxOrdQty);
                $("#DecaPercentQty").val(Number(value.DecaPercent)*100);
                $("#MerchantMinOrdQty").val(value.MerchantMinOrdQty);
                $("#MerchantMaxOrdQty").val(value.MerchantMaxOrdQty);
                $("#MerchantPercentQty").val(Number(value.MerchantPercent)*100);
                $("#conNoteQty").val(value.Note);
            }
        });
        $.post(r + "/Promotion/GetCondittionByCode", { id: dataItem.PromotionID, type: 1 }, function (data) {

            if (data.success) {
                var value = data.data;
                $("#DecaMinOrdAmt").val(value.DecaMinOrdAmt);
                $("#DecaMaxOrdAmt").val(value.DecaMaxOrdAmt);
                $("#DecaPercent").val(Number(value.DecaPercent)*100);
                $("#MerchantMinOrdAmt").val(value.MerchantMinOrdAmt);
                $("#MerchantMaxOrdAmt").val(value.MerchantMaxOrdAmt);
                $("#MerchantPercent").val(Number(value.MerchantPercent)*100);
                $("#conNoteAmt").val(value.Note);
            }
        });
        onBindDataToFormDiscount(dataItem);
        debugger;
        var d = kendo.toString(kendo.parseDate(dataItem.StartDate), 'dd/MM/yyyy');
        var e = kendo.toString(kendo.parseDate(dataItem.EndDate), 'dd/MM/yyyy');
        $("#StartDate").val(d + '-' + e);

    }
}
function onBindDataToFormDiscount(dataItem) {
    for (var propName in dataItem) {
        if (dataItem[propName] != null && dataItem[propName].constructor.toString().indexOf("Date") > -1) {
            var d = kendo.toString(kendo.parseDate(dataItem[propName]), 'dd/MM/yyyy')
            if (d != '01/01/1900') {
                $("#" + propName).val(d);
            }
        }
        else {
            $("#" + propName).val(dataItem[propName]);
            //$("#" + propName).attr('readonly', true);
        }
    }
}
function readHeaderInfo() {

    //contentTab = setContentTab(["PromotionID", "PromotionName", "Status"], "30");

}

function onRefreshPopup() {
    $("#formPopupDe").find('fieldset:eq(0) section:eq(0)').empty().append('<div class="divlabel"><label class="label" style="float: right;">Mã CTKM: </label></div><div class="divfile"><label class="input" style="float:right;width:240px;"><input type="text" id="PromotionID" name="PromotionID" class="input-xs" placeholder="Mã CTKM" style="margin-right:85px" /></label><div style="clear:both"></div>');
    $("#PromotionName").val('');
    $("#Status option[value='True']").attr('selected', true);
    $("#StartDate").val('');
    $("#EndDate").val('');
    $("#Note").val('');
    $("#CreatedAt").val('');
    $("#CreatedBy").val('');
    $('#btnSubmit').css({ 'display': 'block' });
}

function openImport() {
    idPopup = ".k-window";
    $("#divMaskPopup").show();
    $('#popupImport').data("kendoWindow").center().open();
    $(".k-window span.k-i-close").click(function () {
        $("#divMaskPopup").hide();
    });
}

function beginImport() {
    var value = $("input[name='FileUpload']").val();
    if (value != null && value != "") {
        $("#divLoading").show();
        $("#importform").submit();
    }
    else {
        alertBox("Báo lỗi", "Chọn file để nhập Excel", false, 3000);
    }
}
function loadToolbarStyle() {
    // Remove Icon
    $("div.k-grid-toolbar a span").remove();


    $("a.k-grid-cancel-changes").css({ 'background-color': '#D15b47', 'color': 'white' });
    $("a.k-grid-save-changes").hover(
      function () { $(this).css({ 'background-color': '#5b835b', 'color': 'white' }); },
       function () { $(this).css({ 'background-color': '#739e73', 'color': 'white' }); }
    )
    $("a.k-grid-save-changes").css({ 'background-color': '#428bca', 'color': 'white' });

    $("a.btn-custom1").hover(

     function () { $(this).css({ 'background-color': '#5b835b', 'color': 'white' }); },
      function () { $(this).css({ 'background-color': '#739e73', 'color': 'white' }); }
   )
    $("a.btn-custom1").css({ 'background-color': '#739e73', 'color': 'white' });

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
function changeStatusGridD(idGrid) {
    var arrRow = $("#" + idGrid).find(".k-grid-content table tbody tr");
    for (var i = 0; i < arrRow.length; i++) {
        var arrCol = arrRow[i].cells;
        if (arrCol.length == 0) {
            continue;
        }
        changeIsActiveD(arrCol);


    }

    $("#divLoading").hide();
}
function changeIsActiveD(arrCol) {
    for (var j = 0; j < arrCol.length; j++) {
        var tr;
        var columnName = '';
        tr = arrCol[j];
        var attr = $(tr).attr('data-column');
        // For some browsers, `attr` is undefined; for others,
        // `attr` is false.  Check for both.
        if (typeof attr === typeof undefined || attr === false) {
            continue;
        }
        if (attr == 'IsActive') {
            if (tr.textContent == "true") {
                $(tr).empty().append('<span class="label-success" style="font-size:12px">Đang hoạt động</span>');
            }
            else if (tr.textContent == "false") {
                $(tr).empty().append('<span class="label-success" style="font-size:12px">Ngưng hoạt động</span>');
            }
        }
        if (attr == 'IsAllMerchant') {
            if (tr.textContent == "true") {
                $(tr).empty().append('<span class="label-success" style="font-size:12px">Áp dụng cho tất cả gian hàng</span>');
            }
            else if (tr.textContent == "false") {
                $(tr).empty().append('<span class="label-success" style="font-size:12px">Không áp dụng cho tất cả gian hàng</span>');
            }
        }
        if (attr == 'IsAllDistric') {
            if (tr.textContent == "true") {
                $(tr).empty().append('<span class="label-success" style="font-size:12px">Áp dụng cho tất cả ĐBVC</span>');
            }
            else if (tr.textContent == "false") {
                $(tr).empty().append('<span class="label-success" style="font-size:12px">Không áp dụng cho tất cả ĐBVC</span>');
            }
        }
    }
}
function generateSelect2(id) {
    $("#" + id).select2();
    $("#s2id_" + id).css('width', '100%');
    $("#s2id_" + id).find('input').css('width', '100%');
}
function onSaveData() {
    var id = $("#PromotionID").val();
    if (typeof id == 'undefined' || id=="") {
        $("#loadingSaveData").removeClass('hide');
        alertBox("Báo lỗi", "Chưa thêm CTKM!!", false, 3000);
        return;
    }
    var tran = $("#selectTransporter").val();
    if (tran == null || tran.length == 0) {
        tran = "";
    }
    var ter = $("#selectTerritory").val();
    if (ter == null || ter.length == 0) {
        ter = "";
    }
    var mer = $("#selectMerchant").val();
    if (mer == null || mer.length == 0) {
        mer = "";
    }
    $("#loadingSaveData").removeClass('hide');
    $("#btnSaveData").attr('disabled', true);
    $.post(r + "/Promotion/AddDataPromotion", { id: id, tran: tran.toString(), ter: ter.toString(), mer: mer.toString() }, function (data) {
        if (data.success) {
            //selectedTabstrip(2);
            generateSelect2("selectTransporter");
            generateSelect2("selectTerritory");
            generateSelect2("selectMerchant");

            $("#grid").data("kendoGrid").dataSource.read();
            alertBox("Thành công! ", "Lưu thành công", true, 3000);
        }
        else {
            alertBox("Báo lỗi! ", data.message, false, 3000);
            console.log(data.message);
        }
        $("#loadingSaveData").addClass('hide');
        $("#btnSaveData").attr('disabled', false);
    });
}
function onSaveDataAmt() {
    var id = $("#PromotionID").val();
    if (typeof id == 'undefined' || id == "") {
        $("#loadingSaveData").removeClass('hide');
        alertBox("Báo lỗi", "Chưa thêm CTKM!!", false, 3000);
        return;
    }
    var DecaMinOrdAmt = $("#DecaMinOrdAmt").val();
    if (DecaMinOrdAmt == null || DecaMinOrdAmt.length == 0) {
        $("#DecaMinOrdAmt").focus();
        alertBox("Lỗi", "Giá trị chưa được nhập!", false, 3000);
        return;
    }
    if (!$.isNumeric(DecaMinOrdAmt)) {
        $("#DecaMinOrdAmt").focus();
        alertBox("Lỗi", "Giá trị nhập là kiểu số!", false, 3000);
        return;
    }
    var DecaMaxOrdAmt = $("#DecaMaxOrdAmt").val();
    if (DecaMaxOrdAmt == null || DecaMaxOrdAmt.length == 0) {
        $("#DecaMaxOrdAmt").focus();
        alertBox("Lỗi", "Giá trị chưa được nhập!", false, 3000);
        return;
    }
    if (!$.isNumeric(DecaMaxOrdAmt)) {
        $("#DecaMaxOrdAmt").focus();
        alertBox("Lỗi", "Giá trị nhập là kiểu số!", false, 3000);
        return;
    }
    var DecaPercentAmt = $("#DecaPercent").val();
    if (DecaPercentAmt == null || DecaPercent.length == 0) {
        $("#DecaPercentAmt").focus();
        alertBox("Lỗi", "Giá trị chưa được nhập!", false, 3000);
        return;
    }
    if (Number(DecaPercentAmt) < 0 || Number(DecaPercentAmt) > 100) {
        $("#DecaPercentAmt").focus();
        alertBox("", "Giá trị nhập từ 0-100!", false, 3000);
        return;
    }
    if (!$.isNumeric(DecaPercentAmt)) {
        $("#DecaPercentAmt").focus();
        alertBox("Lỗi", "Giá trị nhập là kiểu số!", false, 3000);
        return;
    }
    var MerchantMinOrdAmt = $("#MerchantMinOrdAmt").val();
    if (MerchantMinOrdAmt == null || MerchantMinOrdAmt.length == 0) {
        $("#MerchantMinOrdAmt").focus();
        alertBox("Lỗi", "Giá trị chưa được nhập!", false, 3000);
        return;
    }
    if (!$.isNumeric(MerchantMinOrdAmt)) {
        $("#MerchantMinOrdAmt").focus();
        alertBox("Lỗi", "Giá trị nhập là kiểu số!", false, 3000);
        return;
    }
    var MerchantMaxOrdAmt = $("#MerchantMaxOrdAmt").val();
    if (MerchantMaxOrdAmt == null || MerchantMaxOrdAmt.length == 0) {
        $("#MerchantMaxOrdAmt").focus();
        alertBox("Lỗi", "Giá trị chưa được nhập!", false, 3000);
        return;
    }
    if (!$.isNumeric(MerchantMaxOrdAmt)) {
        $("#MerchantMaxOrdAmt").focus();
        alertBox("Lỗi", "Giá trị nhập là kiểu số!", false, 3000);
        return;
    }
    var MerchantPercentAmt = $("#MerchantPercent").val();
    if (MerchantPercentAmt == null || MerchantPercent.length == 0) {
        $("#MerchantPercentAmt").focus();
        alertBox("Lỗi", "Giá trị chưa được nhập!", false, 3000);
        return;
    }
    if (!$.isNumeric(MerchantPercentAmt)) {
        $("#MerchantPercentAmt").focus();
        alertBox("Lỗi", "Giá trị nhập là kiểu số!", false, 3000);
        return;
    }
    if (Number(MerchantPercentAmt) < 0 || Number(MerchantPercentAmt) > 100) {
        $("#MerchantPercentAmt").focus();
        alertBox("", "Giá trị nhập từ 0-100!", false, 3000);
        return;
    }
    var Note = $("#conNote").val();
    $("#loadingSaveData").removeClass('hide');
    $("#btnSaveDataAmt").attr('disabled', true);
    $.post(r + "/Promotion/AddDataPromotionAmt", {
        id: id, DecaMinOrdAmt: DecaMinOrdAmt.toString(), DecaMaxOrdAmt: DecaMaxOrdAmt.toString(), DecaPercentAmt: DecaPercentAmt.toString(),
        MerchantMinOrdAmt: MerchantMinOrdAmt.toString(), MerchantMaxOrdAmt: MerchantMaxOrdAmt.toString(), MerchantPercentAmt: MerchantPercentAmt.toString(),Note:Note
    }, function (data) {
        if (data.success) {
            //selectedTabstrip(2);
            generateSelect2("selectTransporter");
            generateSelect2("selectTerritory");
            generateSelect2("selectMerchant");

            $("#grid").data("kendoGrid").dataSource.read();
            alertBox("Thành công! ", "Lưu thành công", true, 3000);
        }
        else {
            alertBox("Báo lỗi! ", data.message, false, 3000);
            console.log(data.message);
        }
        $("#loadingSaveData").addClass('hide');
        $("#btnSaveDataAmt").attr('disabled', false);
    });
}
function onSaveDataQty() {
    var id = $("#PromotionID").val();
    if (typeof id == 'undefined' || id == "") {
        $("#loadingSaveData").removeClass('hide');
        alertBox("Báo lỗi", "Chưa thêm CTKM!!", false, 3000);
        return;
    }
    var DecaMinOrdQty = $("#DecaMinOrdQty").val();
    if (DecaMinOrdQty == null || DecaMinOrdQty.length == 0) {
        $("#DecaMinOrdQty").focus();
        alertBox("", "Giá trị chưa được nhập!", false, 3000);
        return;
    }
    if (!$.isNumeric(DecaMinOrdQty))
    {
        $("#DecaMinOrdQty").focus();
        alertBox("", "Giá trị nhập là kiểu số!", false, 3000);
        return;
    }
    var DecaMaxOrdQty = $("#DecaMaxOrdQty").val();
    if (DecaMaxOrdQty == null || DecaMaxOrdQty.length == 0) {
        $("#DecaMaxOrdQty").focus();
        alertBox("", "Giá trị chưa được nhập!", false, 3000);
        return;
    }
    if (!$.isNumeric(DecaMaxOrdQty)) {
        $("#DecaMaxOrdQty").focus();
        alertBox("", "Giá trị nhập là kiểu số!", false, 3000);
        return;
    }
    var DecaPercentQty = $("#DecaPercentQty").val();
    if (DecaPercentQty == null || DecaPercent.length == 0) {
        $("#DecaPercentQty").focus();
        alertBox("", "Giá trị chưa được nhập!", false, 3000);
        return;
    }
    if (Number(DecaPercentQty) < 0 || Number(DecaPercentQty) > 100) {
        $("#DecaPercentQty").focus();
        alertBox("", "Giá trị nhập từ 0-100!", false, 3000);
        return;
    }
    if (!$.isNumeric(DecaPercentQty)) {
        $("#DecaPercentQty").focus();
        alertBox("", "Giá trị nhập là kiểu số!", false, 3000);
        return;
    }
    var MerchantMinOrdQty = $("#MerchantMinOrdQty").val();
    if (MerchantMinOrdQty == null || MerchantMinOrdQty.length == 0) {
        $("#MerchantMinOrdQty").focus();
        alertBox("", "Giá trị chưa được nhập!", false, 3000);
        return;
    }
    if (!$.isNumeric(MerchantMinOrdQty)) {
        $("#MerchantMinOrdQty").focus();
        alertBox("", "Giá trị nhập là kiểu số!", false, 3000);
        return;
    }
    var MerchantMaxOrdQty = $("#MerchantMaxOrdQty").val();
    if (MerchantMaxOrdQty == null || MerchantMaxOrdQty.length == 0) {
        $("#MerchantMaxOrdQty").focus();
        alertBox("", "Giá trị chưa được nhập!", false, 3000);
        return;
    }
    if (!$.isNumeric(MerchantMaxOrdQty)) {
        $("#MerchantMaxOrdQty").focus();
        alertBox("", "Giá trị nhập là kiểu số!", false, 3000);
        return;
    }
    var MerchantPercentQty = $("#MerchantPercentQty").val();
    if (MerchantPercentQty == null || MerchantPercent.length == 0) {
        $("#MerchantPercentQty").focus();
        alertBox("", "Giá trị chưa được nhập!", false, 3000);
        return;
    }
    if (!$.isNumeric(MerchantPercentQty)) {
        $("#MerchantPercentQty").focus();
        alertBox("Lỗi", "Giá trị nhập là kiểu số!", false, 3000);
        return;
    }
    if (Number(MerchantPercentQty) < 0 || Number(MerchantPercentQty) > 100) {
        $("#MerchantPercentQty").focus();
        alertBox("", "Giá trị nhập từ 0-100!", false, 3000);
        return;
    }
    $("#loadingSaveData").removeClass('hide');
    $("#btnSaveDataQty").attr('disabled', true);
    var Note = $("#conNoteQty").val();
    $.post(r + "/Promotion/AddDataPromotionQty", {
        id: id, DecaMinOrdQty: DecaMinOrdQty, DecaMaxOrdQty: DecaMaxOrdQty, DecaPercentQty: DecaPercentQty,
        MerchantMinOrdQty: MerchantMinOrdQty, MerchantMaxOrdQty: MerchantMaxOrdQty, MerchantPercentQty: MerchantPercentQty,Note:Note
    }, function (data) {
        if (data.success) {
            //selectedTabstrip(2);
            $("#grid").data("kendoGrid").dataSource.read();
            alertBox("Thành công! ", "Lưu thành công", true, 3000);
        }
        else {
            alertBox("Báo lỗi! ", data.message, false, 3000);
            console.log(data.message);
        }
        $("#loadingSaveData").addClass('hide');
        $("#btnSaveDataQty").attr('disabled', false);
    });
}
//function ChangeDis() {
//    debugger;
//    if ($("#selectTerritoryProvince").val() != "" && $("#selectTerritoryProvince").val() != null) {
//        $("#selectTerritory").val('').trigger('change');
//        $.post(r + "/Promotion/GetDistrictByProvince", { selectTerritoryProvince: $("#selectTerritoryProvince").val().toString() }, function (data) {
//            if (!data.success) {
//                $("#selectTerritory").removeAttr("disabled");
                
//                $("#selectTerritory").html('');
//                //var html = "<option value =''>Chọn Quận/Huyện </option>";
//                var html = "";
//                for (var i = 0; i < data.length ; i++) {
//                    html += "<option value ='" + data[i].TerritoryID + "'>" + data[i].TerritoryName + "</option>";
//                }
//                $("#selectTerritory").html(html);
//            }
//        });
//    }
//    else {
//        $("#selectTerritory").val('').trigger('change');
//        $("#selectTerritory").prop("disabled", true);
//    }
//}
function ChangeProType() {
    if ($("#selectTypePromotion").val() != "" && $("#selectTypePromotion").val() != null) {
        var value = $("#selectTypePromotion").val();
        if(value=="1")
        {
            $("#divAmt").show();
            $("#divQty").hide();
        }
        else
        {
            $("#divAmt").hide();
            $("#divQty").show();
        }
        //$("#selectTerritory").val('').trigger('change');
        //$.post(r + "/Promotion/GetDistrictByProvince", { selectTerritoryProvince: $("#selectTerritoryProvince").val().toString() }, function (data) {
        //    if (!data.success) {
        //        $("#selectTerritory").removeAttr("disabled");

        //        $("#selectTerritory").html('');
        //        //var html = "<option value =''>Chọn Quận/Huyện </option>";
        //        var html = "";
        //        for (var i = 0; i < data.length ; i++) {
        //            html += "<option value ='" + data[i].TerritoryID + "'>" + data[i].TerritoryName + "</option>";
        //        }
        //        $("#selectTerritory").html(html);
        //    }
        //});
    }
    else {
        //$("#selectTerritory").val('').trigger('change');
        //$("#selectTerritory").prop("disabled", true);
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
function dateToString(date) {

    if (date != null) {
        date = new Date(date.match(/\d+/)[0] * 1);
        var day = date.getDate() < 10 ? "0" + date.getDate() : date.getDate();
        var month = (date.getMonth() + 1) < 10 ? "0" + (date.getMonth() + 1) : (date.getMonth() + 1);
        return date.getFullYear() + '/' + month + '/' + day + ' ' + date.getHours() + ':' + date.getMinutes() + ':' + date.getSeconds();
    }
    else {
        return "";
    }
}
