using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMS.Entities
{
    public class feedbackDTO
    {
        public int Id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string pNo { get; set; }
        public string message { get; set; }
    }
}
