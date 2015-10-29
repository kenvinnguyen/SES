
$(document).ready(function () {
    //fillter & form popup
    $("#selectStatus_search").select2();
    $("#s2id_selectStatus_search").css('width', '100%');

    $("#Status").select2();
    $("#s2id_Status").css('width', '240px');
    $(window).resize(function () {
        resizeOtherGrid(numHeight, $('#gridWHL'));
    });
    $("#WHID").select2();
    $("#s2id_WHID").css('width', '240px');
    $("#WHLID").select2();
    $('#LocationID').select2();
    $("#s2id_LocationID").css('width', '240px');
    //goi ham search khi nhan enter
    $("#txtWHLname").keypress(function (e) {
        if (e.keyCode == 13) {
            doSearch();
        }
    });
    //bam + de them moi
    if ($("#btnInsertWHL").length > 0) {
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
                        var boolCreatedBy = $("#CreatedByWHL").val() != "system";
                        if (indexTabstripActive == 0 && $("#btnInsertWHL").length > 0 && boolCreatedBy) {
                            $("#formPopupWHL").submit();
                        }
                        else if (indexTabstripActive == 1 && $("#btnInsertWHL").length > 0) {
                            onSaveRole();
                        }
                        else if (indexTabstripActive == 2 && $("#btnInsertWHL").length > 0) {
                        }
                        break;
                }
            }
        }
    });

    //popup
    $('#popupImportWHL').kendoWindow({
        width: "600px",
        actions: ["Close"],
        title: "Nhập Excel",
        visible: false,
        resizable: false
    });

    $("#importformWHL").ajaxForm({
        beforeSend: function () {
            $("#popupImportWHL").data("kendoWindow").close();
        },
        uploadProgress: function (event, position, total, percentComplete) { },
        success: function (data) {
            if (data.success) {
                $("#gridWHL").data("kendoGrid").dataSource.read();
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
    $('#popupWHL').kendoWindow({
        width: "500px",
        actions: ["Close"],
        visible: false,
        resizable: false,
        //open: function (e) {
        //    this.wrapper.css({ top: "115px" });
        //    //this.wrapper.css({ top: $('#header').height() });
        //}
    });
    $("#formPopupWHL").validate({
        // Rules for form validation
        rules: {
            WHLName: {
                required: true,
                //alphanumeric: true
            },
        },

        // Messages for form validation
        messages: {
            WHLName: {
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
                    debugger
                    if (data.success) {
                        if (keyAction == 0) {   // Create
                            $("#formPopupWHL").find('fieldset:eq(0) section:eq(0)').show();
                            $("#formPopupWHL").find('fieldset:eq(0) section:eq(0)').empty().append('<div style="float: left; width: 120px; margin-left:0;"><label class="label" style="float: right;">Mã vị trí kho (*)</label></div><div style="float: left; width: 240px; margin-right: 0; margin-left: 10px;"> <label style="float: right;text-align: left;width: 240px;height: 19px;padding-top: 2px;"><strong style="color:red;">' + data.Code + '</strong><input type="hidden" id="WHLID" name="WHLID" value="' + data.Code + '"/></label> <div style="clear:both"></div></di>');
                            $("#CreatedAtWHL").val(dateToString(data.createdat));
                            $("#CreatedByWHL").val(data.createdby);
                            keyAction = -1;
                        }
                        
                        $("#gridWHL").data("kendoGrid").dataSource.read();
                        alertBox("Thành công!", " Lưu thành công", true, 3000);
                        $("#loading").addClass('hide');
                        resizeOtherGrid(290, $('#gridWH'));
                        resizeOtherGrid(290, $('#gridWHL'));
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
function onOpenPopupWHL(key, obj) {
    eventHotKey = true;
    $("#formPopupWHL").find('section em').remove();
    $("#formPopupWHL").find('section label').removeClass('state-error').removeClass('state-success');
    idPopup = ".k-window";
    $("#divMaskPopup").show();
    var popup = $('#popupWHL').data("kendoWindow");
    popup.center().open();
    $(".k-window span.k-i-close").click(function () {
        eventHotKey = false;
        $("#divMaskPopup").hide();
    });
    //Tạo mới
    if (key == 0) {
        keyAction = key;
        popup.title('Thêm');
        $("#formPopupWHL")[0].reset();
        $("#formPopupWHL").find('fieldset:eq(0) section:eq(0)').hide();
        $("#formPopupWHL").find('fieldset:eq(0) section:eq(0)').empty();
        $("#CreatedByWHL").val('');
        $("#CreatedAtWHL").val('');
        setTimeout(function () {
            $("#WHLName").focus();
        }, 500);
    }
     // Cập nhập
    else {
        debugger;
        keyAction = -1;
        popup.title('Chỉnh sửa');
        var id = $(obj).data('id');
        $("#formPopupWHL").find('fieldset:eq(0) section:eq(0)').show();
        $("#formPopupWHL").find('fieldset:eq(0) section:eq(0)').empty().append('<div style="float: left; width: 120px; margin-left:0;"><label class="label" style="float: right;">Mã vị trí kho (*)</label></div><div style="float: left; width: 240px; margin-right: 0; margin-left: 10px;"> <label style="float: right;text-align: left;width: 240px;height: 19px;padding-top: 2px;"><strong style="color:red;">' + id + '</strong><input type="hidden" id="WHLID" name="WHLID" value="' + id + '"/></label> <div style="clear:both"></div></di>');
        //$("#formPopupWHL").find('fieldset:eq(0) section:eq(0)').empty().append('<label class="label" style="float: left;">Mã vị trí kho</label><label style="float: right;text-align: left;width: 240px;height: 19px;padding-top: 2px;"><strong style="color:red;">' + id + '</strong><input type="hidden" id="WHLID" name="WHLID" value="' + id + '"/></label> <div style="clear:both"></div>');
        var currentRow = $(obj).closest("tr");
        var dataItem = $("#gridWHL").data("kendoGrid").dataItem(currentRow);
        //alert(id);
        $("#CreatedByWHL").val(dataItem.CreatedBy);
        $("#CreatedAtWHL").val(dataItem.CreatedAt);
        $("#NoteWHL").val(dataItem.Note)
        onBindDataToFormWHL(dataItem);
        setTimeout(function () {
            $("#WHLName").focus();
        }, 500);
    }
}
function readHeaderInfoWHL() {
    contentTab = setContentTab(["WHID", "WHLID", "WHLName", 'Note', 'Status'], "30");
}
function onBindDataToFormWHL(dataItem) {
    debugger;
    for (var propName in dataItem) {
        if (dataItem[propName] != null && dataItem[propName].constructor.toString().indexOf("Date") > -1) {
            var d = kendo.toString(kendo.parseDate(dataItem[propName]), 'dd/MM/yyyy')
            if (d != '01/01/1900') {
                $("#" + propName).val(d);
            }
        }
        else {
            $("#" + propName).val(dataItem[propName]);
        }
    }
}

function resizeGridWHL(number) {
    var h_search = $(".divSearch").height();
    var h = parseInt($(window).height()) - h_search;
    var content = $("#gridWHL").find(".k-grid-content");
    content.height(h - number);
}
function onDataboundWHL(e) {    
    resizeOtherGrid(numHeight, $('#gridWHL'));
    var grid = $("#gridWHL").data("kendoGrid");
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
function openImportWHL() {
    idPopup = ".k-window";
    $("#divMaskPopup").show();
    $('#popupImportWHL').data("kendoWindow").center().open();
    $(".k-window span.k-i-close").click(function () {
        $("#divMaskPopup").hide();
    });
}

function beginImportWHL() {
    var value = $("#importformWHL input[name='FileUpload']").val();
    if (value != null && value != "") {
        $("#divLoading").show();
        $("#importformWHL").submit();
    }
    else {
        alertBox("Báo lỗi!", " Chọn file để nhập Excel", false, 3000);
    }
}
function doSearchWHL() {
    var grid = $("#gridWHL").data("kendoGrid");
    var filter = { logic: "and", filters: [] };
    var Name = $("#txtWHLname").val();
    if (Name) {
        var filterOr = { logic: "or", filters: [] };
        filterOr.filters.push({ field: "WHName", operator: "contains", value: Name });
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

    text = $("#selectStatus_searchWHL").val();
    if (text) {
        filter.filters.push({ field: "Status", operator: "eq", value: text });
    }

    grid.dataSource.filter(filter);
}