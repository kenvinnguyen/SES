var numHeight = 230;
var keyAction;
var indexTabstripActive = -1;
var contentTab;
$(document).ready(function () {

    //active menu
    resetMenu();
    activeMenu(1, 2, 1, '2_1', 'menu_HOAdminAuthUser');    

    document.title = "Người dùng";

    //tabstrip
    $("#tabstrip").kendoTabStrip({
        animation: { open: { effects: "fadeIn" } },
        activate: onActivateTabstrip,
    });

    //fillter & form popup
    $("#selectIsActive_search").select2();
    $("#s2id_selectIsActive_search").css('width', '100%');

    $("#IsActive").select2();
    $("#s2id_IsActive").css('width', '240px');

    
    generateSelect2("selectRole");    

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
        //open: function (e) {
        //    this.wrapper.css({ top: "115px" });
        //    //this.wrapper.css({ top: $('#header').height() });
        //}
    });    

    $("#formPopup").validate({
        // Rules for form validation
        rules: {
            UserID: {
                required: true,
                alphanumeric: true
            },
            DisplayName: {
                required: true
            },
            FullName: {
                required: true
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
            UserID: {
                required: "Thông tin bắt buộc"                
            },
            DisplayName: {
                required: "Thông tin bắt buộc"
            },
            FullName: {
                required: "Thông tin bắt buộc"
            },
            Phone: {
                digits: "Số điện thoại là ký tự số",
            },
            Email: {
                email: "Email không hợp lệ",
            }
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
                            $("#formPopup").find('fieldset:eq(0) section:eq(0)').empty().append('<div class="divlabel"><label class="label" style="float: right;">Mã người dùng: </label></div><div class="divfile"><label style="float: right;text-align: left;width: 240px;height: 19px;padding-top: 2px;"><strong style="color:red;">' + data.UserID + '</strong><input type="hidden" id="UserID" name="UserID" value="' + data.UserID + '" /></label> <div style="clear:both"></div></div>');
                            $("#RowCreatedAt").val(dateToString(data.RowCreatedAt));
                            $("#RowCreatedBy").val(data.RowCreatedBy);
                            keyAction = -1;
                        }
                        readHeaderInfo();

                        showTabstrip();                        
                        $("#grid").data("kendoGrid").dataSource.read();
                        //selectedTabstrip(1);
                        alertBox("Thành công! ","Lưu thành công", true, 3000);
                    }
                    else {
                        $("#loading").addClass('hide');
                        alertBox("Báo lỗi! ", data.message, false, 3000);
                        console.log(data.message);
                    }
                    $("#loading").addClass('hide');
                }
            });
            return false;
        }
    });

    //goi ham search khi nhan enter
    $("#txtUserID,#Roles_search,#Group_search").keypress(function (e) {
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
                        var boolCreatedBy = $("#RowCreatedBy").val() != "system";
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
});

//========================================== code co ban ====================================

//search
function doSearch() {
    var grid = $("#grid").data("kendoGrid");
    var filter = { logic: "and", filters: [] };    
    var text = $("#txtUserID").val();
    if (text) {
        var filterOr = { logic: "or", filters: [] };
        filterOr.filters.push({ field: "UserID", operator: "contains", value: text });
        filterOr.filters.push({ field: "DisplayName", operator: "contains", value: text });
        filterOr.filters.push({ field: "FullName", operator: "contains", value: text });

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
            alertBox("Lưu thành công", "", true, 3000);
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
        onRefreshPopup();
        setTimeout(function () {
            $("#UserID").focus();
        }, 500);
    }
    else {      // Update
        keyAction = -1;
        popup.title('Chỉnh sửa');
        var id = $(obj).data('id');
        showTabstrip();
        //$("#formPopup").find('fieldset:eq(0) section:eq(0)').empty().append('<label class="label">Mã người dùng: <strong style="color:red;margin-left: 20px;">' + id + '</strong></label><input type="hidden" id="UserID" name="UserID" value="' + id + '" />');
        $("#formPopup").find('fieldset:eq(0) section:eq(0)').empty().append('<div class="divlabel"><label class="label" style="float: right;">Mã người dùng: </label></div><div class="divfile"><label style="float: right;text-align: left;width: 240px;height: 19px;padding-top: 2px;"><strong style="color:red;">' + id + '</strong><input type="hidden" id="UserID" name="UserID" value="' + id + '" /></label> <div style="clear:both"></div></div>');

        var currentRow = $(obj).closest("tr");
        var dataItem = $("#grid").data("kendoGrid").dataItem(currentRow);
        onBindDataToForm(dataItem);
        setTimeout(function () {
            $("#DisplayName").focus();
        }, 500);        
        //selectedTabstrip(0);
        $.post(r + "/HOAdminAuthUser/GetUserByID", { userID: id }, function (data) {
            showLoading();
            
            if (data.success) {
                var value = data.data;
                var active = value.IsActive == true ? "True" : "False";
                $("#IsActive option[value='" + active + "']").attr('selected', true);
                $("#IsActive").select2();
                $("#s2id_IsActive").css('width', '240px');
                $("#RowCreatedAt").val(dateToString(value.RowCreatedAt));
                $("#RowCreatedBy").val(value.RowCreatedBy);
                if (value.RowCreatedBy == "system") {
                    $('#btnSubmit').css({ 'display': 'none' })
                }
                else {
                    $('#btnSubmit').css({ 'display': 'block' })
                }
                // Role of User
                $("#selectRole option:selected").removeAttr('selected');
                for (var i = 0; i < data.groupuser.length; i++) {
                    $("#selectRole option[value='" + data.groupuser[i].RoleID + "']").attr('selected', true);
                }

                //$("#selectRole").select2();
                //$("#s2id_selectRole").css('width', '440px');

                generateSelect2("selectRole");
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

    contentTab = setContentTab(["UserID", "DisplayName", "FullName", "IsActive", 'Phone', 'Email'], "30");

}

function onRefreshPopup() {
    $("#formPopup").find('fieldset:eq(0) section:eq(0)').empty().append('<div class="divlabel"><label class="label" style="float:right">Mã người dùng (*)</label></div><div class="divfile"><label class="input" style="float:right;width:240px;"><input type="text" id="UserID" name="UserID" class="input-xs" placeholder="Mã người dùng" style="margin-right:85px" /></label><div style="clear:both"></div></div>');
    $("#DisplayName").val('');
    $("#FullName").val('');
    $("#IsActive option[value='True']").attr('selected', true);
    $("#Phone").val('');
    $("#Email").val('');
    $("#Note").val('');
    $("#RowCreatedAt").val('');
    $("#RowCreatedBy").val('');
    $('#btnSubmit').css({ 'display': 'block' });
    $("#selectRole option:selected").removeAttr('selected');
    generateSelect2("selectRole");
    hideTabStrip();
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

//tabstrip
function onActivateTabstrip(e) {
    var index = $(e.item).index();
    indexTabstripActive = index;

    if (index != 0) {
        var currentTab = e.contentElement.id;
        getContentTab(currentTab, contentTab);
    }

    setTimeout(function () {
        $("#tabstrip").find('div.k-content:eq(' + index + ')').css('height', '100%');
    }, 1000);
}

function selectedTabstrip(index) {
    indexTabstripActive = index;
    $("#tabstrip").kendoTabStrip().data("kendoTabStrip").select(index);

    if (index != 0) {
        var currentTab = 'tabstrip-' + (index + 1);
        getContentTab(currentTab, contentTab);
    }

    setTimeout(function () {
        $("#tabstrip").find('div.k-content:eq(' + index + ')').css('height', '100%');
    }, 500);
}

function showTabstrip() {
    $("#tabstrip").find('ul li:not(:first)').show();
}

function hideTabStrip() {
    $("#tabstrip").find('ul li:not(:first)').hide();
    setTimeout(function () {
        $("#tabstrip").find('div.k-content:eq(0)').css('height', '100%');
    }, 1000);
    //selectedTabstrip(0);
}

//========================================== code khac neu co ====================================

//Cap nhat phan quyen
function onUpdatePermissionData() {
    $("#divLoading").show();
    $.post(r + "/HOAdminAuthUser/ExecPermissionData", { userID: "" }, function (data) {        
        if (data.success) {
            alertBox("Cập nhật thành công", "", true, 3000);
        }
        else {
            alertBox("Báo lỗi", data.message, false, 3000);
            console.log(data.message);
        }
        $("#divLoading").hide();
    });
}

function onResetPassword(obj) {
    var userID = $(obj).data('id');
    if (userID) {
        $.SmartMessageBox({
            title: "Khôi phục mật khẩu",
            content: "Khôi phục mật khẩu cho " + userID + " ?",
            buttons: '[Hủy][OK]'
        }, function (ButtonPressed) {
            if (ButtonPressed === "OK") {
                $("#divLoading").show();
                $(obj).attr('disabled', true);
                $.post(r + "/HOAdminAuthUser/ResetPasswordUser", { userID: userID }, function (data) {
                    if (data.success) {
                        alertBox("Thông báo!", "Khôi phục thành công", true, 3000);
                    }
                    else {
                        alertBox("Báo lỗi", "Chưa khôi phục được", false, 3000);
                        console.log(data.message);
                    }
                    $(obj).attr('disabled', false);
                    $("#divLoading").hide();
                });
            }
            if (ButtonPressed === "Hủy") {
                return;
            }
        });
    }
    else {
        alertBox("Báo lỗi", "Chưa khôi phục được", false, 3000);
    }
}

//Role
function onSaveRole() {
    var text = $("#selectRole").val();
    if (text == null || text.length == 0) {
        text = "";
    }
    var id = $("#UserID").val();
    $("#loadingSaveRole").removeClass('hide');
    $("#btnSaveRole").attr('disabled', true);
    $.post(r + "/HOAdminAuthUser/AddUserToGroup", { id: id, data: text.toString() }, function (data) {
        if (data.success) {
            //selectedTabstrip(2);
            generateSelect2("selectRole");

            $("#grid").data("kendoGrid").dataSource.read();
            alertBox("Thành công! ","Lưu thành công", true, 3000);
        }
        else {
            alertBox("Báo lỗi! ", data.message, false, 3000);
            console.log(data.message);
        }
        $("#loadingSaveRole").addClass('hide');
        $("#btnSaveRole").attr('disabled', false);
    });
}
