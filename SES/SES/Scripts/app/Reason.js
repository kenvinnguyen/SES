﻿var numHeight = 320;
var keyAction;
var indexTabstripActive = -1;
var contentTab;
$(document).ready(function () {

    //active menu
    resetMenu();
    activeMenu(1, 2, 1, '2_1', 'menu_Reason');

    document.title = "Lý do";

    //fillter & form popup
    $("#selectIsActive_search").select2();
    $("#s2id_selectIsActive_search").css('width', '100%');

    $("#IsActive").select2();
    $("#s2id_IsActive").css('width', '240px');
    $(window).resize(function () {
        resizeGrid(numHeight);
    });

    //popup
    $('#popupImport').kendoWindow({
        width: "600px",
        actions: ["Close"],
        title: "Nhập Excel",
        visible: false,
        resizable: false
    });

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
                    alertBox("Thành công", "", true, 3000);
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
        width: "500px",
        actions: ["Close"],
        visible: false,
        resizable: false,
        //open: function (e) {
        //    this.wrapper.css({ top: "115px" });
        //    //this.wrapper.css({ top: $('#header').height() });
        //}
    });    

    $("#formPopupRe").validate({
        // Rules for form validation
        rules: {
            ReasonID: {
                required: true,
                alphanumeric: true
            },
           
        },

        // Messages for form validation
        messages: {
            ReasonID: {
                required: "Thông tin bắt buộc"                
            },
           
        },

        // Do not change code below
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
                            $("#formPopupRe").find('fieldset:eq(0) section:eq(0)').empty().append('<div class="divlabel"><label class="label" style="float: right;">Mã lý do: </label></div><div class="divfile"><label style="float: right;text-align: left;width: 240px;height: 19px;padding-top: 2px;"><strong style="color:red;">' + data.ReasonID + '</strong><input type="hidden" id="ReasonID" name="ReasonID" value="' + data.ReasonID + '" /></label> <div style="clear:both"></div>');
                            $("#RowCreatedAt").val(dateToString(data.RowCreatedAt));
                            $("#RowCreatedBy").val(data.RowCreatedBy);
                            keyAction = -1;
                        }
                        readHeaderInfo();
                        //$("#popup").data("kendoWindow").close();
                        //$("#divMaskPopup").hide();
                        $("#grid").data("kendoGrid").dataSource.read();
                        //$("#divMaskPopup").hide();
                        //$("#loading").addClass('hide');
                        alertBox("Thành công! ", "Lưu thành công",true, 3000);
                    }
                    else {
                        
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
    $("#txtReasonID").keypress(function (e) {
        if (e.keyCode == 13) {
            doSearch();
        }
    });

    //bam + de them moi
    if ($("#btnInsert").length > 0) {
        $(document).keypress(function (e) {
            if (e.keyCode == 43) {  // 43 : +
                onOpenPopupDe(0, null);
            }
        });
    }    

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
function doSearch() {
    var grid = $("#grid").data("kendoGrid");
    var filter = { logic: "and", filters: [] };    
    var text = $("#txtReasonID").val();
    if (text) {
        var filterOr = { logic: "or", filters: [] };
        filterOr.filters.push({ field: "ReasonID", operator: "contains", value: text });
        filterOr.filters.push({ field: "ReasonType", operator: "contains", value: text });
        filter.filters.push(filterOr);
    }
    text = $("#selectIsActive_search").val();
    if (text) {
        filter.filters.push({ field: "IsActive", operator: "eq", value: text });
    }
    grid.dataSource.filter(filter);
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
    changeStatusGrid('grid',6);
    $("#divLoading").hide();
}

function onRequestStart(e) {
    $("#divLoading").show();
}

function onRequestEnd(e) {
    if (e.type == "update" || e.type == "create" || e.type == "delete") {
        if (e.response.Errors == null) {
            alertBox("Thành công", "Lưu thành công", true, 3000);

        }
        else {
            alertBox("Báo lỗi", e.response.Errors.er.errors[0], false, 3000);
        }
    }    
    $("#divLoading").hide();
}
//popup
function onOpenPopupDe(key, obj) {
    eventHotKey = true;
    $("#formPopupRe").find('section em').remove();
    $("#formPopupRe").find('section label').removeClass('state-error').removeClass('state-success');
    idPopup = ".k-window";
    $("#divMaskPopup").show();
    var popup = $('#popup').data("kendoWindow");
    popup.center().open();
    $(".k-window span.k-i-close").click(function () {
        eventHotKey = false;
        $("#divMaskPopup").hide();
    });
    if (key == 0) {     // Create
        keyAction = key;
        popup.title('Thêm');
        $("#ReasonType").val('None')
        $("#ReasonType").css('width', '240px');
        $("#ReasonType").select2();
        onRefreshPopup();
        setTimeout(function () {
            $("#ReasonID").focus();
        }, 500);
    }
    else {      // Update
        keyAction = -1;
        popup.title('Chỉnh sửa');
        var id = $(obj).data('id');
        //$("#formPopupRe").find('fieldset:eq(0) section:eq(0)').empty().append('<label class="label">Mã người dùng: <strong style="color:red;margin-left: 20px;">' + id + '</strong></label><input type="hidden" id="ReasonID" name="ReasonID" value="' + id + '" />');
        $("#formPopupRe").find('fieldset:eq(0) section:eq(0)').empty().append('<div class="divlabel"><label class="label" style="float:right">Mã lý do (*)</label></div><div class="divfile"><label style="float: right;text-align: left;width: 240px;height: 19px;padding-top: 2px;"><strong style="color:red;">' + id + '</strong><input type="hidden" id="ReasonID" name="ReasonID" value="' + id + '"/></label> <div style="clear:both"></div>');

        var currentRow = $(obj).closest("tr");
        var dataItem = $("#grid").data("kendoGrid").dataItem(currentRow);
        onBindDataToForm(dataItem);
        setTimeout(function () {
            $("#ReasonType").focus();
        }, 500);        
        $.post(r + "/Reason/GetReasonyCode", { ReasonID: id }, function (data) {
            showLoading();
            
            if (data.success) {
                var value = data.data;
                var active = value.IsActive == true ? "True" : "False";
                $("#IsActive option[value='" + active + "']").attr('selected', true);
                $("#IsActive").select2();
                $("#s2id_IsActive").css('width', '240px');
                $("#ReasonType").css('width', '240px');
                $("#ReasonType").select2();
                $("#ReasonType").val(value.ReasonType)
                $("#RowCreatedAt").val(dateToString(value.RowCreatedAt));
                $("#RowCreatedBy").val(value.RowCreatedBy);
                $('#btnSubmit').css({ 'display': 'block' })
                readHeaderInfo();
            }
            else {
                alertBox("Báo lỗi", "", false, 3000);
                console.log(data.message);
            }
            hideLoading();
        });
    }
}

function readHeaderInfo() {

    contentTab = setContentTab(["ReasonID", "ReasonType", "IsActive"], "30");

}

function onRefreshPopup() {
    $("#formPopupRe").find('fieldset:eq(0) section:eq(0)').empty().append('<div class="divlabel"><label class="label" style="float:right">Mã lý do (*)</label></div><div class="divfile"><label class="input" style="float:right;width:240px;"><input type="text" id="ReasonID" name="ReasonID" class="input-xs" placeholder="Mã lý do" style="margin-right:85px" /></label><div style="clear:both"></div>');
    $("#ReasonType").val('');
    $("#IsActive option[value='True']").attr('selected', true);
    $("#Description").val('');
    $("#RowCreatedAt").val('');
    $("#RowCreatedBy").val('');
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

//========================================== code khac neu co ====================================

