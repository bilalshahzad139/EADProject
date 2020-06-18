using PMS.Entities;

namespace PMS.BAL
{
    public static class feedbackBO
    {
        public static int saveFeedBack(string m,string n)
        {
            return DAL.feedbackDAO.saveFeedBack(m,n);
        }
    }
}
