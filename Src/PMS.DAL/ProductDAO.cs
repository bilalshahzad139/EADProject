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
        public static int AddToCart(int pid, int uid)
        {
            var searchQuery = String.Format("select Quantity from dbo.Orders where UserID={0} and ProductID={1}", uid, pid);
            using (DBHelper helper = new DBHelper())
            {
                int res;
                var count = helper.ExecuteScalar(searchQuery);
                if (count == null)
                {
                    res = 0;
                }
                else
                {
                    res = (int)count;
                }
                if (res > 0)
                {
                    String updateQuery = String.Format("update dbo.Orders set Quantity=Quantity + 1");
                    var changed = helper.ExecuteQuery(updateQuery);
                    return changed;
                }
                else
                {
                    String insertQuery = String.Format("insert into dbo.Orders values({0},{1},{2})", uid, pid, 1);
                    var changed = helper.ExecuteQuery(insertQuery);
                    return changed;
                }
            }
        }
        public static List<PMS.Entities.Cart> getCart(int uid)
        {
            String searchQuery = String.Format("select Products.ProductID,Products.Name,Products.Price,ProductPictureNames.PictureName,Orders.Quantity from dbo.Products join dbo.Orders on Products.ProductID=Orders.ProductID join dbo.ProductPictureNames on Orders.ProductID=ProductPictureNames.ProductID where Orders.UserID={0}", uid);
            List<Cart> list = new List<Cart>();
            using (DBHelper helper = new DBHelper())
            {
                var orders = helper.ExecuteReader(searchQuery);
                while (orders.Read())
                {
                    Cart item = new Cart();
                    item.name = orders.GetString(orders.GetOrdinal("Name"));
                    item.pictureName = orders.GetString(orders.GetOrdinal("PictureName"));
                    item.pid = orders.GetInt32(orders.GetOrdinal("ProductID"));
                    item.price = (double)orders.GetDouble(orders.GetOrdinal("Price"));
                    item.quantity = orders.GetInt32(orders.GetOrdinal("Quantity"));
                    list.Add(item);
                }
                return list;
            }

        }
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
