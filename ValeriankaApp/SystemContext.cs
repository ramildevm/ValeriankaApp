using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValeriankaApp
{
    static class SystemContext
    {
        public static Users User { get; set; } = null;
        public static Product Product { get; set; } = null;
        public static Client Client { get; set; } = null;
        public static Shop Shop { get; set; } = null;
        public static Orders Orders { get; set; } = null;
        public static Basket Basket { get; set; } = null;
        public static ShopAddressLink ShopAddressLink { get; set; } = null;

        public static string typeWindow = "Каталог";
    }
}
