using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace SES.Service
{
    public class SqlHelper
    {
        string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public SqlConnection Connection()
        {
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            return con;
        }

        public DataTable SelectQuery(string strSQL)
        {
            DataTable dt = new DataTable();
            SqlConnection cn = new SqlConnection(connectionString);
            try
            {
                SqlCommand cmd = new SqlCommand(strSQL, cn);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            cn.Close();
            return dt;
        }

        public DataTable ExecuteQuery(string spName, List<SqlParameter> listpara)
        {
            DataTable dt = new DataTable();
            SqlConnection con = Connection();
            try
            {                
                SqlCommand command = con.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = spName;
                if (spName != null)
                {
                    foreach (SqlParameter para in listpara)
                    {
                        command.Parameters.Add(para);
                    }
                }
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                adapter.Fill(dt);                
            }
            catch (Exception ex)
            {
                throw ex;
            }
            con.Close();
            return dt;
        }

        public int ExecuteNoneQuery(string spName, List<SqlParameter> listpara)
        {
            int n = -1;
            SqlConnection con = Connection();
            try
            {                
                SqlCommand command = new SqlCommand(spName, con);
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 0;
                if (listpara != null)
                {
                    foreach (SqlParameter para in listpara)
                        command.Parameters.Add(para);
                }
                n = command.ExecuteNonQuery();                
            }
            catch (Exception ex)
            {
                throw ex;
            }
            con.Close();
            return n;
        }

        public DataSet ExcuteQueryDataSet(string sp, List<SqlParameter> listpara)
        {
            DataSet dts = new DataSet();
            SqlConnection con = Connection();
            try
            {                
                SqlCommand cmd = new SqlCommand(sp, con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                if (listpara != null)
                {
                    foreach (SqlParameter para in listpara)
                        cmd.Parameters.Add(para);
                }
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dts);                
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            con.Close();
            return dts;
        }

        public static string convertToUnSign3(string s)
        {
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = s.Normalize(NormalizationForm.FormD);
            string tmp = regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
            return Regex.Replace(tmp, "[^a-zA-Z0-9_ -]+", "", RegexOptions.Compiled).Replace(",", "").Replace(" ", "-").Replace("--", "-").ToLower();
        }

        public static string GetMd5Hash(string value)
        {
            var md5Hasher = MD5.Create();
            var data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(value));
            var sBuilder = new StringBuilder();
            for (var i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }
    }
}