var numHeight = 380;
var keyAction;
var indexTabstripActive = -1;
var contentTab;
$(document).ready(function () {

    //active menu
    resetMenu();
    activeMenu(1, 2, 1, '2_1', 'menu_Transporter');

    document.title = "Danh sách đơn vị vận chuyển";

    //fillter & form popup
    $("#selectIsActive_search").select2();
    $("#s2id_selectIsActive_search").css('width', '100%');

    $("#selectIsActive_searchlocation").select2();
    $("#s2id_selectIsActive_searchlocation").css('width', '100%');
    //$("#RegionF").select2();
    //$("#s2id_RegionF").css('width', '100%');

    $("#Status").select2();
    $("#s2id_Status").css('width', '240px');
    

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

    $("#formPopupDe").validate({
        // Rules for form validation
        rules: {
            TransporterID: {
                required: true,
                alphanumeric: true,
                maxlength: 10
            },
            TransporterName: {
                required: true,
            },
            Weight: {
                digits: true,
            },
           
        },

        // Messages for form validation
        messages: {
            TransporterID: {
                required: "Thông tin bắt buộc",
                alphanumeric: "Chỉ cho phép nhập ký tự alpha(0-9,a-z,A-Z,_)",
                maxlength: "Chiều dài cho phép là {0} ký tự"
            },
            Weight: {
                digits: "Chỉ cho phép nhập lý tự số",
            },
            TransporterName: {
                required: "Thông tin bắt buộc",
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
                            $("#formPopupDe").find('fieldset:eq(0) section:eq(0)').empty().append('<div class="divlabel"><label class="label" style="float:right">Mã ĐVVC(*)</label></div><div class="divfile"><label style="float: right;text-align: left;width: 240px;height: 19px;padding-top: 2px;"><strong style="color:red;">' + data.TransporterID + '</strong><input type="hidden" id="TransporterID" name="TransporterID" value="' + data.TransporterID + '" /></label> <div style="clear:both"></div>');
                            $("#CreatedAt").val(dateToString(data.CreatedAt));
                            $("#CreatedBy").val(data.CreatedBy);
                            keyAction = -1;
                        }
                        readHeaderInfo();
                        //$("#popup").data("kendoWindow").close();
                        //$("#divMaskPopup").hide();
                        $("#grid").data("kendoGrid").dataSource.read();
                        alertBox("Thành công! ", "Lưu thành công", true, 3000);
                        
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
    $("#txtTransporterID").keypress(function (e) {
        if (e.keyCode == 13) {
            doSearch();
        }
    });
    $("#txtTransporterLocationID").keypress(function (e) {
        if (e.keyCode == 13) {
            doSearchTransporterlocation();
        }
    });

    //
    $("#txtTransporterIDTe").keypress(function (e) {
        if (e.keyCode == 13) {
            doSearchLocationterritory();
        }
    });
    $("#txtProvince").keypress(function (e) {
        if (e.keyCode == 13) {
            doSearchLocationterritory();
        }
    });
    $("#txtDistrict").keypress(function (e) {
        if (e.keyCode == 13) {
            doSearchLocationterritory();
        }
    });
    $("#txtTransporterIDLoc").keypress(function (e) {
        if (e.keyCode == 13) {
            doSearchTransporterlocation();
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
                            $("#formPopupDe").submit();
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
    var text = $("#txtTransporterID").val();
    if (text) {
        var filterOr = { logic: "or", filters: [] };
        if ($.isNumeric(text)) {
            filterOr.filters.push({ field: "TransporterID", operator: "eq", value: text });
        }
        
        filterOr.filters.push({ field: "TransporterName", operator: "contains", value: text });
        filter.filters.push(filterOr);
    }
    text = $("#selectIsActive_search").val();
    if (text) {
        filter.filters.push({ field: "Status", operator: "eq", value: text });
    }
    grid.dataSource.filter(filter);
}
function doSearchTransporterlocation() {
    debugger;
    var grid = $("#gridlocation").data("kendoGrid");
    var filter = { logic: "and", filters: [] };
    var text = $("#txtTransporterLocationID").val();
    if (text) {
        var filterOr = { logic: "or", filters: [] };
        if ($.isNumeric(text)) {
            filterOr.filters.push({ field: "TransporterLocationID", operator: "eq", value: text });
        }

        filterOr.filters.push({ field: "TransporterLocationName", operator: "contains", value: text });
        filter.filters.push(filterOr);
    }
    var text = $("#txtTransporterIDLoc").val();
    if (text) {
        var filterOr = { logic: "or", filters: [] };
        filterOr.filters.push({ field: "TransporterName", operator: "contains", value: text });
        filter.filters.push(filterOr);
    }
    text = $("#selectIsActive_searchlocation").val();
    if (text) {
        filter.filters.push({ field: "Status", operator: "eq", value: text });
    }
    grid.dataSource.filter(filter);
}
function doSearchLocationterritory() {
   
    var grid = $("#gridLocationterritory").data("kendoGrid");
    var filter = { logic: "and", filters: [] };
    debugger;
    var text = $("#txtTransporterIDTe").val();
    if (text) {
        var filterOr = { logic: "or", filters: [] };
        if ($.isNumeric(text)) {
            filterOr.filters.push({ field: "TransporterLocationID", operator: "eq", value: text });
        }

        filterOr.filters.push({ field: "TransporterLocationName", operator: "contains", value: text });
        filter.filters.push(filterOr);
    }
    var text = $("#txtProvince").val();
    if (text) {
        var filterOr = { logic: "or", filters: [] };
        filterOr.filters.push({ field: "ProvinceID", operator: "contains", value: text });
        filterOr.filters.push({ field: "ProvinceName", operator: "contains", value: text });
        filter.filters.push(filterOr);
    }
    
    var text = $("#txtDistrict").val();
    if (text) {
        var filterOr = { logic: "or", filters: [] };
        filterOr.filters.push({ field: "DistrictID", operator: "contains", value: text });
        filterOr.filters.push({ field: "DistrictName", operator: "contains", value: text });
        filter.filters.push(filterOr);
    }
   
    //var listRegion = $("#RegionF option:selected");
    //var filterRegion = { logic: "or", filters: [] };
    //if (listRegion.length > 0) {
    //    for (var i = 0; i < listRegion.length; i++) {
    //        var option = listRegion[i].value;
    //        if (option == '' || option =="(All)") {
    //            filterRegion.filters.push({ field: "RegionName", operator: "contains", value: '' });
    //        }
    //        else {
    //            filterRegion.filters.push({ field: "RegionName", operator: "contains", value: option });
    //        }
    //    }
    //    filter.filters.push(filterRegion);
    //}
   
    grid.dataSource.filter(filter);
}
//grid 
function onDatabound(e) {
    resizeOtherGrid(numHeight, $("#grid"));
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
function onDataboundTranLocation(e) {
    //resizeGrid(numHeight);
    resizeOtherGrid(numHeight, $("#gridlocation"));
    var grid = $("#gridlocation").data("kendoGrid");
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
    changeStatusGrid('gridlocation', 6);
    $("#divLoading").hide();
}
//
function onDataboundLocationTerritory(e) {
    //resizeGrid(numHeight);
    resizeOtherGrid(numHeight, $("#gridLocationterritory"));
    var grid = $("#gridLocationterritory").data("kendoGrid");
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
    changeStatusGrid('gridLocationterritory', 6);
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
//popup
function onOpenPopupDe(key, obj) {
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
    if (key == 0) {     // Create
        keyAction = key;
        popup.title('Thêm');
        onRefreshPopup();
        setTimeout(function () {
            $("#TransporterID").focus();
        }, 500);
    }
    else {      // Update
        keyAction = -1;
        popup.title('Chỉnh sửa');
        var id = $(obj).data('id');
        $("#formPopupDe").find('fieldset:eq(0) section:eq(0)').empty().append('<div class="divlabel"><label class="label" style="float:right">Mã ĐVVC (*)</label></div><div class="divfile"><label style="float: right;text-align: left;width: 240px;height: 19px;padding-top: 2px;"><strong style="color:red;">' + id + '</strong><input type="hidden" id="TransporterID" name="TransporterID" value="' + id + '"/></label> <div style="clear:both"></div>');

        var currentRow = $(obj).closest("tr");
        var dataItem = $("#grid").data("kendoGrid").dataItem(currentRow);
        onBindDataToForm(dataItem);
        setTimeout(function () {
            $("#TransporterName").focus();
        }, 500);        
        $.post(r + "/DeliveryUOMManage/GetDeliveryByCode", { TransporterID: id }, function (data) {
            showLoading();
            
            if (data.success) {
                var value = data.data;
                var active = value.Status == true ? "True" : "False";
                $("#Status option[value='" + active + "']").attr('selected', true);
                $("#Status").select2();
                $("#s2id_Status").css('width', '240px');
                $("#CreatedAt").val(dateToString(value.CreatedAt));
                $("#CreatedBy").val(value.CreatedBy);
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

    contentTab = setContentTab(["TransporterID", "TransporterName", "Status", 'Weight'], "30");

}

function onRefreshPopup() {
    $("#formPopupDe").find('fieldset:eq(0) section:eq(0)').empty().append('<div class="divlabel"><label class="label" style="float:right">Mã ĐVVC (*)</label></div><div class="divfile"><label class="input" style="float:right;width:240px;"><input type="text" id="TransporterID" name="TransporterID" class="input-xs" placeholder="Mã ĐVVC" style="margin-right:85px" /></label><div style="clear:both"></div>');
    $("#TransporterName").val('');
    $("#Status option[value='True']").attr('selected', true);
    $("#Weight").val('');
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
        if (attr == 'IsActive') {
            if (tr.textContent == "true") {
                //$(tr).empty().append('<span class="label label-success" style="font-size:12px">Đang hoạt động</span>');
                $(tr).empty().append('<span class="label-success" style="font-size:12px">Đang hoạt động</span>');
            }
            else if (tr.textContent == "false") {
                //$(tr).empty().append('<span class="label label-danger" style="font-size:12px">Ngưng hoạt động</span>');
                $(tr).empty().append('<span class="label-success" style="font-size:12px">Ngưng hoạt động</span>');
            }
        }
        if (attr == 'IsAllMerchannt') {
            if (tr.textContent == "true") {
                //$(tr).empty().append('<span class="label label-success" style="font-size:12px">Đang hoạt động</span>');
                $(tr).empty().append('<span class="label-success" style="font-size:12px;">1</span>');
            }
            else if (tr.textContent == "false") {
                //$(tr).empty().append('<span class="label label-danger" style="font-size:12px">Ngưng hoạt động</span>');
                $(tr).empty().append('<span class="label-success" style="font-size:12px;"">0</span>');
            }
        }
    }
    function resizeOtherGrid(number, gridE) {

        var h_search = $(".divSearch").height();
        var h = parseInt($(window).height()) - h_search;
        var content = gridE.find(".k-grid-content");
        content.height(h - number);
    }
}
