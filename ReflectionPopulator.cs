using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ApplicationDashboardServiceRepository.DB
{
    public class ReflectionPopulator<T>
    {

        public static List<T> CreateList(SqlDataReader reader)
        {
            var results = new List<T>();
            var properties = typeof(T).GetProperties();

            while (reader.Read())
            {
                var item = Activator.CreateInstance<T>();
                foreach (var property in typeof(T).GetProperties())
                {
                    if (!reader.IsDBNull(reader.GetOrdinal(property.Name)))
                    {
                        Type convertTo = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                        property.SetValue(item, Convert.ChangeType(reader[property.Name], convertTo), null);
                    }
                }
                results.Add(item);

            }
            return results;
        }


        public static SqlParameter[] Parameters(object parameters, string[] skipvalues = null, string[] IsEncrypt = null)
        {
            var parameter = new List<SqlParameter>();
            foreach (PropertyInfo pi in parameters.GetType().GetProperties())
            {

                if ((skipvalues == null ? 0 : skipvalues.Where(x => x.Equals(pi.Name.ToString())).Count()).Equals(0))
                {
                    if ((IsEncrypt == null ? 0 : IsEncrypt.Where(x => x.Equals(pi.Name.ToString())).Count()).Equals(1))
                    {
                        parameter.Add(new SqlParameter("@" + pi.Name, CryptographyHelper.Encrypt(Convert.ToString(pi.GetValue(parameters, null)))));
                    }
                    else
                    {
                        parameter.Add(new SqlParameter("@" + pi.Name, pi.GetValue(parameters, null).ToString()));
                    }
                }
            }

            return parameter.ToArray();

        }
    }
}
