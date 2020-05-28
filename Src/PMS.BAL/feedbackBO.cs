using PMS.Entities;

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
