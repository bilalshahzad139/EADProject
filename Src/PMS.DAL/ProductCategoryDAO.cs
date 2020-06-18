using PMS.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace PMS.DAL
{
    public static class ProductCategoryDAO
    {
        public static int Save(String categoryName)
        {
            using (DBHelper helper = new DBHelper())
            {
                String sqlQuery = "";

                sqlQuery = String.Format("INSERT INTO dbo.ProductCategory(ProductCategoryName) VALUES('{0}'); Select @@IDENTITY",
                    categoryName);

                var obj = helper.ExecuteScalar(sqlQuery);
                return Convert.ToInt32(obj);

            }
        }
        public static List<ProductCategoryDTO> GetAllCategories()
        {
            var query = String.Format("Select * from dbo.ProductCategory");
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
        public static ProductCategoryDTO GetCategory(string Categoryname)
        {
            var query = String.Format("Select * from dbo.ProductCategory Where ProductCategoryName='{0}'", Categoryname);

            using (DBHelper helper = new DBHelper())
            {
                var reader = helper.ExecuteReader(query);

                ProductCategoryDTO dto = null;

                if (reader.Read())
                {
                    dto = FillDTO(reader);
                }

                return dto;
            }
        }
        private static ProductCategoryDTO FillDTO(SqlDataReader reader)
        {
            var dto = new ProductCategoryDTO();
            dto.ProductCategoryID = reader.GetInt32(0);

            dto.ProductCategoryName = reader.GetString(1);

            return dto;
        }
    }
}
