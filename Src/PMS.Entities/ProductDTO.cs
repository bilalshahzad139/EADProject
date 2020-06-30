using System;
using System.Collections.Generic;

namespace PMS.Entities
{
    public class ProductDTO
    {
        public int ProductID { get; set; }
        public string Name { get; set; }

        public string ProductDescription { get; set; }
        public int Likes { get; set; }
        public int DisLikes { get; set; }


        public double Price { get; set; }
        public string PictureName { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        public int ModifiedBy { get; set; }
        public bool IsActive { get; set; }
        public int Quantity { get; set; }
        public int Sold { get; set; }
        public bool LowStockNotification { get; set; }
        public int CategoryID { get; set; }
        public int isOnSale { get; set; }
        public int percentageDiscount { get; set; }
        public string saleDescription { get; set; }

        public Boolean IsInWishlist { get; set; }


        public List<CommentDTO> Comments
        {
            get;
            set;
        }
    }
}
