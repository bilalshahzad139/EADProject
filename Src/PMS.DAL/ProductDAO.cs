using PMS.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

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
                        $"Update dbo.Products Set Name='{dto.Name}',Price='{dto.Price}',ModifiedOn='{dto.ModifiedOn}',ModifiedBy='{dto.ModifiedBy}' Where ProductID={dto.ProductID}; Update dbo.ProductPictureNames Set PictureName = '{dto.PictureName}' where ProductID = '{dto.ProductID}'";
                    helper.ExecuteQuery(sqlQuery);
                    return dto.ProductID;
                }
                else
                {
                    sqlQuery =
                        $"INSERT INTO dbo.Products(Name, Price, CreatedOn, CreatedBy,IsActive,ProductCategoryID) VALUES('{dto.Name}','{dto.Price}','{dto.CreatedOn}','{dto.CreatedBy}',{1},{dto.CategoryID}); Select @@IDENTITY";

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
            ;

            using (var helper = new DBHelper())
            {
                var reader = helper.ExecuteReader(query);

                ProductDTO dto = null;

                if (reader.Read()) dto = FillDTO(reader);

                return dto;
            }
        }

        public static List<ProductDTO> GetAllProducts(bool pLoadComments = false)
        {
            const string query = "Select a.ProductID, a.Name, a.Price, a.CreatedBy, a.CreatedOn, a.ModifiedBy, a.ModifiedOn, a.ProductCategoryID, a.IsActive, b.PictureName from dbo.Products a full outer join dbo.ProductPictureNames b on a.ProductID = b.ProductID where a.IsActive = 1;";

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
        public static List<ProductDTO> GetProductsByCategory(int categoryId, Boolean pLoadComments = false)
        {
            string query = "Select a.ProductID, a.Name, a.Price, a.CreatedBy, a.CreatedOn, a.ModifiedBy, a.ModifiedOn, a.ProductCategoryID, a.IsActive, b.PictureName from dbo.Products a full outer join dbo.ProductPictureNames b on a.ProductID = b.ProductID where a.IsActive = 1 and ProductCategoryId= " + categoryId + ";";

            // var query = String.Format("Select * from dbo.Products Where categoryID={0}", categoryId);
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
        public static List<ProductDTO> GetPriceRangedProducts(int from, int to, bool pLoadComments = false)
        {
            var query = "Select * from dbo.Products Where IsActive = 1 AND (Price Between '" + from + "' AND '" + to + "');";
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

        public static List<string> GetMatchingItems(string term)
        {
            List<string> matchingItems = null;
            using (var dbh = new DBHelper())
            {
                matchingItems = dbh.ExecuteStoredProcedure("GetMatchingItems", term);

            }
            return matchingItems;
        }
        public static int AddToWishlist(int uid, int pid)
        {
            String sqlQuery = String.Format("select IsInList from dbo.Wishlist where UserID={0} and ProductID={1}", uid, pid);
            using (DBHelper helper = new DBHelper())
            {
                int result;
                var res = helper.ExecuteScalar(sqlQuery);
                if (res == null)
                {
                    //if the product is not in wishlist then insert it
                    String insertQuery = String.Format("insert into dbo.Wishlist(UserID, ProductID, IsInList) VALUES('{0}','{1}','{2}')", uid, pid, 1);
                    result = helper.ExecuteQuery(insertQuery);
                    return result;
                }
                else
                {
                    //if the product is already in wishlist 
                    result = 0;
                    return result;
                }

            }
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
