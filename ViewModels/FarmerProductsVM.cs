using System.Collections.Generic;
using Project.Models;

namespace Project.ViewModels
{
    public class FarmerProductsVM
    {
        public List<Product> prodList { get; set; }
        public long farmerID { get; set; }

        public FarmerProductsVM()
        {
            prodList = new List<Product>();
            farmerID = 0;
        }
    }
}