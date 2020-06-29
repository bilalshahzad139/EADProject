using PMS.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;

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
                        $"Update dbo.Products Set Name='{dto.Name}',Price='{dto.Price}',ModifiedOn='{dto.ModifiedOn}',ModifiedBy='{dto.ModifiedBy}',Description='{dto.ProductDescription}' Where ProductID={dto.ProductID}; Update dbo.ProductPictureNames Set PictureName = '{dto.PictureName}' where ProductID = '{dto.ProductID}'";
                    helper.ExecuteQuery(sqlQuery);
                    return dto.ProductID;
                }
                else
                {
                    sqlQuery = $"INSERT INTO dbo.Products(Name, Price, CreatedOn, CreatedBy,IsActive,ProductCategoryID,Quantity,Sold,Description)" +
                        $" VALUES('{dto.Name}','{dto.Price}','{dto.CreatedOn.ToString("MM/dd/yyyy HH:MM")}','{dto.CreatedBy}',{1},'{dto.CategoryID}','{dto.Quantity}','{dto.Sold}','{dto.ProductDescription}'); Select @@IDENTITY";
                    var ali = 123;
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
            var query = $"Select a.ProductID, a.Name, a.Price, a.CreatedBy, a.CreatedOn, a.ModifiedBy, a.ModifiedOn, a.ProductCategoryID, a.IsActive,a.Description, b.PictureName, cast((((Quantity-Sold)/(Quantity+0.0)))*100.0 as float) LowStockNotification from dbo.Products a full outer join dbo.ProductPictureNames b on a.ProductID = b.ProductID where a.ProductID='{pid}'";
            ;

            using (var helper = new DBHelper())
            {
                var reader = helper.ExecuteReader(query);

                ProductDTO dto = null;

                if (reader.Read()) dto = FillDTO(reader);

                return dto;
            }
        }
        public static int AddLikesAndGetCount(int ProductID, int UserID)
        {
            var query = $"Select a.likes from dbo.LikesDislikes a  join dbo.Products b on a.ProductID = b.ProductID join  dbo.Users c on a.UserID=c.UserID where a.ProductID='{ProductID}' and a.UserID='{UserID}'";
            var sqlQuery = "";

            using (var helper = new DBHelper())
            {
                var reader = helper.ExecuteReader(query);
                if (reader.Read())
                {
                    int res;
                    if (reader.GetInt32(reader.GetOrdinal("Likes")) == 1)
                        res = 0;
                    else
                        res = 1;
                    sqlQuery= $"Update  dbo.LikesDislikes set Likes={res} where UserID={UserID} and ProductID={ProductID}";
                }
                else
                {
                    sqlQuery = $"INSERT INTO dbo.LikesDislikes(UserID,ProductID,Likes) Values ('{UserID}','{ProductID}','1')";

                }
            }
            using (var helpr = new DBHelper())
            {
                helpr.ExecuteQuery(sqlQuery);
            }

            return getLikesCount(ProductID);


        }

        public static int AddDisLikesAndGetCount(int ProductID, int UserID)
        {
            var query = $"Select a.Dislikes from dbo.LikesDislikes a  join dbo.Products b on a.ProductID = b.ProductID join  dbo.Users c on a.UserID=c.UserID where a.ProductID='{ProductID}' and a.UserID='{UserID}'";
            var sqlQuery = "";

            using (var helper = new DBHelper())
            {
                var reader = helper.ExecuteReader(query);
                if (reader.Read())
                {
                    int res;
                    if (reader.GetInt32(reader.GetOrdinal("Dislikes")) == 1)
                        res = 0;
                    else
                        res = 1;
                    sqlQuery = $"Update  dbo.LikesDislikes set Dislikes={res} where UserID={UserID} and ProductID={ProductID}";
                }
                else
                {
                    sqlQuery = $"INSERT INTO dbo.LikesDislikes(UserID,ProductID,Dislikes) Values ('{UserID}','{ProductID}','1')";

                }
            }
            using (var helpr = new DBHelper())
            {
                helpr.ExecuteQuery(sqlQuery);
            }

            return getDisLikesCount(ProductID);

        }

        public static List<ProductDTO> GetAllProducts(bool pLoadComments = false)
        {
            const string query = "Select a.ProductID, a.Name, a.Price, a.CreatedBy, a.CreatedOn, a.ModifiedBy, a.ModifiedOn, a.ProductCategoryID, a.IsActive,a.Description, b.PictureName, a.Quantity, a.Sold, cast((((Quantity-Sold)/(Quantity+0.0)))*100.0 as float) LowStockNotification from dbo.Products a full outer join dbo.ProductPictureNames b on a.ProductID = b.ProductID where a.IsActive = 1 ;";
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
            string query = "Select a.ProductID, a.Name, a.Price, a.CreatedBy, a.CreatedOn, a.ModifiedBy, a.ModifiedOn,a.Description, a.ProductCategoryID, a.IsActive, b.PictureName from dbo.Products a full outer join dbo.ProductPictureNames b on a.ProductID = b.ProductID where a.IsActive = 1 and ProductCategoryId= " + categoryId + ";";

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
            var query = "Select a.ProductID, a.Name, a.Price, a.CreatedBy, a.CreatedOn, a.ModifiedBy, a.ModifiedOn, a.ProductCategoryID, a.IsActive, b.PictureName, a.Quantity,a.Description, a.Sold, cast((((Quantity-Sold)/(Quantity+0.0)))*100.0 as float) LowStockNotification from dbo.Products a full outer join dbo.ProductPictureNames b on a.ProductID = b.ProductID Where a.IsActive = 1 AND (a.Price Between '" + from + "' AND '" + to + "');";
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
        public static List<ProductDTO> GetLatestProducts(bool pLoadComments = false)
        {
            const string query = "Select a.ProductID, a.Name, a.Price, a.CreatedBy, a.CreatedOn, a.ModifiedBy, a.ModifiedOn, a.ProductCategoryID, a.IsActive,  a.Quantity, a.Sold,a.Description, b.PictureName, cast((((Quantity-Sold)/(Quantity+0.0)))*100.0 as float) LowStockNotification from dbo.Products a full outer join dbo.ProductPictureNames b on a.ProductID = b.ProductID where a.IsActive = 1 Order BY CreatedOn desc;";

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
        public static List<ProductDTO> getTrendingProducts(bool pLoadComments = false)
        {
            const string query = "Select a.ProductID, a.Name, a.Price, a.CreatedBy, a.CreatedOn, a.ModifiedBy, a.ModifiedOn, a.ProductCategoryID, a.IsActive,  a.Quantity, a.Sold,a.Description, (a.Quantity-a.Sold) remaining, b.PictureName, cast((((Quantity-Sold)/(Quantity+0.0)))*100.0 as float) LowStockNotification from dbo.Products a full outer join dbo.ProductPictureNames b on a.ProductID = b.ProductID where a.IsActive = 1 Order BY remaining;";

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
        private static Boolean isLowStock(Double stockPercent)
        {
            if (stockPercent < 50)
                return true;
            return false;
        }
        public static List<string> GetRelatedFAQ(string text)
        {
            var s = GetSearchWords(text);
            var condition = getCondition(s);
            List<string> faq = new List<string>();
            using (var helper = new DBHelper())
            {
                string query = "Select Question, Answer from dbo.FrequentlyAskedQuestion  where " + condition;
                var reader = helper.ExecuteReader(query);
                while (reader.Read())
                {
                    faq.Add(reader.GetString(reader.GetOrdinal("Question")));
                    faq.Add(reader.GetString(reader.GetOrdinal("Answer")));
                }
            }
            return faq;
        }
        public static string getCondition(string[] keywords)
        {
            var condition = "Question like '% " + keywords[0] + "%'";
            for (int i = 1; i < keywords.Length; i++)
            {
                if (!keywords[i].Equals(' ') || keywords[i] != null)
                    condition = condition + "or Question like '%" + keywords[i] + "%'";
            }
            return condition;
        }
        public static string[] GetSearchWords(string text)
        {
            string pattern = @"\S+";
            Regex re = new Regex(pattern);
            int c = 0;
            MatchCollection matches = re.Matches(text);
            for (int i = 0; i < matches.Count; i++)
            {
                if ((matches[i].Value).ToLower() != "how" && (matches[i].Value).ToLower() != "what" && (matches[i].Value).ToLower() != "when" && (matches[i].Value).ToLower() != "where" && (matches[i].Value).ToLower() != "i" && (matches[i].Value).ToLower() != "my" && (matches[i].Value).ToLower() != "me" && (matches[i].Value).ToLower() != "the" && (matches[i].Value).ToLower() != "is" && (matches[i].Value).ToLower() != "do")
                    c++;
            }
            string[] words = new string[c];
            int j = 0;
            for (int i = 0; i < matches.Count; i++)
            {
                if ((matches[i].Value).ToLower() != "how" && (matches[i].Value).ToLower() != "what" && (matches[i].Value).ToLower() != "when" && (matches[i].Value).ToLower() != "where" && (matches[i].Value).ToLower() != "i" && (matches[i].Value).ToLower() != "my" && (matches[i].Value).ToLower() != "me" && (matches[i].Value).ToLower() != "the" && (matches[i].Value).ToLower() != "is" && (matches[i].Value).ToLower() != "do")
                {
                    words[j] = matches[i].Value.ToLower();
                    j++;
                }
            }
            return words;
        }

        private static int getLikesCount(int ProductID)
        {
            using (var helpr = new DBHelper())
            {
                var sqlQuery = $"Select Count(Likes) from dbo.LikesDislikes where Likes=1 and ProductID={ProductID}";
                int recordCount = (int)(helpr.ExecuteScalar(sqlQuery));
                return recordCount;
            }

        }

        private static int getDisLikesCount(int ProductID)
        {
            using (var helpr = new DBHelper())
            {
                var sqlQuery = $"Select Count(Dislikes) from dbo.LikesDislikes where Dislikes=1 and ProductID={ProductID}";
                int recordCount = (int)(helpr.ExecuteScalar(sqlQuery));
                return recordCount;
            }

        }

        private static ProductDTO FillDTO(SqlDataReader reader)
        {
            var dto = new ProductDTO();
            dto.ProductID = reader.GetInt32(reader.GetOrdinal("ProductID"));
            dto.Name = reader.GetString(reader.GetOrdinal("Name"));
            var isNoDescription = reader.IsDBNull(reader.GetOrdinal("Description"));
            if (isNoDescription)
            {
                dto.ProductDescription = "No Description Addded";
            }
            else
            {
                dto.ProductDescription = reader.GetString(reader.GetOrdinal("Description"));

            }
            dto.Price = reader.GetDouble(reader.GetOrdinal("Price"));
            dto.Quantity = reader.GetInt32(reader.GetOrdinal("Quantity"));
            dto.Sold = reader.GetInt32(reader.GetOrdinal("Sold"));
            //  dto.PictureName = reader.GetString(reader.GetOrdinal("PictureName"));
            dto.CreatedOn = reader.GetDateTime(reader.GetOrdinal("CreatedOn"));
            dto.CreatedBy = reader.GetInt32(reader.GetOrdinal("CreatedBy"));

            dto.LowStockNotification = isLowStock(reader.GetDouble(reader.GetOrdinal("LowStockNotification")));

            if (reader.GetValue(reader.GetOrdinal("ModifiedOn")) != DBNull.Value)
                dto.ModifiedOn = reader.GetDateTime(reader.GetOrdinal("ModifiedOn"));
            if (reader.GetValue(reader.GetOrdinal("ModifiedBy")) != DBNull.Value)
                dto.ModifiedBy = reader.GetInt32(reader.GetOrdinal("ModifiedBy"));

            dto.IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"));
            dto.Likes = getLikesCount(dto.ProductID);
            dto.DisLikes = getDisLikesCount(dto.ProductID);
            return dto;
        }
    }
}
