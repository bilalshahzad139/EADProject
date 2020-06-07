using PMS.Entities;
using System;

namespace PMS.DAL
{
    public static class feedbackDAO
    {
        public static int saveFeedBack(feedbackDTO zoz)
        {
            String sqlQuery = "";

            sqlQuery = String.Format("INSERT INTO dbo.feedback(name,message) VALUES('{0}','{1}')",
                    zoz.name, zoz.message);

            using (DBHelper helper = new DBHelper())
            {
                return helper.ExecuteQuery(sqlQuery);
            }
        }
    }
}
