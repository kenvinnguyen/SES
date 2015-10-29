var numHeight = 230;
var keyAction;
var indexTabstripActive = -1;
var contentTab;
$(document).ready(function () {

    //active menu
    resetMenu();
    activeMenu(1, 2, 1, '2_1', 'menu_DeliveryFormula');

    document.title = "Danh sách công thức tính phí vận chuyển";

    //fillter & form popup
    $("#selectStatus_search").select2();
    $("#s2id_selectStatus_search").css('width', '100%');

    $("#Status").select2();
    $("#s2id_Status").css('width', '240px');
    $("#RuleType").select2();
    $("#s2id_DiscountionType").css('width', '240px');


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
        width: "500",
        actions: ["Close"],
        visible: false,
        resizable: false,
    });

    $("#formPopup").validate({
        // Rules for form validation
        rules: {
            FormulaName: {
                required: true,
            },
            Formula: {
                required: true,
            },
           

        },

        // Messages for form validation
        messages: {
            FormulaName: {
                required: "Thông tin bắt buộc",
            },
            Formula: {
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
                            $("#formPopup").find('fieldset:eq(0) section:eq(0)').empty().append('<div class="divlabel"><label class="label" style="float: right;">Mã công thức: </label></div><div class="divfile"><label style="float: right;text-align: left;width: 240px;height: 19px;padding-top: 2px;"><strong style="color:red;">' + data.FormulaID + '</strong><input type="hidden" id="FormulaID" name="FormulaID" value="' + data.FormulaID + '" /></label> <div style="clear:both"></div>');
                            $("#CreatedAt").val(dateToString(data.CreatedAt));
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

    $("#txtFormulaID").keypress(function (e) {
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
                        if ($("#btnInsert").length > 0) {
                            $("#formPopup").submit();
                        }
                        break;
                }
            }
        }
    });
    //$("#FromDate").kendoDatePicker();
    //$("#EndDate").kendoDatePicker();
});

//========================================== code co ban ====================================

//search
function doSearch() {
    debugger;
    var grid = $("#grid").data("kendoGrid");
    var filter = { logic: "and", filters: [] };
    var text = $("#txtFormulaID").val();
    if (text) {
        var filterOr = { logic: "or", filters: [] };
        filterOr.filters.push({ field: "FormulaID", operator: "contains", value: text });
        filterOr.filters.push({ field: "FormulaName", operator: "contains", value: text });
        filterOr.filters.push({ field: "Formula", operator: "contains", value: text });
        filter.filters.push(filterOr);
    }
    text = $("#selectStatus_search").val();
    if (text) {
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

    // Get the export link as jQuery object
    var $exportLink = grid.element.find('.export');

    // Get its 'href' attribute - the URL where it would navigate to
    var href = $exportLink.attr('href');
    if (href) {
        // Update the 'page' parameter with the grid's current page
        //href = href.replace(/page=([^&]*)/, 'page=' + requestObject.page || '~');

        // Update the 'sort' parameter with the grid's current sort descriptor
        href = href.replace(/sort=([^&]*)/, 'sort=' + requestObject.sort || '~');

        // Update the 'pageSize' parameter with the grid's current pageSize
        //href = href.replace(/pageSize=([^&]*)/, 'pageSize=' + grid.dataSource._pageSize);

        //update filter descriptor with the filters applied

        href = href.replace(/filter=([^&]*)/, 'filter=' + (requestObject.filter || '~'));

        // Update the 'href' attribute
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
            alertBox("Báo lỗi!", e.response.Errors.er.errors[0], false, 3000);
        }
    }
    $("#divLoading").hide();
}
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
    if (key == 0) {     // Create
        keyAction = key;
        popup.title('Thêm');
        //$("#RuleType").val('None')
        //$("#RuleType").css('width', '240px');
        //$("#RuleType").select2();
        onRefreshPopup();
        setTimeout(function () {
            $("#FormulaID").focus();
        }, 500);
    }
    else {      // Update
        keyAction = -1;
        popup.title('Chỉnh sửa');
        var id = $(obj).data('id');
        $("#formPopup").find('fieldset:eq(0) section:eq(0)').empty().append('<div class="divlabel"><label class="label" style="float: right;">Mã công thức: </label></div><div class="divfile"><label style="float: right;text-align: left;width: 240px;height: 19px;padding-top: 2px;"><strong style="color:red;">' + id + '</strong><input type="hidden" id="FormulaID" name="FormulaID" value="' + id + '"/></label> <div style="clear:both"></div>');

        var currentRow = $(obj).closest("tr");
        var dataItem = $("#grid").data("kendoGrid").dataItem(currentRow);
        onBindDataToForm(dataItem);
        setTimeout(function () {
            $("#FormulaName").focus();
        }, 500);
        $.post(r + "/DeliveryFormula/GetFormulaByID", { FormulaID: id }, function (data) {
            showLoading();

            if (data.success) {
                var value = data.data;
                var active = value.Status == true ? "True" : "False";
                $("#Status option[value='" + active + "']").attr('selected', true);
                $("#Status").select2();
                $("#s2id_Status").css('width', '240px');
                //$("#RuleType").css('width', '240px');
                //$("#RuleType").select2();
                //$("#RuleType").val(value.RuleType);
                $("#CreatedAt").val(dateToString(value.CreatedAt));
                $("#CreatedBy").val(value.CreatedBy);
                $('#btnSubmit').css({ 'display': 'block' })
              
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

    //contentTab = setContentTab(["FormulaID", "PromotionName", "Status"], "30");

}

function onRefreshPopup() {
    $("#formPopup").find('fieldset:eq(0) section:eq(0)').empty().append('<div class="divlabel"><label class="label" style="float: right;">Mã công thức: </label></div><div class="divfile"><label class="input" style="float:right;width:240px;"><input type="text" id="FormulaID" name="FormulaID" class="input-xs" placeholder="Mã công thức" style="margin-right:85px" /></label><div style="clear:both"></div>');
    $("#FormulaName").val('');
    $("#Status option[value='True']").attr('selected', true);
    $("#Formula").val('');
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

