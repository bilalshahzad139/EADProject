using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMS.Entities
{
    public class ProductSearchDTO
    {
        public String productName { get; set; }
        public float minPrice { get; set; }
        public float maxprice { get; set; }
        public int categoryId { get; set; }

    }
}
