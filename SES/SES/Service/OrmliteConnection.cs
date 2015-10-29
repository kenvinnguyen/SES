using ServiceStack.OrmLite;
using ServiceStack.OrmLite.SqlServer;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;

namespace SES.Service
{
    public class OrmliteConnection
    {
        public IDbConnection openConn()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;            
            var dbFactory = new OrmLiteConnectionFactory(connectionString,SqlServerOrmLiteDialectProvider.Instance);
            IDbConnection dbConn = dbFactory.OpenDbConnection();
            OrmLiteConfig.DialectProvider.UseUnicode = true;
            return dbConn;
        }        
    }
}