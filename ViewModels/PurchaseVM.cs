using System;
using System.Collections.Generic;
using Project.Models;

namespace Project.ViewModels
{
    public class PurchaseVM
    {
        public long clientID { get; set; }
        public List<Purchase> PurchaseList { get; set; }
       
    }
}