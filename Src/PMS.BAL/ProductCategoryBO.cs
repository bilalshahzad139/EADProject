using PMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMS.BAL
{
    public class ProductCategoryBO
    {
        public static List<ProductCategoryDTO> GetAllCategories()
        {
            return PMS.DAL.ProductCategoryDAO.GetAllCategories();
        }
        public static int Save(String categoryName)
        {
            return PMS.DAL.ProductCategoryDAO.Save(categoryName);
        }
    }
}
