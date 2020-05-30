using PMS.Entities;
using System.Collections.Generic;

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
