﻿using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SES.Service;
using SES.Models;
using System.Text.RegularExpressions;
using OfficeOpenXml;
using System.IO;
using System.Collections;
using System.Configuration;
using log4net;
using System.Data;
using ServiceStack.OrmLite;
using ServiceStack.OrmLite.SqlServer;
using System.Data.SqlClient;
using Dapper;
namespace SES.Controllers
{
    public class SalesPersonController : CustomController
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        //
        // GET: /SalesPerson/
        public ActionResult PartialSalesPerson()
        {
            if (userAsset.ContainsKey("View") && userAsset["View"])
            {
                IDbConnection dbConn = new OrmliteConnection().openConn();
                var dict = new Dictionary<string, object>();
                dict["asset"] = userAsset;
                dict["activestatus"] = new CommonLib().GetActiveStatus();
                dict["listSalesPerson"] = dbConn.Select<SalesPerson>(p => p.Status == true);
                dbConn.Close();
                return PartialView("_SalesPerson", dict);
            }
            else
                return RedirectToAction("NoAccess", "Error");
        }

        public ActionResult Read([DataSourceRequest]DataSourceRequest request)
        {
            var dbConn = new OrmliteConnection().openConn();
            string whereCondition = "";
            if (request.Filters.Count > 0)
            {
                whereCondition = " AND " + new KendoApplyFilter().ApplyFilter(request.Filters[0]);
            }
            var data = new SalesPerson().GetList(request.Page, request.PageSize, whereCondition).ToList();
            return Json(data.ToDataSourceResult(request));
        }
	}
}