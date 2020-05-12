using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMS.Entities
{
    public class ProductDTO
    {
        public int ProductID { get; set; }
        public string Name { get; set; }

        public double Price { get; set; }
        public string PictureName { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        public int ModifiedBy { get; set; }
        public bool IsActive { get; set; }

        public List<CommentDTO> Comments
        {
            get;
            set;
        }
    }
}
