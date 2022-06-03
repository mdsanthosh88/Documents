using ApplicationDashboardServiceCommon.Model;
using ApplicationDashboardServiceCommon.Result;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace ApplicationDashboardServiceRepository.DB
{
    public class SqlHelper
    {
        private readonly string connectionString;
        public SqlHelper(IConfiguration configuration)
        {
            connectionString = configuration.GetSection("ConnectionStrings").GetValue<string>("StudentDetailsDb");
        }

        public Result<SchemaTypes> GetAllApplication(int LOBID)
        {

            try
            {
                var SchemaTypes = new SchemaTypes();

                List<SchemaTypes> lstSchemaTypes = new List<SchemaTypes>();

                SqlParameter[] parameters = { new SqlParameter("@LOBID", LOBID) };
                var application = Helper.ExecuteReader<ApplicationModel>(connectionString, "Get_ApplicationName_SubCategories", CommandType.StoredProcedure, parameters);
                var properties = typeof(ApplicationModel).GetProperties();


                List<string> name = new List<string>();
                List<string> dataTypes = new List<string>();
                List<Array> a1 = new List<Array>();

                foreach (var prop in properties)
                {
                    name.Add(prop.Name);
                    dataTypes.Add(prop.PropertyType.Name);
                }

                SchemaTypes.Schema = typeof(ApplicationModel).Name;
                SchemaTypes.Columns = name;
                SchemaTypes.DataTypes = dataTypes;

                foreach (var item in application)
                {
                    string[] cars = { item.ApplicationName, item.ApplicationNameCount.ToString(), item.Requester, item.RequesterCount.ToString(), item.TicketPriority, item.ConfigurationItem };
                    a1.Add(cars);
                }

                SchemaTypes.Data = a1;
                return new Result<SchemaTypes>(true, SchemaTypes, "success");

            }
            catch (Exception ex)
            {
                return null;
                throw ex;
            }
        }

        public Result<List<LOBSubCategories>> GetLOBSubCategories(string ApplicationName, int LOBID)
        {

            try
            {
                var lOBSubCategories = Helper.ExecuteReader<LOBSubCategories>(connectionString, "select L.Name,acs.ApplicationName,acs.Configuration_Item,LOC.TicketPriority,LOC.TicketNo,LOC.TicketStatus,LOC.Description,LOC.Work_Notes, CASE  WHEN LOC.Requester ='Manager Alert'  THEN 1   WHEN LOC.Requester  LIKE 'Queiror Vanessa' THEN 2  ELSE 0 END as Requester,LOC.Createdby,Rtrim(FORMAT(LOC.CreatedDate,'dd/MM/yyyy hh:mm:ss')) as CreatedDate  from  [LOBSubCategories] LOC (nolock) inner join Application_CIs_Server acs (nolock) on Acs.Configuration_Item=LOC.Configuration_Item  inner join LOBInfo L (nolock) on L.Id=acs.LOBID where ACS.ApplicationName ='" + ApplicationName + "' and acs.LOBID=" + LOBID + " and LOc.IsActive=1  ", CommandType.Text);

                return new Result<List<LOBSubCategories>>(true, lOBSubCategories.ToList(), "success");

            }
            catch (Exception ex)
            {
                return new Result<List<LOBSubCategories>>(false, null, "faile");
                throw ex;
            }
        }



        public Result<List<Application_Status>> GetAllApplication_Total()
        {

            try
            {
                var application = Helper.ExecuteReader<Application_Status>(connectionString, "Get_ApplicationName_Total", CommandType.StoredProcedure);


                return new Result<List<Application_Status>>(true, application.ToList(), "success");

            }
            catch (Exception ex)
            {
                return new Result<List<Application_Status>>(false, null, "faile");
                throw ex;
            }
        }

        public Result<List<Application_Status>> GetApplicationName_Dashboard_Filter(int LOBID)
        {

            try
            {
                SqlParameter[] parameters = { new SqlParameter("@LOBID", LOBID) };
                var application = Helper.ExecuteReader<Application_Status>(connectionString, "Get_ApplicationName_Dashboard_Filter", CommandType.StoredProcedure, parameters);


                return new Result<List<Application_Status>>(true, application.ToList(), "success");

            }
            catch (Exception ex)
            {
                return new Result<List<Application_Status>>(false, null, "faile");
                throw ex;
            }
        }


        public Result<List<ApplicationModel>> GetApplicationName_Filter(int LOBID, int Requester)
        {
            try
            {
                SqlParameter[] parameters = { new SqlParameter("@LOBID", LOBID), new SqlParameter("@Requester", Requester) };
                var application = Helper.ExecuteReader<ApplicationModel>(connectionString, "Get_ApplicationName_Filter", CommandType.StoredProcedure, parameters);
                return new Result<List<ApplicationModel>>(true, application.ToList(), "success");

            }
            catch (Exception ex)
            {
                return new Result<List<ApplicationModel>>(false, null, "faile");
                throw ex;
            }
        }

        public Result<bool> DeleteDetails(int Id)
        {
            try
            {

                SqlParameter[] parameters = new SqlParameter[]
                {
                new SqlParameter() {ParameterName = "@Id", Value= Id},
                };
                Helper.ExecuteNonQuery(connectionString, "Delete from dbo.[Student] where Id = @Id", CommandType.Text, parameters);
                return new Result<bool>(true, true, "Sucess");
            }
            catch (Exception ex)
            {
                return new Result<bool>(false, false, "fail");
                throw ex;
            }

        }






        // UserInsert
        public List<User> GetUserName(string UserName)
        {
            List<User> users = new List<User>();
            try
            {
                return Helper.ExecuteReader<User>(connectionString, "SELECT [Id],[UserName],[Password],[EmailId],[IsActive] FROM dbo.[User] WHERE IsActive = 1 and [UserName]='" + UserName + "'", CommandType.Text);
            }
            catch (Exception ex)
            {
                return users;
                throw ex;
            }
        }
        public Result<List<User>> GetAll()
        {
            List<User> users = new List<User>();
            try
            {
                var list = Helper.ExecuteReader<User>(connectionString, "SELECT [Id],[UserName],[Password],[EmailId],[IsActive] FROM dbo.[User]", CommandType.Text).Select(x => new User
                {
                    UserName = x.UserName,
                    Id = x.Id,
                    EmailId = x.EmailId,
                    Password = CryptographyHelper.Decrypt(x.Password),
                    IsActive = x.IsActive,
                });
                return new Result<List<User>>(true, list.ToList(), "Success");

            }
            catch (Exception ex)
            {
                return new Result<List<User>>(false, users, "fail");
                throw ex;
            }
        }


        public Result<List<User>> GetById(int Id)
        {
            List<User> users = new List<User>();
            try
            {
                var list = Helper.ExecuteReader<User>(connectionString, "SELECT [Id],[UserName],[Password],[EmailId],[IsActive] FROM dbo.[User] WHERE [Id]='" + Id + "'", CommandType.Text).Select(x => new User
                {
                    UserName = x.UserName,
                    Id = x.Id,
                    EmailId = x.EmailId,
                    Password = CryptographyHelper.Decrypt(x.Password),
                    IsActive = x.IsActive,
                });
                return new Result<List<User>>(true, list.ToList(), "Success");

            }
            catch (Exception ex)
            {
                return new Result<List<User>>(false, users, "fail");
                throw ex;
            }
        }


        public Result<bool> UserInsert(User user)
        {
            try
            {

                string Sql = "INSERT INTO dbo.[User](UserName, Password, EmailId, IsActive)VALUES(@UserName, @Password, @EmailId, @IsActive)";
                string[] skipvalues = new string[] { "Id" };
                string[] Isencrypt = new string[] { "Password" };
                Helper.ExecuteNonQuery(connectionString, Sql, CommandType.Text, ReflectionPopulator<User>.Parameters(user, skipvalues, Isencrypt));

                return new Result<bool>(true, true, "Sucess");
            }
            catch (Exception ex)
            {
                return new Result<bool>(false, true, "fail");
                throw ex;
            }
        }
        public Result<bool> UserUpdate(User user)
        {
            try
            {

                string Sql = "Update  dbo.[User] Set UserName=@UserName, Password=@Password, EmailId=@EmailId, IsActive=@IsActive where Id=@Id";

                string[] Isencrypt = new string[] { "Password" };
                Helper.ExecuteNonQuery(connectionString, Sql, CommandType.Text, ReflectionPopulator<User>.Parameters(user, null, Isencrypt));

                return new Result<bool>(true, true, "Sucess");
            }
            catch (Exception ex)
            {
                return new Result<bool>(false, true, "fail");
                throw ex;
            }
        }

        public Result<bool> DeleteUsers(int Id)
        {
            try
            {

                SqlParameter[] parameters = new SqlParameter[]
                {
                new SqlParameter() {ParameterName = "@Id", Value= Id},
                };
                Helper.ExecuteNonQuery(connectionString, "Delete from dbo.[User] where Id = @Id", CommandType.Text, parameters);
                return new Result<bool>(true, true, "Sucess");
            }
            catch (Exception ex)
            {
                return new Result<bool>(false, false, "fail");
                throw ex;
            }

        }
    }

    public static class Helper
    {
        public static int ExecuteNonQuery(this string connectionString, string cmdText, CommandType type, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(cmdText, conn))
                {
                    cmd.CommandType = type;

                    if (parameters.Length != 0 && parameters != null)
                    {
                        foreach (SqlParameter param in parameters)
                        {
                            cmd.Parameters.Add(param);
                        }
                    }
                    conn.Open();
                    return cmd.ExecuteNonQuery();
                }
            }
        }


        public static List<T> ExecuteReader<T>(this string connectionString, string CommandName, CommandType type, params SqlParameter[] parameters)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(CommandName, connection))
                {
                    command.CommandTimeout = 1000;
                    command.CommandType = type;
                    if (parameters.Length != 0 && parameters != null)
                    {
                        foreach (SqlParameter param in parameters)
                        {
                            command.Parameters.Add(param);
                        }
                    }
                    command.Connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        return ReflectionPopulator<T>.CreateList(reader);
                    }



                }
            }
        }
    }
}
