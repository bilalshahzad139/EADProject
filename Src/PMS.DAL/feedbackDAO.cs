using PMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMS.DAL
{
    public static class feedbackDAO
    {
        public static int saveFeedBack(feedbackDTO zoz)
        {
            String sqlQuery = "";

            sqlQuery = String.Format("INSERT INTO dbo.feedback(name,email,pNo,message) VALUES('{0}','{1}','{2}','{3}')",
                    zoz.name, zoz.email, zoz.pNo, zoz.message);

            using (DBHelper helper = new DBHelper())
            {
                return helper.ExecuteQuery(sqlQuery);
            }
        }
    }
}
