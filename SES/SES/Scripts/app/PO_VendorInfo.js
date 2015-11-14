var numHeight = 320;
var keyAction;
var indexTabstripActive = -1;
var contentTab;

$(document).ready(function () {
    //active menu

    resetMenu();

    $("ul#menuLeft").find('#ul_root_4').addClass('open');
    $("ul#menuLeft").find('#ul_root_4').css('display', 'block');
    $("ul#menuLeft").find('#ul_root_4 ul#ul_item_5').css('display', 'block');
    $("#menu_PO_VendorInfo").parent().addClass('active');

    document.title = "Thông tin NCC";
    //fillter & form popup
    $("#Status").select2();
    $("#s2id_Status").css('width', '240px');
    $("#ProvinceID").select2();
    $("#s2id_ProvinceID").css('width', '240px');

    $("#selectIsActive_search").select2();
    $("#s2id_selectIsActive_search").css('width', '100%');
    $("#SignOffDate").kendoDatePicker({
        format: "dd/MM/yyyy",
    });
    $(window).resize(function () {
        resizeGrid(numHeight);
    });

    //goi ham search khi nhan enter
    $("#txtVendorID").keypress(function (e) {
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
    $(document).bind('keydown', function (event) {
        if (eventHotKey) {
            if (event.ctrlKey || event.metaKey) {
                switch (String.fromCharCode(event.which).toLowerCase()) {
                    case 's':
                        event.preventDefault();
                        var boolCreatedBy = $("#CreatedBy").val() != "system";
                        $("#formPopup").submit();
                        break;
                }
            }
        }
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
    $('#popup').kendoWindow({
        width: "780px",
        actions: ["Close"],
        visible: false,
        resizable: false,
        resizable: false,
        close: function (e) {
            $("#divMaskPopup").hide();
        }
    });
    $("#formPopup").validate({
        // Rules for form validation
        rules: {
            VendorID: {
                required: true,
                //alphanumeric: true
            },
            VendorName: {
                required: true,
                //alphanumeric: true
            },
            Phone: {
                digits: true,
            },
            Email: {
                email: true,
            },
            SignOffDate: {
                required: true,
            },
            ProvinceID: {
                required: true,
            },
        },

        // Messages for form validation
        messages: {
            VendorID: {
                required: "Thông tin bắt buộc"
            },
            VendorName: {
                required: "Thông tin bắt buộc"
            },
            Phone: {
                digits: "Chỉ cho nhập ký tự số",
            },
            Email: {
                email: "Email không hợp lệ",
            },
            ProvinceID: {
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
                        if (keyAction == 0) {
                            // Create
                            $("#formPopup").find('fieldset:eq(0) section:eq(0)').empty().append('<div style="float: left; width: 120px; margin-left:0;"><label class="label" style="float: right;">Mã NCC (*) </label></div><div style="float: left; width: 240px; margin-right: 0; margin-left: 10px;"><label style="float: right;text-align: left;width: 240px;height: 19px;padding-top: 2px;"><strong style="color:red;">' + data.VendorID + '</strong><input type="hidden" id="VendorID" name="VendorID" value="' + data.VendorID + '" /></label> <div style="clear:both"></div></div>');
                            $("#CreatedAt").val(dateToString(data.CreatedAt));
                            $("#CreatedBy").val(data.CreatedBy);
                            keyAction = -1;
                        }
                        $("#grid").data("kendoGrid").dataSource.read();
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

//popup
function onOpenPopup(key, obj) {
    eventHotKey = true;
    $("#formPopup").find('section em').remove();
    $("#formPopup").find('section label').removeClass('state-error').removeClass('state-success');
    idPopup = ".k-window";
    $("#divMaskPopup").show();
    var popup = $('#popup').data("kendoWindow");
    popup.center().open();
    $(".k-window span.k-i-close").click(function () {
        eventHotKey = false;
        $("#divMaskPopup").hide();
    });
    //Tạo mới
    if (key == 0) {
        keyAction = key;
        popup.title('Thêm');
        //onRefreshPopup
        $("#formPopup").find('fieldset:eq(0) section:eq(0)').hide();
        $('#formPopup')[0].reset();
        $("#formPopup").find('fieldset:eq(0) section:eq(0)').empty().append('<div style="float: left; width: 120px; margin-left:0;"><label class="label" style="float: right;">Mã NCC (*) </label>');
        $("#formPopup").find('fieldset:eq(1) section:eq(0)').empty().append('<div style="float: left; height:24px; margin-left:0;"><label class="label" style="float: right;"> </label> </div>');
        $("#formPopup").find('fieldset:eq(1) section:eq(0)').hide();
        $("#VendorID").attr('readonly', false);
        $("#CreatedBy").val('');
        $("#CreatedAt").val('');
        $("#StartDate").val('');
        $("#EndDate").val('');
        $("#VendorName").val('');
        $("#VendorID").val('');
        $("#Note").val('');
        //$("#Status option[value='True']").attr('selected', true);
        $("#Status option:selected").removeAttr('selected');
        $("#Status").trigger("change");
        $("#ProvinceID option").removeAttr('selected');
        $('#ProvinceID').trigger('change');
        setTimeout(function () {
            $("#VendorID").focus();
        }, 500);
    }
        // Cập nhậpS
    else {
        keyAction = -1;
        popup.title('Chỉnh sửa');
        var id = $(obj).data('id');
        $("#formPopup").find('fieldset:eq(0) section:eq(0)').show();
        $("#formPopup").find('fieldset:eq(1) section:eq(0)').empty().append('<div style="float: left; height:24px; margin-left:0;"><label class="label" style="float: right;height:24px"></label> </div>');
        $("#formPopup").find('fieldset:eq(1) section:eq(0)').show();
        $("#formPopup").find('fieldset:eq(0) section:eq(0)').empty().append('<div style="float: left; width: 120px; margin-left:0;"><label class="label" style="float: right;">Mã NCC (*)</label></div><div style="float: left; width: 240px; margin-right: 0; margin-left: 10px;"> <label style="float: right;text-align: left;width: 240px;height: 19px;padding-top: 2px;"><strong style="color:red;">' + id + '</strong><input type="hidden" id="VendorID" name="VendorID" value="' + id + '"/></label> <div style="clear:both"></div></di>');
        var currentRow = $(obj).closest("tr");
        var dataItem = $("#grid").data("kendoGrid").dataItem(currentRow);
        onBindDataToFormVendor(dataItem);
        var status = dataItem.Status == true ? "True" : "False";
        $("#Status").val(status);
        $("#Status").trigger('change');
        $('#ProvinceID').val(dataItem.ProvinceID);
        $('#ProvinceID').trigger('change');
        setTimeout(function () {
            $("#VendorName").focus();
        }, 500);
    }
}

// Load lại data khi tạo mới hoặc cập nhật
function onBindDataToFormVendor(dataItem) {
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

//search
function doSearch() {
    var grid = $("#grid").data("kendoGrid");
    var filter = { logic: "and", filters: [] };
    var text = $("#txtVendorID").val();
    if (text) {
        var filterOr = { logic: "or", filters: [] };
        filterOr.filters.push({ field: "VendorID", operator: "contains", value: text });
        filterOr.filters.push({ field: "VendorName", operator: "contains", value: text });
        filter.filters.push(filterOr);
    }
    text = $("#selectIsActive_search").val();
    if (text) {
        filter.filters.push({ field: "Status", operator: "eq", value: text });
    }

    grid.dataSource.filter(filter);
}

function onDatabound(e) {
    resizeGrid(numHeight);
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
    changeStatusGrid('grid', 6);
    $("#divLoading").hide();
}
function onRequestStart(e) {
    $("#divLoading").show();
}
function onRequestEnd(e) {
    if (e.type == "update" || e.type == "create" || e.type == "delete") {
        if (e.response.Errors == null) {
            alertBox("Thành công! ", "Lưu thành công", true, 3000);
        }
        else {
            alertBox("Báo lỗi", e.response.Errors.er.errors[0], false, 3000);
        }
    }
    $("#divLoading").hide();
}
function resizeGrid(number) {
    var h_search = $(".divSearch").height();
    var h = parseInt($(window).height()) - h_search;
    var content = $("#grid").find(".k-grid-content");
    content.height(h - number);
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
        alertBox("Báo lỗi!", " Chọn file để nhập Excel", false, 3000);
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
//Format kieu thời gian
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
function changeStatusGrid(idGrid) {
    var arrRow = $("#" + idGrid).find(".k-grid-content table tbody tr");
    for (var i = 0; i < arrRow.length; i++) {
        var arrCol = arrRow[i].cells;
        if (arrCol.length == 0) {
            continue;
        }
        //lưu ý nên để những đoạn VendorID change chỉ cần thay đổi màu sắc lên trên những đoạn VendorID thay đổi giá trị của cột
        //đoạn VendorID bên dưới chỉ là đoạn VendorID ví dụ đổi màu của cột dựa theo những thông số trên cột khác
        //changeNameByIsActive(arrCol);

        //'change' trong 1 column không liên quan những column khác
        changeIsActive(arrCol);
    }
    $("#divLoading").hide();
}

function changeIsActive(arrCol) {
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
        if (attr == 'Status') {
            if (tr.textContent == "true") {
                //$(tr).empty().append('<span class="label label-success" style="font-size:12px">Đang hoạt động</span>');
                $(tr).empty().append('<span class="label-success" style="font-size:12px">Đang hoạt động</span>');
            }
            else if (tr.textContent == "false") {
                //$(tr).empty().append('<span class="label label-danger" style="font-size:12px">Ngưng hoạt động</span>');
                $(tr).empty().append('<span class="label-success" style="font-size:12px">Ngưng hoạt động</span>');
            }
        }
    }
}



