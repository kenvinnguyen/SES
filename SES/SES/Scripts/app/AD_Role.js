var numHeight = 230;
var generateTreeList = false;
var menuIDSelected = "";
var indexTabstripActive = -1;
var selectedMenu = "";
var contentTab;
$(document).ready(function () {
    //active menu
    resetMenu();

    $("ul#menuLeft").find('#ul_root_1').addClass('open');
    $("ul#menuLeft").find('#ul_root_1').css('display', 'block');
    $("ul#menuLeft").find('#ul_root_1 ul#ul_item_2').css('display', 'block');
    $("#menu_AD_Role").parent().addClass('active');

    document.title = "Nhóm người dùng";
    //tabstrip
    //$("#tabstrip").kendoTabStrip({
    //    animation: { open: { effects: "fadeIn" } },
    //    activate: onActivateTabstrip,
    //});
    //fillter & form popup
    $("#selectIsActive_search").select2();
    $("#s2id_selectIsActive_search").css('width', '100%');

    $("#IsActive").select2();
    $("#s2id_IsActive").css('width', '240px');

    generateSelect2("selectUser");
    generateSelect2("selectAction");

    $(window).resize(function () {
        resizeGrid(numHeight);
    });

    //popup
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
    //form trong popup
    $("#formPopup").validate({
        // Rules for form validation
        rules: {
            RoleName: {
                required: true
            }
        },

        // Messages for form validation
        messages: {
            RoleName: {
                required: "Thông tin bắt buộc"
            }
        },

        // Do not change code below
        errorPlacement: function (error, element) {
            error.insertAfter(element);
        },

        submitHandler: function (form) {
            $(form).ajaxSubmit({
                //clearForm: true,//To clear form after ajax submitting
                beforeSend: function () {
                    $("#loading").removeClass('hide');
                    $("#formPopup").find('button').attr('disabled', true);
                },
                success: function (data) {
                    if (data.success) {
                        if (data.insert) {
                            $("#formPopup").find('fieldset:eq(0) section:eq(0)').empty().append('<div class="divlabel"><label class="label" style="float: right;">Mã nhóm (*) </label></div><div class="divfile"><label style="float: right;text-align: left;width: 240px;height: 19px;padding-top: 2px;"><strong style="color:red;">' + data.RoleID + '</strong><input type="hidden" id="RoleID" name="RoleID" value="' + data.RoleID + '"/></label> <div style="clear:both"></div></div>');
                            //$("#formPopup").find('fieldset:eq(0) section:eq(0)').empty().append('<label class="label" style="float: left;">Mã nhóm (*) </label><label style="float: right;text-align: left;width: 240px;height: 19px;padding-top: 2px;"><strong style="color:red;">' + data.RoleID + '</strong><input type="hidden" id="RoleID" name="RoleID" value="' + data.RoleID + '"/></label> <div style="clear:both"></div>');
                            $("#RowCreatedAt").val(dateToString(data.createdat));
                            $("#RowCreatedBy").val(data.createdby);
                        }
                        $("#RoleID").val(data.RoleID);
                        readHeaderInfo();
                        $("#grid").data("kendoGrid").dataSource.read();
                        $("#formPopup").find('button').attr('disabled', false);
                        showTabstrip();
                        alertBox("Thành công! ", "Lưu thành công", true, 3000);
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
    $("#txtRoleID").keypress(function (e) {
        if (e.keyCode == 13) {
            doSearch();
            return false;
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
                        //create at

                        var boolCreatedBy = $("#RowCreatedBy").val() != "system";
                        if (indexTabstripActive == 0 && $("#btnInsert").length > 0 && boolCreatedBy) {
                            $("#formPopup").submit();
                        }
                        else if (indexTabstripActive == 1 && $("#btnInsert").length > 0) {
                            onSaveUser();
                        }
                        break;
                }
            }
        }
    });

    //================================== Tab Phân quyền chức năng =====================================

    $("#selectAction").change(function () {
        var value = $(this).val();
        if (value) {
            $("#PermissionAction").show();
        }
        else {
            $("#PermissionAction").hide();
        }
        //alert(value);
        generateFuncTreeList(value);
    });
});

//========================================== code co ban ====================================

//search
function doSearch() {
    var grid = $("#grid").data("kendoGrid");
    var filter = { logic: "and", filters: [] };
    var text = $("#txtRoleID").val();
    if (text) {
        var filterOr = { logic: "or", filters: [] };
        if ($.isNumeric(text))
        {
            filterOr.filters.push({ field: "RoleID", operator: "eq", value: text });
        }
        filterOr.filters.push({ field: "RoleName", operator: "contains", value: text });
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
    changeStatusGrid('grid', 3);
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
        debugger;
        popup.title('Thêm');
        $("#RoleID").val('');
        $("#selectUser option:selected").removeAttr('selected');
        $("#treelist").hide();
        onRefreshPopup();
        setTimeout(function () {
            $("#RoleID").focus();
        }, 500);
       
    }
    else if (key == 1 || key == 2) {      // 1 : Update, 2 : Sao chép           
        var id = $(obj).data('id');
        $("#selectAction option").removeAttr('selected');

        generateSelect2("selectAction");
        if (key == 1) {
            popup.title('Chỉnh sửa');
            $("#RoleID").val(id);
            //$("#formPopup").find('fieldset:eq(0) section:eq(0)').empty().append('<label class="label">Mã nhóm: <strong style="color:red;margin-left: 52px;">' + id + '</strong></label>');
            $("#formPopup").find('fieldset:eq(0) section:eq(0)').empty().append('<div class="divlabel"><label class="label" style="float: right;">Mã nhóm (*) </label></div><div class="divfile"><label style="float: right;text-align: left;width: 240px;height: 19px;padding-top: 2px;"><strong style="color:red;">' + id + '</strong><input type="hidden" id="RoleID" name="RoleID" value="' + id + '"/></label> <div style="clear:both"></div></div>');
            var currentRow = $(obj).closest("tr");
            var dataItem = $("#grid").data("kendoGrid").dataItem(currentRow);
            //alert(dataItem.RoleID)
            generateFuncTreeList(dataItem.RoleID);
            onBindDataToForm(dataItem);
            //showTabstrip();
            showLoading();
            $.post(r + "/AD_Role/GetByID", { id: id }, function (data) {
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
                    // User of Role
                    $("#selectUser option:selected").removeAttr('selected');
                    for (var i = 0; i < data.listuser.length; i++) {
                        $("#selectUser option[value='" + data.listuser[i].UserID + "']").attr('selected', true);
                    }

                    generateSelect2("selectUser");
                    readHeaderInfo();

                }
                else {
                    alertBox("Báo lỗi", "", false, 3000);
                    console.log(data.message);
                }
                hideLoading();
            });
        }
        else if (key == 2) {
            popup.title('Sao chép');
            $("#IsCopy").val('1');      // Value = 1 gửi lên Server để biết là sao chép
            $("#RoleID").val(id);
            $("#formPopup").find('fieldset:eq(0) section:eq(0)').empty();
            onRefreshPopup();
            $("#selectUser option:selected").removeAttr('selected');
            generateSelect2("selectUser");
            setTimeout(function () {
                $("#tabstrip").find('div.k-content:eq(0)').css('height', '100%');
            }, 1000);
        }
    }
    setTimeout(function () {
        $("#RoleName").focus();
    }, 500);
}

function readHeaderInfo() {

    contentTab = setContentTab(["RoleID", "RoleName", "IsActive"], "30");

}

function onRefreshPopup() {
    $("#formPopup").find('fieldset:eq(0) section:eq(0)').empty();
    $("#RoleName").val('');
    $("#IsActive option:selected").removeAttr('selected');
    $("#IsActive").select2();
    $("#s2id_IsActive").css('width', '240px');
    $("#Note").val('');
    $("#RowCreatedAt").val('');
    $("#RowCreatedBy").val('');
    $("#divGridAction").empty;
    $('#btnSubmit').css({ 'display': 'block' });
    generateSelect2("selectUser");
    hideTabStrip();
}

//tabstrip
//function onActivateTabstrip(e) {
//    var index = $(e.item).index();
//    indexTabstripActive = index;
//    if (index == 2) {
//        $("#divGridAction").empty();
//        if (!generateTreeList) {
//            generateFuncTreeList("");
//        }

//    }
//    $("#popup").closest(".k-window").css('top', $('#header').height());

//    if (index != 0) {
//        var currentTab = e.contentElement.id;
//        getContentTab(currentTab, contentTab);
//    }

//    setTimeout(function () {
//        $("#tabstrip").find('div.k-content:eq(' + index + ')').css('height', '100%');
//    }, 1000);

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

function showTabstrip() {
    $("#tabstrip").find('ul li:not(:first)').show();
}

function hideTabStrip() {
    $("#tabstrip").find('ul li:not(:first)').hide();
    setTimeout(function () {
        $("#tabstrip").find('div.k-content:eq(0)').css('height', '100%');
    }, 1000);
}

//========================================== Tab Người dùng ==================================

function onSaveUser() {
    var text = $("#selectUser").val();
    if (text == null || text.length == 0) {
        text = "";
    }
    var id = $("#RoleID").val();
    if (typeof id == 'undefined') {
        alertBox("Báo lỗi", "Bạn chưa thêm nhóm quyền!!", false, 3000);
        return;
    }
    $("#loadingSaveUser").removeClass('hide');
    $("#btnSaveUser").attr('disabled', true);
    $.post(r + "/AD_Role/AddUserToGroup", { id: id, data: text.toString() }, function (data) {
        if (data.success) {
            generateSelect2("selectUser");
            setTimeout(function () {
                generateFuncTreeList("");
            }, 500);
            $("#grid").data("kendoGrid").dataSource.read();
            alertBox("Lưu thành công", "", true, 3000);
        }
        else {
            alertBox("Báo lỗi", "Chưa lưu được", false, 3000);
            console.log(data.message);
        }
        $("#loadingSaveUser").addClass('hide');
        $("#btnSaveUser").attr('disabled', false);
    });
}

//========================================== Tab Phân quyền chức năng ========================

function generateFuncTreeList(action) {
    debugger;
    if (action == null || action == 'undefined' || action =="") {
        action = "";
        $("#treelist").hide();
        return;
    }
    var id = $("#RoleID").val();
    if (typeof id == 'undefined') {
        return;
    }
    $("#treelist").show();
    showLoading();
    $.post(r + "/AD_Role/GetMenu", { action: action, roleID: $("#RoleID").val() }, function (data) {
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
            $("#tabstrip").css('cursor', 'pointer');

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

function onLoadGridAction() {
    var obj = { "RoleID": $("#RoleID").val(), "MenuID": menuIDSelected };
    var detailTemplate = kendo.template($('#templateGridAction').html());
    $("#divGridAction").html(detailTemplate(obj));
    loadToolbarStyle();
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

function onRequestStartGridAction(e) {
    $("#divLoading").show();
}

function onRequestEndGridAction(e) {
    if (e.type == "create" || e.type == "update" || e.type == "delete") {
        if (e.response.Errors == null) {
            onLoadGridAction();
            alertBox("Thành công", "", true, 3000);
        }
        else {
            alertBox("Báo lỗi", e.response.Errors.er.errors[0], false, 3000);
        }
    }
}

function onDataboundGridAction(e) {
    $("#divLoading").hide();
}

function onEditGridAction(e) {
    if (e.model.isNew() == false) {
        $('input[name=Action]').parent().html(e.model.Action);
    }
}

function SavePermission(obj) {
    var id = $("#RoleID").val();
    if (typeof id == 'undefined') {
        alertBox("Báo lỗi", "Chưa thêm nhóm quyền!!", false, 3000);
        return;
    }
    $("#loadingSavePermission").removeClass('hide');
    $(obj).attr('disabled', true);
    $.post(r + "/AD_Role/SavePermission", { RoleID: $("#RoleID").val(), Action: $("#selectAction").val(), MenuIDs: selectedMenu }, function (data) {
        if (data.success) {
            alertBox("Lưu thành công", "", true, 3000);
        }
        else {
            alertBox("Báo lỗi", data.message, false, 3000);
            console.log(data.message);
        }
        $("#loadingSavePermission").addClass('hide');
        $(obj).attr('disabled', false);
    });
}

//========================================== code khac neu co ====================================
