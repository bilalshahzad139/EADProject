using PMS.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace PMS.DAL
{
    public class ProductSearchDAO
    {
        private static ProductDTO FillDTO(SqlDataReader reader)
        {
            var dto = new ProductDTO();
            if (reader != null)
            {
                dto.ProductID = reader.GetInt32(reader.GetOrdinal("ProductID"));
                dto.Name = reader.GetString(reader.GetOrdinal("Name"));
                dto.Price = reader.GetDouble(reader.GetOrdinal("Price"));
                var isNoDescription = reader.IsDBNull(reader.GetOrdinal("Description"));
                if (isNoDescription)
                {
                    dto.ProductDescription = "No Description Addded";
                }
                else
                {
                    dto.ProductDescription = reader.GetString(reader.GetOrdinal("Description"));

                }
                dto.PictureName = reader.GetString(reader.GetOrdinal("PictureName"));
                dto.CreatedOn = reader.GetDateTime(reader.GetOrdinal("CreatedOn"));
                dto.CreatedBy = reader.GetInt32(reader.GetOrdinal("CreatedBy"));
                if (reader.GetValue(reader.GetOrdinal("ModifiedOn")) != DBNull.Value)
                    dto.ModifiedOn = reader.GetDateTime(reader.GetOrdinal("ModifiedOn"));
                if (reader.GetValue(reader.GetOrdinal("ModifiedBy")) != DBNull.Value)
                    dto.ModifiedBy = reader.GetInt32(reader.GetOrdinal("ModifiedBy"));
                dto.IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"));
                dto.Likes = getLikesCount(dto.ProductID);
                dto.DisLikes = getDisLikesCount(dto.ProductID);
                dto.IsInWishlist = getWishlistProducts(dto.ProductID);
            }
            return dto;
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

        public static List<ProductDTO> GetProductByName(ProductSearchDTO dt) //function for getting product
        {
            var query = "SP_Product_SearchByName";
            using (var helper = new DBHelper())
            {
                if (dt.maxPrice == Int32.MaxValue)
                {
                    dt.maxPrice = 0;
                }
                SqlParameter[] sqlParameters = new SqlParameter[]
                {
                    new SqlParameter(){ParameterName="product_name",SqlDbType=System.Data.SqlDbType.VarChar, Value= dt.productName},
                    new SqlParameter(){ParameterName="minPrice",SqlDbType=System.Data.SqlDbType.Decimal, Value= dt.minPrice},
                    new SqlParameter(){ParameterName="maxPrice",SqlDbType=System.Data.SqlDbType.Decimal, Value= dt.maxPrice},
                    new SqlParameter(){ParameterName="categoryId",SqlDbType=System.Data.SqlDbType.Int, Value= dt.categoryId }
                };

                var reader = helper.ExecuteReader(query, sqlParameters);

                var list = new List<ProductDTO>();

                while (reader.Read())
                {
                    var dto = FillDTO(reader);
                    if (dto != null) list.Add(dto);
                }
                var commentsList = CommentDAO.GetTopComments(2);

                foreach (var prod in list)
                {
                    var prodComments = commentsList.Where(c => c.ProductID == prod.ProductID).ToList();
                    prod.Comments = prodComments;
                }
                return list;
            }
        }
        public static bool getWishlistProducts(int product_id)
        {
            var uid = (UserDTO)System.Web.HttpContext.Current.Session["user"];


            String sqlQuery = String.Format("select IsInList from dbo.Wishlist where UserID={0} and ProductID={1}", uid.UserID, product_id);
            using (DBHelper helper = new DBHelper())
            {
                var res = helper.ExecuteScalar(sqlQuery);
                if (res == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }

            }

        }
        }
}
