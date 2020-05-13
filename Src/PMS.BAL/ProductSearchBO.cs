using PMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMS.BAL
{
    public class ProductSearchBO
    {
        public static List<ProductDTO> GetProductByName(String prodName)
        {
            return DAL.ProductDAO.GetProductByName(prodName);
        }
    }
}
