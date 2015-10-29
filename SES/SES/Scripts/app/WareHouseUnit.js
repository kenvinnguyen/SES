$(document).ready(function () {

    $("#selectStatus_searchUnit").select2();
    $("#s2id_selectStatus_searchUnit").css('width', '100%');

    $("#Status").select2();
    $("#s2id_Status").css('width', '240px');
    $(window).resize(function () {
        resizeGrid(numHeight);
    });

    //goi ham search khi nhan enter
    $("#txtNameUnit").keypress(function (e) {
        if (e.keyCode == 13) {
            doSearchUnit();
        }
    });
    //bam + de them moi
    if ($("#btnInsertUnit").length > 0) {
        $(document).keypress(function (e) {
            if (e.keyCode == 43) {  // 43 : +
                onOpenPopup(0, null);
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
                        var boolCreatedBy = $("#CreatedByUnit").val() != "system";
                        if (indexTabstripActive == 0 && $("#btnInsertUnit").length > 0 && boolCreatedBy) {
                            $("#formPopupUnit").submit();
                        }
                        else if (indexTabstripActive == 1 && $("#btnInsertUnit").length > 0) {
                            onSaveRole();
                        }
                        else if (indexTabstripActive == 2 && $("#btnInsertUnit").length > 0) {
                        }
                        break;
                }
            }
        }
    });

    //popup
    $('#popupImportUnit').kendoWindow({
        width: "600px",
        actions: ["Close"],
        title: "Nhập Excel",
        visible: false,
        resizable: false
    });

    $("#importformUnit").ajaxForm({
        beforeSend: function () {
            $("#popupImportUnit").data("kendoWindow").close();
        },
        uploadProgress: function (event, position, total, percentComplete) { },
        success: function (data) {
            if (data.success) {
                $("#gridUnit").data("kendoGrid").dataSource.read();
                hideMaskPopup('#divMaskPopup');
                if (data.totalError > 0) {
                    console.log(data.errorfile);
                    $.SmartMessageBox({
                        title: "Lỗi",
                        content: "Có dòng lỗi, tải về để sửa lại.",
                        buttons: '[Hủy][Tải]'
                    }, function (ButtonPressed) {
                        if (ButtonPressed === "Tải") {
                            window.open(r + "/ExcelImport/" + data.link, "_blank");
                        }
                        if (ButtonPressed === "Hủy") {
                            return;
                        }
                    });
                }
                else {
                    alertBox("Thành công! ", "Cập nhập thành công " + data.total + " mẫu tin", true, 3000);
                }
            }
            else {
                alertBox("Báo lỗi! ", data.message, false, 3000);
                console.log(data.message);
            }
            $("#divLoading").hide();
            hideMaskPopup('#divMaskPopup');
        },
        complete: function (xhr) { }
    });

    //form trong popup
    $('#popupUnit').kendoWindow({
        width: "500px",
        actions: ["Close"],
        visible: false,
        resizable: false,
        //open: function (e) {
        //    this.wrapper.css({ top: "115px" });
        //    //this.wrapper.css({ top: $('#header').height() });
        //}
    });
    $("#formPopupUnit").validate({
        // Rules for form validation
        rules: {
            UnitName: {
                required: true,
                //alphanumeric: true
            },
        },

        // Messages for form validation
        messages: {
            UnitName: {
                required: "Thông tin bắt buộc"
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
                            $("#formPopupUnit").find('fieldset:eq(0) section:eq(0)').show();
                            $("#formPopupUnit").find('fieldset:eq(0) section:eq(0)').empty().append('<div style="float: left; width: 120px; margin-left:0;"><label class="label" style="float: right;">Mã đơn vị (*)</label></div><div style="float: left; width: 240px; margin-right: 0; margin-left: 10px;"> <label style="float: right;text-align: left;width: 240px;height: 19px;padding-top: 2px;"><strong style="color:red;">' + data.Code + '</strong><input type="hidden" id="UnitID" name="UnitID" value="' + data.Code + '"/></label> <div style="clear:both"></div></di>');
                            $("#CreatedAtUnit").val(dateToString(data.createdate));
                            $("#CreatedByUnit").val(data.createdby);
                            keyAction = -1;
                        }
                        readHeaderInfoUnit();
                        $("#gridUnit").data("kendoGrid").dataSource.read();
                        alertBox("Thành công!", "Lưu thành công", true, 3000);
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
function onOpenPopupUnit(key, obj) {
    eventHotKey = true;
    $("#formPopupUnit").find('section em').remove();
    $("#formPopupUnit").find('section label').removeClass('state-error').removeClass('state-success');
    idPopup = ".k-window";
    $("#divMaskPopup").show();
    var popup = $('#popupUnit').data("kendoWindow");
    popup.center().open();
    $(".k-window span.k-i-close").click(function () {
        eventHotKey = false;
        $("#divMaskPopup").hide();
    });
    //Tạo mới
    if (key == 0) {
        keyAction = key;
        popup.title('Thêm');
        //onRefreshPopupUnit();
        $("#formPopupUnit")[0].reset();
        $("#formPopupUnit").find('fieldset:eq(0) section:eq(0)').hide();
        $("#formPopupUnit").find('fieldset:eq(0) section:eq(0)').empty();
        $("#CreatedByUnit").val('');
        $("#CreatedAtUnit").val('');
        setTimeout(function () {
            $("#UnitName").focus();
        }, 500);
    }
        // Cập nhập
    else {
        debugger;
        keyAction = -1;
        popup.title('Chỉnh sửa');
        var id = $(obj).data('id');
        $("#formPopupUnit").find('fieldset:eq(0) section:eq(0)').show();
        $("#formPopupUnit").find('fieldset:eq(0) section:eq(0)').empty().append('<div style="float: left; width: 120px; margin-left:0;"><label class="label" style="float: right;">Mã đơn vị (*)</label></div><div style="float: left; width: 240px; margin-right: 0; margin-left: 10px;"> <label style="float: right;text-align: left;width: 240px;height: 19px;padding-top: 2px;"><strong style="color:red;">' + id + '</strong><input type="hidden" id="UnitID" name="UnitID" value="' + id + '"/></label> <div style="clear:both"></div></di>');
        //$("#formPopupUnit").find('fieldset:eq(0) section:eq(0)').empty().append('<label class="label" style="float: left;">Mã đơn vị</label><label style="float: right;text-align: left;width: 240px;height: 19px;padding-top: 2px;"><strong style="color:red;">' + id + '</strong><input type="hidden" id="UnitID" name="UnitID" value="' + id + '"/></label> <div style="clear:both"></div>');
        var currentRow = $(obj).closest("tr");
        var dataItem = $("#gridUnit").data("kendoGrid").dataItem(currentRow);
        $("#CreatedByUnit").val(dataItem.CreatedBy);
        $("#CreatedAtUnit").val(dataItem.CreatedAt);
        $("#NoteUnit").val(dataItem.Note)
        onBindDataToFormWHL(dataItem);
        setTimeout(function () {
            $("#UnitName").focus();
        }, 500);
        readHeaderInfoUnit();
    }
}
function readHeaderInfoUnit() {
    contentTab = setContentTab(["UnitID", "UnitName", 'Note', 'Status'], "30");
}
//function onRefreshPopupUnit() {
//    $("#formPopupUnit").find('fieldset:eq(0) section:eq(0)').empty().append('<label class="label" style="float:left">Đơn vị tính</label><label class="input" style="float:right;width:240px;"><input type="text" id="UnitName" name="UnitName" class="input-xs" placeholder="Đơn vị tính" style="margin-right:85px" /><b class="tooltip tooltip-top-right">Vị trí kho</b></label><div style="clear:both"></div>');
//    $("#UnitName").val('');
//    $("#Note").val('');
//    $("#CreatedAtUnit").val('');
//    $("#CreatedByUnit").val('');
//    $('#btnSubmit').css({ 'display': 'block' });
//}
function onDataboundUnit(e) {
    resizeOtherGrid(numHeight, $('#gridUnit'));
    var grid = $("#gridUnit").data("kendoGrid");
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
function doSearchUnit() {
    var grid = $("#gridUnit").data("kendoGrid");
    var filter = { logic: "and", filters: [] };
    var Name = $("#txtWHName").val();
    if (Name) {
        var filterOr = { logic: "or", filters: [] };
        filterOr.filters.push({ field: "UnitName", operator: "contains", value: Name });
        filter.filters.push(filterOr);
    }
    var text = $("#Group_search").val();
    if (text) {
        var filterOr = { logic: "or", filters: [] };
        filterOr.filters.push({ field: "Roles", operator: "contains", value: text });
        filter.filters.push(filterOr);
    }
    var text = $("#Roles_search").val();
    if (text) {
        var filterOr = { logic: "or", filters: [] };
        filterOr.filters.push({ field: "AppliedFor", operator: "contains", value: text });
        filter.filters.push(filterOr);
    }

    text = $("#selectStatus_searchUnit").val();
    if (text) {
        filter.filters.push({ field: "Status", operator: "eq", value: text });
    }

    grid.dataSource.filter(filter);
}
function openImportUnit() {
    idPopup = ".k-window";
    $("#divMaskPopup").show();
    $('#popupImportUnit').data("kendoWindow").center().open();
    $(".k-window span.k-i-close").click(function () {
        $("#divMaskPopup").hide();
    });
}

function beginImportUnit() {
    debugger;
    var value = $("#importformUnit input[name='FileUpload']").val();
    if (value != null && value != "") {
        $("#divLoading").show();
        $("#importformUnit").submit();
    }
    else {
        alertBox("Có lỗi!", " Chọn file để nhập Excel", false, 3000);
    }
}