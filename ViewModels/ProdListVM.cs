using System.Collections.Generic;
using Project.Models;

namespace Project.ViewModels
{
    public class ProdListVM
    {
        public List<Product> prodList { get; set; }
        public Dictionary<Product, int> cart { get; set; }
        public long clientID { get; set; }
        public long prodID { get; set; }
        public int amount { get; set; }
        public string message { get; set; }

        public ProdListVM()
        {
            prodList = new List<Product>();
            cart = new Dictionary<Product, int>();
            clientID = 0;
            prodID = 0;
            amount = 0;
        }
    }
}