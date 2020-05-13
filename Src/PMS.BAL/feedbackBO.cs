using PMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMS.BAL
{
    public static class feedbackBO
    {
        public static int saveFeedBack(feedbackDTO xox)
        {
            return DAL.feedbackDAO.saveFeedBack(xox);
        }
    }
}
