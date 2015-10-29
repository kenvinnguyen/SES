var checkedIds = new Array();
var numHeight = 230;
var keyAction;
var indexTabstripActive = -1;
var currentAnnouncementID = "";
var editor;
var contentTab;

$(document).ready(function () {

    //active menu
    resetMenu();
    $("ul#menuLeft").find('li:eq(1)').addClass('open');
    $("ul#menuLeft").find('li:eq(1) ul#ul_root_2').css('display', 'block');
    $("ul#menuLeft").find('li:eq(1) ul#ul_root_2 ul#ul_item_1').css('display', 'block');
    $("ul#menuLeft").find('li:eq(1) ul#ul_root_2 ul#ul_item_1 ul#ul_item2_2').css('display', 'block');
    $("#menu_HOAdminAuthAnnouncement").parent().addClass('active');

    document.title = "Thông báo";
    loadToolbarStyle();

    //tabstrip
    //$("#tabstrip").kendoTabStrip({
    //    animation: { open: { effects: "fadeIn" } },
    //    activate: onActivateTabstrip,
    //});

    //fillter & form popup
    $("#selectIsActive_search").select2();
    $("#s2id_selectIsActive_search").css('width', '100%');

    $("#Status").select2();
    $("#s2id_Status").css('width', '240px');

    //editor
    $(function () { createEditor('en', 'HTMLContentView') });
    $(window).resize(function () {
        resizeGrid(numHeight);
    });

    //check box on grid
    $('#checkAll').bind('click', function () {
        
        if ($(this).is(':checked')) {
            $('.checkvalue').each(function () {
                if (!$(this).is(':checked')) {
                    $(this).trigger('click');
                }
            })
        }
        else {
            $('.checkvalue').each(function () {
                if ($(this).is(':checked')) {
                    $(this).trigger('click');
                }
            })
        }
    });
    $('#popupImport').kendoWindow({
        width: "600px",
        actions: [ "Close"],
        title: "Import",
        visible: false,
        resizeable: false,
      
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
                    alertBox("Thành công!", "Cập nhật thành công!", true, 3000);
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

    //popup
    $('#popup').kendoWindow({
        width: parseInt($(window).width()) - $('#header').height() - $('#header').height(),
        actions: ["Close"],
        visible: false,
        resizable: false,
        //open: function (e) {
        //    this.wrapper.css({ top: $('#header').height() });
        //}
    });

    //form trong popup
    $("#formPopup").validate({
        // Rules for form validation
        rules: {
            AnnouncementID: {
                required: true,
                alphanumeric: true
            },
            Title: {
                required: true
            },
           
        },

        // Messages for form validation
        messages: {
            AnnouncementID: {
                required: "Thông tin bắt buộc"
            },
            Title: {
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
                data: '{"HTMLContent":"' + CKEDITOR.instances['HTMLContentView'].getData() + '"}',
                beforeSend: function () {
                    $("#loading").removeClass('hide');
                    return;
                },
                success: function (data) {
                    if (data.success) {
                        if (keyAction == 0) {   // Create
                            //$("#formPopup").find('fieldset:eq(0) section:eq(0)').empty().append('<label class="label">Mã thông báo: <strong style="color:red;margin-left: 20px;">' + data.
                            + '</strong></label><input type="hidden" id="AnnouncementID" name="AnnouncementID" value="' + data.AnnouncementID + '" />';
                            $("#formPopup").find('fieldset:eq(0) section:eq(0)').empty().append('<label class="label" style="float: left; width: 120px;">Mã thông báo: </label><label style="float: left;text-align: left;width: 240px;height: 19px;padding-top: 2px;"><strong style="color:red;">' + data.AnnouncementID + '</strong><input type="hidden" id="AnnouncementID" name="AnnouncementID" value="' + data.AnnouncementID + '" /></label> <div style="clear:both"></div>');
                            $("#CreatedAt").val(dateToString(data.createdat));
                            $("#CreatedBy").val(data.createdby);
                            currentAnnouncementID = data.AnnouncementID;
                            keyAction = -1;
                        }
                        readHeaderInfo();
                        //showTabStrip();
                        $("#grid").data("kendoGrid").dataSource.read();
                        //selectedTabstrip(1);
                        alertBox("Thành công !", "Lưu thành công",true, 3000);
                    }
                    else {
                        $("#loading").addClass('hide');
                        alertBox("Báo lỗi", data.message, false, 3000);
                        console.log(data.message);
                    }
                    $("#loading").addClass('hide');
                }
            });
            return false;
        }
    });

    //goi ham search khi nhan enter
    $("#txtAnnouncementID,#AppliedFor_search").keypress(function (e) {
        if (e.keyCode == 13) {
            doSearch();
            return false;
        }
    });

    $("#selectIsActive_search").change(function () {
       // doSearch();
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
                        if (indexTabstripActive == -1) {
                            indexTabstripActive = 0;
                        }
                        if (indexTabstripActive == 0 && $("#btnInsert").length > 0) {
                            $("#formPopup").submit();
                        }
                        else if (indexTabstripActive == 1 && $("#btnInsert").length > 0) {
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

    var text = $("#txtAnnouncementID").val();
    if (text) {
        var filterOr = { logic: "or", filters: [] };
        if ($.isNumeric(text)) {
            filterOr.filters.push({ field: "AnnouncementID", operator: "eq", value: text });
        }
        filterOr.filters.push({ field: "Title", operator: "contains", value: text });
        filter.filters.push(filterOr);
    }
    var text = $("#AppliedFor_search").val();
    if (text) {
        filter.filters.push({ field: "AppliedFor", operator: "contains", value: text });
    }
    text = $("#selectIsActive_search").val();
    if (text) {

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

    changeStatusGrid('grid', 4);

    //load checkbox of selected rows
    
    for (var i = 0; i < checkedIds.length; i++) {
        
        $('#grid').find("input[class='checkvalue'][data-id='" + checkedIds[i] + "']").prop('checked', true);
    }
    //

    //$("#gridForm input[name='SalePriceOrther']")

    if (checkedIds.length > 0) {
        $("#btnDelete").css('opacity', '1');
    } else {
        $("#btnDelete").css('opacity', '0.3');
    }
}

function checkBox(obj) {
    
    if ($(obj).is(':checked')) {
        var index = checkedIds.indexOf($(obj).data('id'));
        if (index < 0) {
            //khong co moi pust vao, co roi thi bo qua
            checkedIds.push($(obj).data('id'));
        }
       
    }
    else {
        var index = checkedIds.indexOf($(obj).data('id'));
        if (index > -1) {
            checkedIds.splice(index, 1);
        }
    }
    if (checkedIds.length > 0) {
        $('#btnDelete').css('opacity', '1');
    } else {
        $('#btnDelete').css('opacity', '0.3');
    }
  
}

function checkBoxAll() {
    

}

function onRequestStart(e) {
    $("#divLoading").show();
}

function onRequestEnd(e) {
    
    if (e.type == "update" || e.type == "create" ) {
        if (e.response.Errors == null) {
            
            alertBox("Thành công! ", "Lưu thành công", true, 3000);

        }
        else {
            alertBox("Báo lỗi", e.response.Errors.er.errors[0], false, 3000);
        }
    }
    
    else if (e.type == "read") {
        
    }
    else {
        arrExport = new Array();
        for (var i = 0; i < e.sender._view.length; i++) {
            var value = e.sender._view[i];
            arrExport.push(value.AnnouncementID);
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
    
    //var htmlContentDivHeight = parseInt($(window).height()) - $('#htmlContentInfor').height() - 260;
    //$("#htmlContentDiv").height(htmlContentDivHeight);
    //$("#HTMLContentView").height(htmlContentDivHeight - $('#htmlContentInfor').height());
    if (key == 0) {     // Create
        keyAction = 0;
        popup.title('Thêm');
        onRefreshPopup();
        setTimeout(function () {
            $("#Title").focus();
        }, 500);
        indexTabstripActive = 0;
        //currentAnnouncementID = "";
       
    }
    else {      // Update       
        keyAction = -1;
        popup.title('Chỉnh sửa');
        var id = $(obj).data('id');
        //showTabStrip();
        //$("#formPopup").find('fieldset:eq(0) section:eq(0)').empty().append('<label class="label">Mã thông báo (*)<strong style="color:red;margin-left:10%">' + id + '</strong></label><input type="hidden" id="AnnouncementID" name="AnnouncementID" value="'+id+'"/>');
        $("#formPopup").find('fieldset:eq(0) section:eq(0)').empty().append('<label class="label" style="float: left; width: 120px;">Mã thông báo (*) </label><label style="float: left;text-align: left;width: 240px;height: 19px;padding-top: 2px;"><strong style="color:red;">' + id + '</strong><input type="hidden" id="AnnouncementID" name="AnnouncementID" value="' + id + '"/></label> <div style="clear:both"></div>');
        var currentRow = $(obj).closest("tr");
        var dataItem = $("#grid").data("kendoGrid").dataItem(currentRow);
        onBindDataToForm(dataItem);
        setTimeout(function () {
            $("#Title").focus();
        }, 500);
        showLoading();
        $.post(r + "/HOAdminAuthAnnouncement/GetByID", { id: id }, function (data) {
            if (data.success) {
               
                var value = data.data;
                currentAnnouncementID = value.AnnouncementID;
                var active = value.IsActive == true ? "True" : "False";
                $("#AnnouncementID").val(value.AnnouncementID);
                $("#Title").val(value.Title);
            
                $("#TextContent").val(value.TextContent);
                
                // set value
                //$("#HTMLContentView").data("kendoEditor").value(htmlDecode(value.HTMLContentView));
                CKEDITOR.instances['HTMLContentView'].setData(htmlDecode(value.HTMLContent));
                $("#HTMLContent").val(value.HTMLContent);
                $("#CreatedAt").val(dateToString(value.CreatedAt));
                $("#CreatedBy").val(value.CreatedBy);

                $("#Status option[value='" + active + "']").attr('selected', true);
                $("#Status").select2();
                $("#s2id_Status").css('width', '240px');
               
                
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

    contentTab = setContentTab(["AnnouncementID", "Title"], "30");

}

function onRefreshPopup() {
   
    $('#formPopup')[0].reset();
    $("#formPopup").find('fieldset:eq(0) section:eq(0)').empty();
    //$("#formPopup").find('fieldset:eq(0) section:eq(0)').empty().append('<label class="label">Mã thông báo (*)<strong style="color:red;margin-left:10%"></strong></label><input type="hidden" id="AnnouncementID" name="AnnouncementID" value="0"/>');

    //$("#formPopup").find('fieldset:eq(0) section:eq(0)').empty().append('<label class="label" style="float:left">Mã thông báo (*)(*)</label><label class="input" style="float:right;width:240px;"><input type="text" id="AnnouncementID" name="AnnouncementID" class="input-xs" placeholder="Mã thông báo" style="margin-right:85px" /><b class="tooltip tooltip-top-right">Mã thông báo</b></label><div style="clear:both"></div>');
    //$("#HTMLContentView").data("kendoEditor").value("");
    CKEDITOR.instances['HTMLContentView'].setData(htmlDecode(""));
    $("#Status option[value='True']").attr('selected', true);
    $("#CreatedAt").val('');
    $("#CreatedBy").val('');
    //hideTabStrip();
}

function deleteSelected() {
    
    //var data = checkedIds;
    if (checkedIds != "" && checkedIds != null) {
        $.SmartMessageBox({
            title: "",
            content: "Bạn có muốn xóa không?",
            buttons: '[Không][Có]'
        }, function (ButtonPressed) {
            if (ButtonPressed === "Có") {
                
                $.post(r + "/HOAdminAuthAnnouncement/Deactive", { data: checkedIds.toString() }, function (data) {
                    if (data.success) {
                        
                        //$('#grid').attr('data-kendogrid-selected', '');
                        checkedIds = new Array();
                        //$.gritter.add({
                        //    // (string | mandatory) the heading of the notification
                        //    title: '',
                        //    // (string | mandatory) the text inside the notification
                        //    text: 'Xóa thành công',
                        //    class_name: 'gritter-success'
                        //});

                        alertBox("Thành công! ", "Xóa thành công", true, 3000);
                   
                        $("#grid").data("kendoGrid").dataSource.read();
                    }
                    else {
                        //$.gritter.add({
                        //    // (string | mandatory) the heading of the notification
                        //    title: '',
                        //    // (string | mandatory) the text inside the notification
                        //    text: data.message,
                        //    class_name: 'gritter-error'
                        //});

                        alertBox(data.message, "", false, 3000);

                    }
                });
            }
            if (ButtonPressed === "Không") {
                return;
            }

        });
    } else {
        return;
    }
}

//import
function openImport() {
    idPopup = ".k-window";
    $("#divMaskPopup").show();
    $('#popupImport').data("kendoWindow").open();
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

//tabstrip
//function onActivateTabstrip(e) {
//    var index = $(e.item).index();

//    indexTabstripActive = index;

//    if (index != 0) {
//        var currentTab = e.contentElement.id;
//        getContentTab(currentTab, contentTab);
//    }

//    setTimeout(function () {
//        $("#tabstrip").find('div.k-content:eq(' + index + ')').css('height', '100%');
//    }, 500);

//}

//function selectedTabstrip(index) {

//    indexTabstripActive = index;
//    $("#tabstrip").kendoTabStrip().data("kendoTabStrip").select(index);

//    if (index != 0) {
//        var currentTab = 'tabstrip-' + (index + 1);
//        getContentTab(currentTab, contentTab);
//    }


//    setTimeout(function () {
//        $("#tabstrip").find('div.k-content:eq(' + index + ')').css('height', '100%');
//    }, 500);


//}

//function showTabstrip() {
//    $("#tabstrip").find('ul li:not(:first)').show();
//}

//function hideTabStrip() {
//    $("#tabstrip").find('ul li:not(:first)').hide();
//    setTimeout(function () {
//        $("#tabstrip").find('div.k-content:eq(0)').css('height', '100%');
//    }, 1000);
//    selectedTabstrip(0);
//}

//========================================== code khac neu co ====================================

//Save Form
function onSaveForm() {
    var editor_data = CKEDITOR.instances['HTMLContentView'].getData();
    var AnnouncementID = $("#AnnouncementID").val();
    var Title = $("#Title").val();
    var TextContent = $("#TextContent").val();
    var Status = $("#Status").val();
    var CreatedAt= $("#CreatedAt").val();
    var CreatedBy = $("#CreatedBy").val();

    var form = $('#formPopup');
    var action = form.attr("action");
    var serializedForm = form.serializeArray();

    var data = { AnnouncementID: AnnouncementID, Title: Title, HTMLContentView: editor_data, TextContent: TextContent, Status: Status, CreatedAt: CreatedAt };
    $("#loadingSaveForm").removeClass('hide');
    $("#btnSaveForm").attr('disabled', true);
    $.post(r + "/HOAdminAuthAnnouncement/Create",
        { data: data }
        , function (data) {
        if (data.success) {
            alertBox("Thành công!","Lưu thành công", true, 3000);
            $("#grid").data("kendoGrid").dataSource.read();
        }
        else {
            alertBox("Báo lỗi", "Chưa lưu được", false, 3000);
            console.log(data.message);
        }
        $("#loadingSaveForm").addClass('hide');
        $("#btnSaveForm").attr('disabled', false);
    });
}

//ckeditor
function createEditor(languageCode, id) {
    var htmlContentDivHeight = parseInt($(window).height()) - $('#htmlContentInfor').height() - 460;
    var editor = CKEDITOR.replace(id, { language: languageCode, height: htmlContentDivHeight });
    CKEDITOR.instances['HTMLContentView'].on('change', function () {
        var editor_data = CKEDITOR.instances['HTMLContentView'].getData();
        $('#HTMLContent').val(htmlEncode(editor_data));
    });
}

function getAnnouncementID() {

    return { AnnouncementID: currentAnnouncementID };
}
