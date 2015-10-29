var numHeight = 320;
var keyAction;
var indexTabstripActive = -1;
var contentTab;
$(document).ready(function () {

    //active menu
    resetMenu();
    activeMenu(1, 2, 1, '2_1', 'menu_DeliveryPromotion');

    document.title = "Chương trình khuyến mãi";

    //fillter & form popup
    $("#selectIsActive_search").select2();
    $("#s2id_selectIsActive_search").css('width', '100%');

    $("#Status").select2();
    $("#s2id_Status").css('width', '240px');
    $("#PromotionType").select2();
    $("#s2id_PromotionType").css('width', '240px');
    

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
        width: "780px",
        actions: ["Close"],
        visible: false,
        resizable: false,
    });    

    $("#formPopupDe").validate({
        // Rules for form validation
        rules: {
            PromotionID: {
                required: true,
                alphanumeric: true,
                maxlength: 10
            },
            PromotionName: {
                required: true,
            },
            Phone: {
                digits: true,
            },
           
        },

        // Messages for form validation
        messages: {
            PromotionID: {
                required: "Thông tin bắt buộc",
                alphanumeric: "Chỉ cho phép nhập ký tự alpha(0-9,a-z,A-Z,_)",
                maxlength: "Chiều dài cho phép là {0} ký tự"
            },
            PromotionName: {
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
                            $("#formPopupDe").find('fieldset:eq(0) section:eq(0)').empty().append('<div class="divlabel"><label class="label" style="float: right;">Mã CTKM: </label></div><div class="divfile"><label style="float: right;text-align: left;width: 240px;height: 19px;padding-top: 2px;"><strong style="color:red;">' + data.PromotionID + '</strong><input type="hidden" id="PromotionID" name="PromotionID" value="' + data.PromotionID + '" /></label> <div style="clear:both"></div>');
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
    $("#txtPromotionID").keypress(function (e) {
        if (e.keyCode == 13) {
            doSearch();
        }
    });
    $("#txtPromotionID").keypress(function (e) {
        if (e.keyCode == 13) {
            doSearch();
        }
    });
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
    $("#FromDate").kendoDatePicker();
    $("#EndDate").kendoDatePicker();
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
    changeStatusGrid('grid',6);
    $("#divLoading").hide();
}

function onRequestStart(e) {
    $("#divLoading").show();
}

function onRequestEnd(e) {
    if (e.type == "update" || e.type == "create" || e.type == "delete") {
        if (e.response.Errors == null) {
            alertBox("Thành công !", "Lưu thành công", true, 3000);

        }
        else {
            alertBox("Báo lỗi", e.response.Errors.er.errors[0], false, 3000);
        }
    }    
    $("#divLoading").hide();
}
//popup
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
    if (key == 0) {     // Create
        keyAction = key;
        popup.title('Thêm');
        $("#PromotionType").val('None')
        $("#PromotionType").css('width', '240px');
        $("#PromotionType").select2();
        onRefreshPopup();
        setTimeout(function () {
            $("#PromotionID").focus();
        }, 500);
    }
    else {      // Update
        keyAction = -1;
        popup.title('Chỉnh sửa');
        var id = $(obj).data('id');
        $("#formPopupDe").find('fieldset:eq(0) section:eq(0)').empty().append('<div class="divlabel"><label class="label" style="float: right;">Mã CTKM (*) </label></div><div class="divfile"><label style="float: right;text-align: left;width: 240px;height: 19px;padding-top: 2px;"><strong style="color:red;">' + id + '</strong><input type="hidden" id="PromotionID" name="PromotionID" value="' + id + '"/></label> <div style="clear:both"></div>');

        var currentRow = $(obj).closest("tr");
        var dataItem = $("#grid").data("kendoGrid").dataItem(currentRow);
        onBindDataToForm(dataItem);
        setTimeout(function () {
            $("#PromotionName").focus();
        }, 500);        
        $.post(r + "/DeliveryPromotion/GetPromotionCode", { PromotionID: id }, function (data) {
            showLoading();
            
            if (data.success) {
                var value = data.data;
                var active = value.Status == true ? "True" : "False";
                $("#Status option[value='" + active + "']").attr('selected', true);
                $("#Status").select2();
                $("#s2id_Status").css('width', '240px');
                $("#PromotionType").css('width', '240px');
                $("#PromotionType").select2();
                $("#PromotionType").val(value.PromotionType);
                $("#CreatedAt").val(dateToString(value.CreatedAt));
                $("#CreatedBy").val(value.CreatedBy);
                $('#btnSubmit').css({ 'display': 'block' })
                //readHeaderInfo();
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

    //contentTab = setContentTab(["PromotionID", "PromotionName", "Status"], "30");

}

function onRefreshPopup() {
    $("#formPopupDe").find('fieldset:eq(0) section:eq(0)').empty().append('<div class="divlabel"><label class="label" style="float: right;">Mã CTKM (*) </label></div><div class="divfile"><label class="input" style="float:right;width:240px;"><input type="text" id="PromotionID" name="PromotionID" class="input-xs" placeholder="Mã CCKM" style="margin-right:85px" /></label><div style="clear:both"></div>');
    $("#PromotionName").val('');
    $("#Status option[value='True']").attr('selected', true);
    $("#FromDate").val('');
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


