var numHeight = 290;
var keyAction;
var indexTabstripActive = -1;
var contentTab;
$(document).ready(function () {
   
    resetMenu()
    $("#menu_WareHouse").parent().addClass('active');

    document.title = "Danh mục kho";
    //fillter & form popup
    $("#selectStatus_search").select2();
    $("#s2id_selectStatus_search").css('width', '100%');
    $("#selectStatus_searchWHL").select2();
    $("#s2id_selectStatus_searchWHL").css('width', '100%');

    $("#Status").select2();
    $("#s2id_Status").css('width', '240px');
    $(window).resize(function () {
        resizeGrid(numHeight, $('#gridWH'));
    });

    //goi ham search khi nhan enter
    $("#txtWHName").keypress(function (e) {
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
                        if (indexTabstripActive == 0 && $("#btnInsert").length > 0 && boolCreatedBy) {
                            $("#formPopup").submit();
                        }
                        else if (indexTabstripActive == 1 && $("#btnInsert").length > 0) {
                            onSaveRole();
                        }
                        else if (indexTabstripActive == 2 && $("#btnInsert").length > 0) {
                        }
                        break;
                }
            }
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

    $("#importform").ajaxForm({
        beforeSend: function () {
            $("#popupImport").data("kendoWindow").close();
        },
        uploadProgress: function (event, position, total, percentComplete) { },
        success: function (data) {
            if (data.success) {
                $("#gridWH").data("kendoGrid").dataSource.read();
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
        width: "500px",
        actions: ["Close"],
        visible: false,
        resizable: false,
        //open: function (e) {
        //    this.wrapper.css({ top: "115px" });
        //    //this.wrapper.css({ top: $('#header').height() });
        //}
    });
    $("#formPopup").validate({
        // Rules for form validation
        rules: {
            WHName: {
                required: true,
                //alphanumeric: true
            },
        },

        // Messages for form validation
        messages: {
            WHName: {
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
                            $("#formPopup").find('fieldset:eq(0) section:eq(0)').empty().append('<label class="label" style="float: left;">Mã kho</label><label style="float: right;text-align: left;width: 240px;height: 19px;padding-top: 2px;"><strong style="color:red;">' + data.Code + '</strong><input type="hidden" id="WHID" name="WHID" value="' + data.Code + '" /></label> <div style="clear:both"></div>');
                            $("#CreatedAt").val(dateToString(data.createdate));
                            $("#CreatedBy").val(data.createdby);
                            keyAction = -1;
                        }
                        readHeaderInfo();
                        $("#gridWH").data("kendoGrid").dataSource.read();
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
 //$('#btnWh').click(function () { alert(); });
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
        $("#formPopup")[0].reset();
        $("#formPopup").find('fieldset:eq(0) section:eq(0)').hide();
        $("#CreatedAt").val('');
        $("#CreatedBy").val('');
        $("#formPopup").find('fieldset:eq(0) section:eq(0)').empty();
        setTimeout(function () {
            $("#WHName").focus();
        }, 500);
    }
        // Cập nhập
    else {
        keyAction = -1;
        popup.title('Chỉnh sửa');
        var id = $(obj).data('id');
        $("#formPopup").find('fieldset:eq(0) section:eq(0)').show();
        //$("#formPopup").find('fieldset:eq(0) section:eq(0)').empty().append('<label class="label" style="float: left;">Mã kho</label><label style="float: right;text-align: left;width: 240px;height: 19px;padding-top: 2px;"><strong style="color:red;">' + id + '</strong><input type="hidden" id="WHID" name="WHID" value="' + id + '"/></label> <div style="clear:both"></div>');
        $("#formPopup").find('fieldset:eq(0) section:eq(0)').empty().append('<div style="float: left; width: 120px; margin-left:0;"><label class="label" style="float: right;">Mã ấn phẩm (*)</label></div><div style="float: left; width: 240px; margin-right: 0; margin-left: 10px;"> <label style="float: right;text-align: left;width: 240px;height: 19px;padding-top: 2px;"><strong style="color:red;">' + id + '</strong><input type="hidden" id="WHID" name="WHID" value="' + id + '"/></label> <div style="clear:both"></div></di>');
        var currentRow = $(obj).closest("tr");
        var dataItem = $("#gridWH").data("kendoGrid").dataItem(currentRow);
        onBindDataToForm(dataItem);
        setTimeout(function () {
            $("#WHName").focus();
        }, 500);
        readHeaderInfo();
    }
}
// Load lại data khi tạo mới hoặc cập nhật
function onBindDataToForm(dataItem) {
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
function doSearch() {
    var grid = $("#gridWH").data("kendoGrid");
    var filter = { logic: "and", filters: [] };
    var Name = $("#txtWHName").val();
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

    text = $("#selectStatus_search").val();
    if (text) {
        filter.filters.push({ field: "Status", operator: "eq", value: text });
    }

    grid.dataSource.filter(filter);
}
//grid 
function onDatabound(e) {
    resizeOtherGrid(numHeight, $('#gridWH'));
    var grid = $("#gridWH").data("kendoGrid");
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
function resizeGrid(number) {
    var h_search = $(".divSearch").height();
    var h = parseInt($(window).height()) - h_search;
    var content = $("#gridWH").find(".k-grid-content");
    content.height(h - number);
}
function readHeaderInfo() {
    contentTab = setContentTab(["WHID", "WHName", "Address", "WHKeeper", 'Note', 'Status'], "30");
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
function setContentTab(listItem, widthcontent) {
    debugger;
    contentTab = "";
    for (var i = 0; i <= listItem.length - 1; i++) {
        var itemValue = "";
        var itemLabel = "";
        var itemType = $("#" + listItem[i]).is('select');
        if ($("#" + listItem[i]).is('select')) {
            itemValue = $("#" + listItem[i] + " option:selected").text();
        }
        else if ($("#" + listItem[i]).is('label')) {
            itemValue = $("#" + listItem[i]).text();
        }
        else if ($("#" + listItem[i]).is('span')) {
            itemValue = $("#" + listItem[i]).text();
        }
        else {
            itemValue = $("#" + listItem[i]).val();
            if (itemValue == 'true') {
                itemValue = "Có";
            }
            else if (itemValue == 'false') {
                itemValue = "Không";
            }
        }
        //itemLabel
        if ($("#" + listItem[i]).is('label')) {

            itemLabel = $("#" + listItem[i]).parent().find("label:eq(0)").text();

        }
        else {

            itemLabel = $("#" + listItem[i]).parent().parent().find("label:eq(0)").text();
            //itemLabel = itemLabel.replace('(*)', '');
        }
        itemLabel = $.trim(itemLabel.replace('(*)', ''));

        itemLabel = $.trim(itemLabel.replace(':', ''));
        itemValue = $.trim(itemValue);
        contentTab += ''
                    + '<div style="width:' + widthcontent + '%;float:left">'
                        + '<label>'
                            + '<i class="fa fa-lg fa-caret-right" style="color:blue;margin-right:5px"></i>'
                            + '<span style="color:#808080; font-size: 12px;font-family: Tahoma, Verdana, Segoe, sans-serif;">' + itemLabel + ': </span>'
                        + '</label>'
                        + '<label>'
                            + '<span style=" font-size: 12px;font-weight: bold;font-family: Tahoma, Verdana, Segoe, sans-serif;">&nbsp;' + itemValue + '</span>'
                        + '</label>'
                    + '</div>';

    }
    return contentTab;
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
function resizeOtherGrid(number, gridE) {

    var h_search = $(".divSearch").height();
    var h = parseInt($(window).height()) - h_search;
    var content = gridE.find(".k-grid-content");
    content.height(h - number);
}






