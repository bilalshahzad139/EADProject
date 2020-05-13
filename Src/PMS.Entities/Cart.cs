using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMS.Entities
{
    public class Cart
    {
        public int pid { get; set; }
        public String pictureName { get; set; }
        public String name { get; set; }
        public double price { get; set; }
        public int quantity { get; set; }

    }
}
