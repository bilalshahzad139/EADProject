using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PMS.Entities;
using System.Data.SqlClient;

namespace PMS.DAL
{
    public static class ProductDAO
    {
        public static int Save(ProductDTO dto)
        {
            using (var helper = new DBHelper())
            {
                var sqlQuery = "";
                if (dto.ProductID > 0)
                {
                    sqlQuery =
                        $"Update dbo.Products Set Name='{dto.Name}',Price='{dto.Price}',PictureName='{dto.PictureName}',ModifiedOn='{dto.ModifiedOn}',ModifiedBy='{dto.ModifiedBy}' Where ProductID={dto.ProductID}";
                    helper.ExecuteQuery(sqlQuery);
                    return dto.ProductID;
                }
                else
                {
                    sqlQuery =
                        $"INSERT INTO dbo.Products(Name, Price, PictureName, CreatedOn, CreatedBy,IsActive,ProductCategoryID) VALUES('{dto.Name}','{dto.Price}','{dto.PictureName}','{dto.CreatedOn}','{dto.CreatedBy}',{1},'1'); Select @@IDENTITY";

                    var obj = helper.ExecuteScalar(sqlQuery);
                    return Convert.ToInt32(obj);
                }
            }
        }
        public static ProductDTO GetProductById(int pid)
        {
            var query = $"Select * from dbo.Products Where ProductId={pid}";

            using (var helper = new DBHelper())
            {
                var reader = helper.ExecuteReader(query);

                ProductDTO dto = null;

                if (reader.Read()) dto = FillDTO(reader);

                return dto;
            }
        }

        public static List<ProductDTO> GetAllProducts(bool pLoadComments=false)
        {
            const string query = "Select * from dbo.Products Where IsActive = 1;";

            using (var helper = new DBHelper())
            {
                var reader = helper.ExecuteReader(query);
                var list = new List<ProductDTO>();

                while (reader.Read())
                {
                    var dto = FillDTO(reader);
                    if (dto != null) list.Add(dto);
                }

                if (!pLoadComments) return list;
                //var commentsList = CommentDAO.GetAllComments();

                var commentsList = CommentDAO.GetTopComments(2);

                foreach (var prod in list)
                {
                    var prodComments = commentsList.Where(c => c.ProductID == prod.ProductID).ToList();
                    prod.Comments = prodComments;
                }
                return list;
            }
        }

        public static List<ProductDTO> GetPriceRangedProducts(int from, int to, bool pLoadComments = false)
        {
            var query = "Select * from dbo.Products Where IsActive = 1 AND (Price Between '"+from+"' AND '"+to+"');";
            using (var helper = new DBHelper())
            {
                var reader = helper.ExecuteReader(query);
                var list = new List<ProductDTO>();

                while (reader.Read())
                {
                    var dto = FillDTO(reader);
                    if (dto != null) list.Add(dto);
                }

                if (!pLoadComments) return list;
                //var commentsList = CommentDAO.GetAllComments();

                var commentsList = CommentDAO.GetTopComments(2);

                foreach (var prod in list)
                {
                    var prodComments = commentsList.Where(c => c.ProductID == prod.ProductID).ToList();
                    prod.Comments = prodComments;
                }
                return list;
            }
        }

        public static int DeleteProduct(int pid)
        {
            var sqlQuery = $"Update dbo.Products Set IsActive=0 Where ProductID={pid}";

            using (var helper = new DBHelper())
            {
                return helper.ExecuteQuery(sqlQuery);
            }
        }

        private static ProductDTO FillDTO(SqlDataReader reader)
        {
            var dto = new ProductDTO();
            dto.ProductID = reader.GetInt32(0);
            dto.Name = reader.GetString(1);
            dto.Price = reader.GetDouble(2);
            dto.PictureName = reader.GetString(3);
            dto.CreatedOn = reader.GetDateTime(4);
            dto.CreatedBy = reader.GetInt32(5);
            if (reader.GetValue(6) != DBNull.Value)
                dto.ModifiedOn = reader.GetDateTime(6);
            if (reader.GetValue(7) != DBNull.Value)
                dto.ModifiedBy = reader.GetInt32(7);

            dto.IsActive = reader.GetBoolean(8);
            return dto;
        }
    }
}
