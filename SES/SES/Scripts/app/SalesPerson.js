﻿var numHeight = 230;
var keyAction;
var indexTabstripActive = -1;
var contentTab;

$(document).ready(function () {
    //active menu
    resetMenu();
    $("#menu_SalesPerson").parent().addClass('active');

    document.title = "Nhân viên bán hàng";
    //fillter & form popup
    $("#selectStatus_search").select2();
    $("#s2id_selectStatus_search").css('width', '100%');

    $("#Status").select2();
    $("#s2id_Status").css('width', '240px');

    $("#Gender").select2();
    $("#s2id_Gender").css('width', '240px');
    //$("#TransporterID").select2();
    //$("#s2id_TransporterID").css('width', '240px');

    $(window).resize(function () {
        resizeGrid(numHeight);
    });

    $("#txtSalesPersonID").keypress(function (e) {
        if (e.keyCode == 13) {
            doSearch();
        }
    });


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
        width: "400px",
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
            SalesPersonID: {
                required: true,
                //alphanumeric: true
            },
            SalesPersonName: {
                required: true,
                //alphanumeric: true
            },

            Gender: {
                required: true,
                //alphanumeric: true
            },


            //DiscountPercent: {
            //    required: true,
            //    number: true,
            //    range: [0, 100]
            //},

        },

        // Messages for form validation
        messages: {
            SalesPersonID: {
                required: "Thông tin bắt buộc"
            },
            SalesPersonName: {
                required: "Thông tin bắt buộc"
            },
            Gender: {
                required: "Thông tin bắt buộc"
            },

            //DiscountPercent: {
            //    required: "Thông tin bắt buộc",
            //    number: "Phần trăm chiết khấu phải là số",
            //    range: "Cho phép nhập từ 0 đến 100"
            //},
            //TransporterID: {
            //    required: "Thông tin bắt buộc"
            //},
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
                            $("#formPopup").find('fieldset:eq(0) section:eq(0)').empty().append('<div style="float: left; width: 120px; margin-left:0;"><label class="label" style="float: right;">Mã nhân viên (*) </label></div><div style="float: left; width: 240px; margin-right: 0; margin-left: 10px;"><label style="float: right;text-align: left;width: 240px;height: 19px;padding-top: 2px;"><strong style="color:red;">' + data.SalesPersonID + '</strong><input type="hidden" id="SalesPersonID" name="SalesPersonID" value="' + data.SalesPersonID + '" /></label> <div style="clear:both"></div></div>');
                            $("#CreatedAt").val(dateToString(data.CreatedAt));
                            $("#CreatedBy").val(data.CreatedBy);
                            keyAction = -1;
                        }
                        //readHeaderInfo();
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
        $("#formPopup").find('fieldset:eq(0) section:eq(0)').empty().append('<div class="divlabel"><label class="label" style="float:right">Mã nhân viên (*)</label></div><div class="divfile"><label class="input" style="float:right;width:240px;"><input type="text" id="SalesPersonID" name="SalesPersonID" class="input-xs" placeholder="Mã nhân viên" style="margin-right:85px" /></label><div style="clear:both"></div></div>');
        $('#formPopup')[0].reset();
        $("#SalesPersonID").attr('readonly', false);
        $("#CreatedBy").val('');
        $("#CreatedAt").val('');
        $("#SalesPersonName").val('');
        //$("#TransporterID").val('');
        //$("#TransporterID").trigger("change");
        $("#Status option[value='True']").attr('selected', true);
        $("#Gender option[value='True']").attr('selected', true);
        $("#CompanyID option").removeAttr('selected');
        generateSelect2("CompanyID");
        setTimeout(function () {
            $("#SalesPersonID").focus();
        }, 500);
    }
        // Cập nhậpS
    else {
        keyAction = -1;
        popup.title('Chỉnh sửa');
        var id = $(obj).data('id');
        //$("#ContractID").attr('readonly', true);
        //$("#ContractID").css('color', 'red');
        $("#formPopup").find('fieldset:eq(0) section:eq(0)').empty().append('<div style="float: left; width: 120px; margin-left:0;"><label class="label" style="float: right;">Mã nhân viên (*)</label></div><div style="float: left; width: 240px; margin-right: 0; margin-left: 10px;"> <label style="float: right;text-align: left;width: 240px;height: 19px;padding-top: 2px;"><strong style="color:red;">' + id + '</strong><input type="hidden" id="SalesPersonID" name="SalesPersonID" value="' + id + '"/></label> <div style="clear:both"></div></di>');
        var currentRow = $(obj).closest("tr");
        var dataItem = $("#grid").data("kendoGrid").dataItem(currentRow);
        $.post(r + "/Conpany/GetCompanyByID", { id: id }, function (data) {
            if (data.success) {
                $("#CompanyID option:selected").removeAttr('selected');
                generateSelect2("CompanyID");
                for (var i = 0; i < data.listcompany.length; i++) {
                    $("#CompanyID option[value='" + data.listcompany[i].CompanyID + "']").attr('selected', true);
                }
                $("#CompanyID").trigger("change");
            }
        });
        onBindDataToFormSalesPerson(dataItem);
        var status = dataItem.Status == true ? "True" : "False";
        $('#Status').val(status);
        $('#Status').trigger('change');
        setTimeout(function () {
            $("#SalesPersonName").focus();
        }, 500);
        readHeaderInfo();
    }
}

// Load lại data khi tạo mới hoặc cập nhật
function onBindDataToFormSalesPerson(dataItem) {
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
    var grid = $("#grid").data("kendoGrid");
    var filter = { logic: "and", filters: [] };
    var text = $("#txtSalesPersonID").val();
    if (text) {
        var filterOr = { logic: "or", filters: [] };
        filterOr.filters.push({ field: "SalesPersonID", operator: "contains", value: text });
        filterOr.filters.push({ field: "SalesPersonName", operator: "contains", value: text });
        filter.filters.push(filterOr);
    }

    text = $("#selectStatus_search").val();
    if (text) {
        filter.filters.push({ field: "Status", operator: "eq", value: text });
    }

    grid.dataSource.filter(filter);
}

function readHeaderInfo() {
    contentTab = setContentTab(['SalesPersonID', 'SalesPersonName', 'CompanyID', 'Gender', 'Phone', 'Email', 'Address', 'Status'
    ], "30");
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