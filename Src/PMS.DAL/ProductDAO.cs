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
            using (DBHelper helper = new DBHelper())
            {
                String sqlQuery = "";
                if (dto.ProductID > 0)
                {
                    sqlQuery =
                        $"Update dbo.Products Set Name='{dto.Name}',Price='{dto.Price}',ModifiedOn='{dto.ModifiedOn}',ModifiedBy='{dto.ModifiedBy}' Where ProductID={dto.ProductID}; Update dbo.ProductPictureNames Set PictureName = '{dto.PictureName}' where ProductID = '{dto.ProductID}'";
                    helper.ExecuteQuery(sqlQuery);
                    return dto.ProductID;
                }
                else
                {
                    sqlQuery =
                        $"INSERT INTO dbo.Products(Name, Price, CreatedOn, CreatedBy,IsActive,ProductCategoryID) VALUES('{dto.Name}','{dto.Price}','{dto.CreatedOn}','{dto.CreatedBy}',{1},'1'); Select @@IDENTITY";

                    var obj = helper.ExecuteScalar(sqlQuery);
                    sqlQuery =
                        $"INSERT INTO dbo.ProductPictureNames(PictureName, ProductID ) Values ('{dto.PictureName}','{Convert.ToInt32(obj)}');";
                    helper.ExecuteQuery(sqlQuery);
                    return Convert.ToInt32(obj);
                }
            }
        }
        public static ProductDTO GetProductById(int pid)
        {
            var query = $"Select a.ProductID, a.Name, a.Price, a.CreatedBy, a.CreatedOn, a.ModifiedBy, a.ModifiedOn, a.ProductCategoryID, a.IsActive, b.PictureName from dbo.Products a full outer join dbo.ProductPictureNames b on a.ProductID = b.ProductID where a.ProductID='{pid}'";
            using (DBHelper helper = new DBHelper())
            {
                var reader = helper.ExecuteReader(query);

                ProductDTO dto = null;

                if (reader.Read())
                {
                    dto = FillDTO(reader);
                }

                return dto;
            }
        }

        public static List<ProductDTO> GetAllProducts(Boolean pLoadComments=false)
        {
            const string query = "Select a.ProductID, a.Name, a.Price, a.CreatedBy, a.CreatedOn, a.ModifiedBy, a.ModifiedOn, a.ProductCategoryID, a.IsActive, b.PictureName from dbo.Products a full outer join dbo.ProductPictureNames b on a.ProductID = b.ProductID where a.IsActive = 1;";
            using (DBHelper helper = new DBHelper())
            {
                var reader = helper.ExecuteReader(query);
                List<ProductDTO> list = new List<ProductDTO>();

                while (reader.Read())
                {
                    var dto = FillDTO(reader);
                    if (dto != null)
                    {
                        list.Add(dto);
                    }
                }
                if (pLoadComments == true)
                {
                    //var commentsList = CommentDAO.GetAllComments();

                    var commentsList = CommentDAO.GetTopComments(2);

                    foreach (var prod in list)
                    {
                        List<CommentDTO> prodComments = commentsList.Where(c => c.ProductID == prod.ProductID).ToList();
                        prod.Comments = prodComments;
                    }
                }
                return list;
            }
        }

        public static List<ProductDTO> GetPriceRangedProducts(int from, int to, Boolean pLoadComments = false)
        {
            var query = "Select * from dbo.Products Where IsActive = 1 AND (Price Between '"+from+"' AND '"+to+"');";
            using (DBHelper helper = new DBHelper())
            {
                var reader = helper.ExecuteReader(query);
                List<ProductDTO> list = new List<ProductDTO>();

                while (reader.Read())
                {
                    var dto = FillDTO(reader);
                    if (dto != null)
                    {
                        list.Add(dto);
                    }
                }
                if (pLoadComments == true)
                {
                    //var commentsList = CommentDAO.GetAllComments();

                    var commentsList = CommentDAO.GetTopComments(2);

                    foreach (var prod in list)
                    {
                        List<CommentDTO> prodComments = commentsList.Where(c => c.ProductID == prod.ProductID).ToList();
                        prod.Comments = prodComments;
                    }
                }
                return list;
            }
        }

        public static int DeleteProduct(int pid)
        {
            String sqlQuery = String.Format("Update dbo.Products Set IsActive=0 Where ProductID={0}", pid);

            using (DBHelper helper = new DBHelper())
            {
                return helper.ExecuteQuery(sqlQuery);
            }
        }

        public static List<string> GetMatchingItems(string term)
        {
            List<string> matchingItems = null;
            using (var dbh = new DBHelper())
            {
                matchingItems = dbh.ExecuteStoredProcedure("GetMatchingItems", term);

            }
            return matchingItems;
        }

        private static ProductDTO FillDTO(SqlDataReader reader)
        {
            var dto = new ProductDTO();
            dto.ProductID = reader.GetInt32(reader.GetOrdinal("ProductID"));
            dto.Name = reader.GetString(reader.GetOrdinal("Name"));
            dto.Price = reader.GetDouble(reader.GetOrdinal("Price"));
            dto.PictureName = reader.GetString(reader.GetOrdinal("PictureName"));
            dto.CreatedOn = reader.GetDateTime(reader.GetOrdinal("CreatedOn"));
            dto.CreatedBy = reader.GetInt32(reader.GetOrdinal("CreatedBy"));
            if (reader.GetValue(reader.GetOrdinal("ModifiedOn")) != DBNull.Value)
                dto.ModifiedOn = reader.GetDateTime(reader.GetOrdinal("ModifiedOn"));
            if (reader.GetValue(reader.GetOrdinal("ModifiedBy")) != DBNull.Value)
                dto.ModifiedBy = reader.GetInt32(reader.GetOrdinal("ModifiedBy"));

            dto.IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"));
            return dto;
        }
    }
}
