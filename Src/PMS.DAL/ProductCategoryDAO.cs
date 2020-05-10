using PMS.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMS.DAL
{
    public static class ProductCategoryDAO
    {
        public static int Save(String categoryName)
        {
            using (DBHelper helper = new DBHelper())
            {
                String sqlQuery = "";
               
                    sqlQuery = String.Format("INSERT INTO dbo.category(CategoryName) VALUES('{0}'); Select @@IDENTITY",
                        categoryName);

                    var obj = helper.ExecuteScalar(sqlQuery);
                    return Convert.ToInt32(obj);
                
            }
        }
        public static List<ProductCategoryDTO> GetAllCategories()
        {
            var query = String.Format("Select * from dbo.category");
            using (DBHelper helper = new DBHelper())
            {
                var reader = helper.ExecuteReader(query);
                List<ProductCategoryDTO> list = new List<ProductCategoryDTO>();

                while (reader.Read())
                {
                    var dto = FillDTO(reader);
                    if (dto != null)
                    {
                        list.Add(dto);
                    }
                }
                return list;
            }
           
        }
            private static ProductCategoryDTO FillDTO(SqlDataReader reader)
        {
            var dto = new ProductCategoryDTO();
            dto.CategoryID = reader.GetInt32(0);
           
            dto.CategoryName = reader.GetString(1);
            
            return dto;
        }
    }
}
