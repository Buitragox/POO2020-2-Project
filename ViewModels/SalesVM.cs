using System;
using System.Collections.Generic;
using Project.Models;

namespace Project.ViewModels
{
    public class SalesVM
    {
       public long farmerID { get; set; }
       public List<Sale> SaleList { get; set; }
       
    }
}