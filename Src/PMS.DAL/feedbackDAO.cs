using PMS.Entities;
using System;

namespace PMS.DAL
{
    public static class feedbackDAO
    {
        public static int saveFeedBack(string ms,string ns)
        {
            String sqlQuery = "";

            sqlQuery = String.Format("INSERT INTO dbo.feedback(name,message) VALUES('{0}','{1}')",
                    ns, ms);

            using (DBHelper helper = new DBHelper())
            {
                return helper.ExecuteQuery(sqlQuery);
            }
        }
    }
}
