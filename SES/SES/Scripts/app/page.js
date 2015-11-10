var idPopup;
var eventHotKey = false;

$(document).ready(function () {
    //trang chu
    $("#menu_Home").click(function () {        
        resetMenuHomePage();
        $(document).unbind("keypress");
        $(document).unbind("keydown");
        eventHotKey = false;
        onLoadPage(r + "/Home/Partial");
        hideLoading();
    });
    //Công ty
    $("#menu_Company").click(function () {
        onLoadPage(r + "/Company/PartialCompany");
    });
    //Nhân viên bán hàng
    $("#menu_SalesPerson").click(function () {
        onLoadPage(r + "/SalesPerson/PartialSalesPerson");
    });

    //phan quyen
    $("#menu_HOAdminAuthRole").click(function () {
        onLoadPage(r + "/HOAdminAuthRole/PartialRole");
    });
    //nguoi dung
    $("#menu_HOAdminAuthUser").click(function () {
        onLoadPage(r + "/HOAdminAuthUser/PartialUser");
    });
    //thong bao
    $("#menu_HOAdminAuthAnnouncement").click(function () {
        onLoadPage(r + "/HOAdminAuthAnnouncement/TreeView");
    });
    //location
    $("#menu_Location").click(function () {
        onLoadPage(r + "/Location/PartialLocation");
    });
    //Lịch Nghỉ
    $("#menu_AdminMasterHoliday").click(function () {
        onLoadPage(r + "/AdminMasterHoliday/TreeView");
    });
    //Lý do
    $("#menu_Reason").click(function () {
        onLoadPage(r + "/Reason/PartialReason");
    });
    // Phân cấp Vùng miền
    $("#menu_AdminMasterTerritory").click(function () {
        onLoadPage(r + "/AdminMasterTerritory/PartialOthersTerritory");
    });
    //$("#menu_AdminMasterTerritory").click(function () {
    //    onLoadPage(r + "/AdminMasterTerritory/IndexRegion");
    //});
    //$("#menu_AdminMasterTerritory").click(function () {
    //    onLoadPage(r + "/AdminMasterTerritory/IndexProvince");
    //});
    //$("#menu_AdminMasterTerritory").click(function () {
    //    onLoadPage(r + "/AdminMasterTerritory/IndexDistrict");
    //});
    //Kho ấn phẩm
    $("#menu_ListPublication").click(function () {
        onLoadPage(r + "/ListPublication/PartialRole");
    });
    //Kho và đơn vị tính
    $("#menu_WareHouse").click(function () {
        onLoadPage(r + "/WareHouse/PartialRole");
    });
    //Mu hàng
    $('#menu_SalesOrder').click(function () {
        onLoadPage(r + "/SalesOrder/PartialRole");
    })
    //Đơn vị vận chuyển
    $("#menu_Transporter").click(function () {
        onLoadPage(r + "/Transporter/PartialTransporter");
    });
    $("#menu_DeliveryFee").click(function () {
        onLoadPage(r + "/DeliveryFee/PartialDeliveryFee");
    });
    $("#menu_DeliveryDiscount").click(function () {
        onLoadPage(r + "/DeliveryDiscount/PartialDeliveryDiscount");
    });
    $("#menu_DeliveryPromotion").click(function () {
        onLoadPage(r + "/DeliveryPromotion/PartialDeliveryPromotion");
    });
    $("#menu_RuleCheck").click(function () {
        onLoadPage(r + "/RuleCheck/PartialRuleCheck");
    });
    $("#menu_Printer").click(function () {
        onLoadPage(r + "/Printer/PartialPrinter");
    });
    $("#menu_Contract").click(function () {
        onLoadPage(r + "/Contract/PartialContract");
    });
    $("#menu_Products").click(function () {
        onLoadPage(r + "/Products/PartialProducts");
    });
    $("#menu_DeliveryFormula").click(function () {
        onLoadPage(r + "/DeliveryFormula/PartialDeliveryFormula");
    });
    $("#menu_Customer").click(function () {
        onLoadPage(r + "/Customer/PartialCustomer");
    });
    $("#menu_Vendor").click(function () {
        onLoadPage(r + "/Vendor/PartialVendor");
    });
    $("#menu_Promotion").click(function () {
        onLoadPage(r + "/Vendor/Promotion");
    });
        var url = localStorage['urlpage'] || '';
        if (url) {
            onLoadPage(url);
        }
        else {
            onLoadPage(r + "/Home/Partial");
        }
});

    function onLoadPage(url) {
        $(document).unbind("keypress");
        $(document).unbind("keydown");
        eventHotKey = false;
        if (url != "" && url != null) {
            $.ajax({
                url: url,
                type: "get",
                beforeSend: function () { $("#divLoading").show(); },
                success: function (data) {
                
                    $("div.k-window").remove();
                    $("div#content").html(data);
                    localStorage['urlpage'] = url;
                    $("#divLoading").hide();
                },
           
                error: function (e) { }            
            });
        }
    }

    function resetMenu() {
        $("#menuLeft").find('li').removeClass('active').removeClass('open');
        //$("#menuLeft").find('ul').css('display', 'none');
        //$("#menuLeft").find('em').removeClass('fa-minus-square-o').addClass('fa-plus-square-o');
    }
    function resetMenuHomePage() {
        $("#menuLeft").find('li').removeClass('active').removeClass('open');
        $("#menuLeft").find('ul').css('display', 'none');
        $("#menuLeft").find('em').removeClass('fa-minus-square-o').addClass('fa-plus-square-o');
    }
    function hideMaskPopup(idMask) {
        eventHotKey = false;
        $(idMask).hide();
        $(idPopup).fadeOut();
    }