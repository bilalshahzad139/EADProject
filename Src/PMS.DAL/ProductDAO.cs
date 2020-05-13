﻿using System;
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
                    sqlQuery = String.Format("Update dbo.Products Set Name='{0}',Price='{1}',PictureName='{2}',ModifiedOn='{3}',ModifiedBy='{4}' Where ProductID={5}",
                        dto.Name, dto.Price, dto.PictureName, dto.ModifiedOn, dto.ModifiedBy, dto.ProductID);
                    helper.ExecuteQuery(sqlQuery);
                    return dto.ProductID;
                }
                else
                {
                    sqlQuery = String.Format("INSERT INTO dbo.Products(Name, Price, PictureName, CreatedOn, CreatedBy,IsActive) VALUES('{0}','{1}','{2}','{3}','{4}',{5}); Select @@IDENTITY",
                        dto.Name, dto.Price, dto.PictureName, dto.CreatedOn, dto.CreatedBy, 1);

                    var obj = helper.ExecuteScalar(sqlQuery);
                    return Convert.ToInt32(obj);
                }
            }
        }
        public static ProductDTO GetProductById(int pid)
        {
            var query = String.Format("Select * from dbo.Products Where ProductId={0}", pid);

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
            var query = "Select * from dbo.Products Where IsActive = 1;";

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

        public static int AddToCart(int pid,int uid)
        {
            var searchQuery = String.Format("select Quantity from dbo.Orders where UserID={0} and ProductID={1}", uid, pid);
            using (DBHelper helper = new DBHelper())
            {
                int res;
                var count =helper.ExecuteScalar(searchQuery);
                if (count==null)
                {
                     res = 0;
                }
                else
                {
                     res = (int)count;
                }
                if (res >0)
                {
                    String updateQuery = String.Format("update dbo.Orders set Quantity=Quantity + 1");
                    var changed = helper.ExecuteQuery(updateQuery);
                    return changed;
                }
                else
                {
                    String insertQuery = String.Format("insert into dbo.Orders values({0},{1},{2})", uid, pid,1);
                    var changed= helper.ExecuteQuery(insertQuery);
                    return changed;
                }
            }
        }
        public static List<PMS.Entities.Cart> getCart(int uid)
        {
            String searchQuery = String.Format("select Products.ProductID,Products.Name,Products.Price,Products.PictureName,Orders.Quantity from dbo.Products join dbo.Orders on Products.ProductID=Orders.ProductID where Orders.UserID={0}", uid);
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
                    item.price =(double) orders.GetDouble(orders.GetOrdinal("Price"));
                    item.quantity = orders.GetInt32(orders.GetOrdinal("Quantity"));
                    list.Add(item);
                }
                return list;
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
