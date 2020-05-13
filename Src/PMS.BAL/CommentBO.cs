using PMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMS.BAL
{
    public static class CommentBO
    {
        public static int Save(CommentDTO dto)
        {
            return DAL.CommentDAO.Save(dto);
        }

        public static CommentDTO GetCommentById(int pid)
        {
            return DAL.CommentDAO.GetCommentById(pid);
        }
        public static List<CommentDTO> GetAllComments()
        {
            return DAL.CommentDAO.GetAllComments();
        }


    }
}
