using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMS.BAL
{
    public class ProductSearchBO
    {
        public static List<ProductDTO> GetProductByName(ProductSearchDTO dt)
        {
            return DAL.ProductSearchDAO.GetProductByName(dt);
        }
    }
}
