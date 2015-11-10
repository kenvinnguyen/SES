var numHeight = 320;
var keyAction;
var generateTreeList = false;
var selectedMenu = "";

$(document).ready(function () {
    //active menu
    resetMenu();
    $("#menu_Customer").parent().addClass('active');

    document.title = "Khách hàng";
    //fillter & form popup
    $("#selectStatus_search").select2();
    $("#s2id_selectStatus_search").css('width', '100%');

    $("#Status").select2();
    $("#s2id_Status").css('width', '240px');
    $("#ProvinceID").select2();
    $("#s2id_ProvinceID").css('width', '240px');
    $("#DistrictID").select2();
    $("#s2id_DistrictID").css('width', '240px');
    $("#Shoptype").select2();
    $("#s2id_Shoptype").css('width', '240px');
    //onCheck();
    $("#DistrictID").prop("disabled", true);
    $(window).resize(function () {
        resizeGrid(numHeight);
    });

    //goi ham search khi nhan enter
    $("#txtCustomerID").keypress(function (e) {
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
                alertBox("Báo lỗi! ",  data.message , false, 3000);
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
        //open: function (e) {
        //    this.wrapper.css({ top: "115px" });
        //    //this.wrapper.css({ top: $('#header').height() });
        //}
    });
    $("#formPopup").validate({
        // Rules for form validation
        rules: {
            //CustomerID: {
            //    required: true,
            //    alphanumeric: true,
            //    maxlength: 10
            //},
            CustomerName: {
                required: true,
            },
            Phone: {
                digits: true,
            },
            Email: {
                email: true,
            }
        },

        // Messages for form validation
        messages: {
            //CustomerID: {
            //    required: "Thông tin bắt buộc",
            //},
            CustomerName: {
                required: "Thông tin bắt buộc",
            },
            Phone: {
                digits: "Chỉ cho nhập ký tự số",
            },
            Email: {
                email: "Email không hợp lệ",
            }
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
                            $("#formPopup").find('fieldset:eq(0) section:eq(0)').show();
                            $("#formPopup").find('fieldset:eq(0) section:eq(0)').empty().append('<div style="float: left; width: 120px; margin-left:0;"><label class="label" style="float: right;">Mã KH </label></div><div style="float: left; width: 240px; margin-right: 0; margin-left: 10px;"><label style="float: right;text-align: left;width: 240px;height: 19px;padding-top: 2px;"><strong style="color:red;">' + data.CustomerID + '</strong><input type="hidden" id="CustomerID" name="CustomerID" value="' + data.CustomerID + '" /></label> <div style="clear:both"></div></div>');
                            $("#formPopup").find('fieldset:eq(1) section:eq(0)').show();
                            $("#formPopup").find('fieldset:eq(1) section:eq(0)').empty().append('<div style="float: left; width: 120px;height:24px; margin-left:0;"><label class="label" style="float: right;"></label> <div style="clear:both"></div>');
                            debugger;
                            $("#CustomerID").val(data.CustomerID);
                            $("#CreatedAt").val(dateToString(data.CreatedAt));
                            $("#CreatedBy").val(data.CreatedBy);
                            keyAction = -1;
                        }
                        readHeaderInfo();
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
        $("#formPopup")[0].reset();
        $("#formPopup").find('fieldset:eq(0) section:eq(0)').hide();
        $("#formPopup").find('fieldset:eq(0) section:eq(0)').empty();
        $("#formPopup").find('fieldset:eq(1) section:eq(0)').hide();
        $("#formPopup").find('fieldset:eq(1) section:eq(0)').empty();
        $("#ProvinceID option:selected").removeAttr("selected");
        $("#ProvinceID").trigger("change");
        $("#DistrictID option:selected").removeAttr("selected");
        $("#DistrictID").trigger("change");
        $("#Shoptype option:selected").removeAttr("selected");
        $("#Shoptype").trigger("change");
        $("#divcon").height(210);
        $("#CustomerID").val('');
        $("#CreatedAt").val('');
        $("#CreatedBy").val('');
        $("#CustomerName").focus();
        $('#Status').val("True");
        $('#Status').trigger('change');
        generateFuncTreeList("1");
        setTimeout(function () {
            $("#CustomerName").focus();
        }, 500);
    }
    // Cập nhập
    else {
        keyAction = -1;
        popup.title('Chỉnh sửa');
        var id = $(obj).data('id');
        $("#divcon").height(240);
        $("#formPopup").find('fieldset:eq(0) section:eq(0)').show();
        $("#formPopup").find('fieldset:eq(0) section:eq(0)').empty().append('<div style="float: left; width: 120px; margin-left:0;"><label class="label" style="float: right;">Mã KH (*)</label></div><div style="float: left; width: 240px; margin-right: 0; margin-left: 10px;"> <label style="float: right;text-align: left;width: 240px;height: 19px;padding-top: 2px;"><strong style="color:red;">' + id + '</strong><input type="hidden" id="CustomerID" name="CustomerID" value="' + id + '"/></label> <div style="clear:both"></div></div>');
        $("#formPopup").find('fieldset:eq(1) section:eq(0)').show();
        $("#formPopup").find('fieldset:eq(1) section:eq(0)').empty().append('<div style="float: left; width: 120px;height:24px; margin-left:0;"><label class="label" style="float: right;"></label> <div style="clear:both"></div>');
        var currentRow = $(obj).closest("tr");
        var dataItem = $("#grid").data("kendoGrid").dataItem(currentRow);
        onBindDataToForm(dataItem);
        var prov = dataItem.ProvinceID;
        var dist = dataItem.DistrictID;
        generateFuncTreeList(dataItem.CustomerID);
        var shoptype = dataItem.Shoptype;
        $("#ProvinceID").val(prov);
        $("#ProvinceID").trigger('change');
        $("#DistrictID").val(dist);
        $("#DistrictID").trigger('change');
        $("#Shoptype").val(shoptype);
        $("#Shoptype").trigger('change');
        var status = dataItem.Status == true ? "True" : "False";
        $('#Status').val(status);
        $('#Status').trigger('change');
        setTimeout(function () {
            $("#CustomerName").focus();
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

//search
function doSearch() {
    var grid = $("#grid").data("kendoGrid");
    var filter = { logic: "and", filters: [] };
    var Name = $("#txtname").val();
    if (Name) {
        var filterOr = { logic: "or", filters: [] };
        filterOr.filters.push({ field: "Name", operator: "contains", value: Name });
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
function readHeaderInfo() {
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
function ChangeDistrict() {
    if ($("#ProvinceID").val() != "" && $("#ProvinceID").val() != null) {
        $("#DistrictID").val('').trigger('change');
        $.post(r + "/Customer/GetDisByProvinceID", { ProvinceID: $("#ProvinceID").val() }, function (data) {
            if (!data.success) {
                $("#DistrictID").removeAttr("disabled");
                $("#DistrictID").html('');
                var html = "<option value =''>Chọn Tỉnh/Thành phố</option>";
                for (var i = 0; i < data.length ; i++) {
                    html += "<option value ='" + data[i].TerritoryID + "'>" + data[i].TerritoryName + "</option>";
                }
                $("#DistrictID").html(html);
            }
        });
    }
    else {
        $("#DistrictID").val('').trigger('change');
        $("#DistrictID").prop("disabled", true);
    }
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
        //lưu ý nên để những đoạn code change chỉ cần thay đổi màu sắc lên trên những đoạn code thay đổi giá trị của cột
        //đoạn code bên dưới chỉ là đoạn code ví dụ đổi màu của cột dựa theo những thông số trên cột khác
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
function showLoading() {
    $("#divLoading").show();
}

function hideLoading() {
    $("#divLoading").hide();
}
// Treelist
function generateFuncTreeList(customerid) {
    debugger;
    if (customerid == null || customerid == 'undefined' || customerid == "") {
        customerid = "";
        return;
    }
    $("#treelist").show();
    showLoading();
    $.post(r + "/Customer/GetCustomerHirerachy", { customerid: customerid }, function (data) {
        debugger;
        if (data.success) {
            var obj = $("#treelist").data('kendoTreeView');
            if (typeof obj == 'undefined') {
                $("#treelist").kendoTreeView({
                    checkboxes: {
                        name: "cbMenuTreeview",
                        checkChildren: true
                    },
                    dataSource: data.Data,
                    check: onCheck,
                    change: onChangeTreeList
                });
            }
            else {
                obj.setDataSource(new kendo.data.HierarchicalDataSource({
                    data: data.Data
                }));
            }
            $("#treelist ul:first-child").css("height", "210px");
            generateTreeList = true;
            onCheck();
        }
        else {
            alertBox("Báo lỗi", data.message, false, 3000);
            console.log(data.message);
        }
        hideLoading();
    });
}

function checkParentNode(nodes, checkedNodes) {
    if (typeof nodes != "undefined" && nodes.checked != true) {
        if (jQuery.inArray(nodes.id, checkedNodes) < 0)
            checkedNodes.unshift(nodes.id);
        if (typeof nodes.parent().parent() != "undefined") {
            checkParentNode(nodes.parent().parent(), checkedNodes);
        }
    }
}

// function that gathers IDs of checked nodes
function checkedNodeIds(nodes, checkedNodes) {
    for (var i = 0; i < nodes.length; i++) {
        if (nodes[i].checked) {
            if (typeof nodes[i].parent() != "undefined" && typeof nodes[i].parent().parent() != "undefined") {
                checkParentNode(nodes[i].parent().parent(), checkedNodes);
            }
            checkedNodes.push(nodes[i].id);
        }
        if (nodes[i].hasChildren) {
            checkedNodeIds(nodes[i].children.view(), checkedNodes);
        }
    }
}

// show checked node IDs on datasource change
function onCheck() {
    var checkedNodes = [],
        treeView = $("#treelist").data("kendoTreeView"),
        message;
    checkedNodeIds(treeView.dataSource.view(), checkedNodes);
    if (checkedNodes.length > 0) {
        selectedMenu = checkedNodes.toString();
    }
    else {
        selectedMenu = "";
    }
    console.log(selectedMenu);
}

function onDataBindingTreeList(e) {
    $("#divLoading").show();
}

function onDataBoundTreeList(e) {
    $("#divLoading").hide();
}

function onChangeTreeList(e) {
    var selectedRows = this.select();
    var dataItem = this.dataItem(selectedRows[0]);
    if (dataItem.children.view().length > 0 || dataItem.id == "Home") {
        $("#divGridAction").empty();
        return;
    }
    menuIDSelected = dataItem.id;
    onLoadGridAction();
}

function SaveCustomerHirerachy(obj) {
    debugger;
    var id = $("#CustomerID").val();
    debugger;
    if (typeof id == 'undefined' || id=="") {
        alertBox("Báo lỗi", "Khách hàng chưa được tạo!!", false, 3000);
        return;
    }
    $("#loadingSave").removeClass('hide');
    $(obj).attr('disabled', true);
    $.post(r + "/Customer/SaveCustomerHirerachy", { CustomerID: $("#CustomerID").val(), CustomerHirerachyIDs: selectedMenu }, function (data) {
        if (data.success) {
            alertBox("Lưu thành công", "", true, 3000);
        }
        else {
            alertBox("Báo lỗi", data.message, false, 3000);
            console.log(data.message);
        }
        $("#loadingSave").addClass('hide');
        $(obj).attr('disabled', false);
    });
}



