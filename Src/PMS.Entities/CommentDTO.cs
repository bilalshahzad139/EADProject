using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMS.Entities
{
    public class CommentDTO
    {
        public int CommentID { get; set; }
        public int UserID { get; set; }
        public int ProductID { get; set; }
        public string CommentText { get; set; }

        public DateTime CommentOn { get; set; }

        public string UserName { get; set; }

        public string PictureName { get; set; }

        public string CommentOnStr => CommentOn.ToString("dd-MM-yyyy HH:MM:ss");
    }
}
