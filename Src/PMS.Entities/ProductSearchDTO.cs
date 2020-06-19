using System;

namespace PMS.Entities
{
    public class ProductSearchDTO
    {
        public String productName { get; set; }
        public float minPrice { get; set; }
        public float maxPrice { get; set; }
        public int categoryId { get; set; }

    }
}
