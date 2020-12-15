using System;
using System.ComponentModel.DataAnnotations;

namespace Project.Models
{
    public class Sale
    {
        [Key]
        public long ID { get; set; }
        public long FarmerID { get; set; }
        public DateTime Date { get; set; }
        public string Name { get; set; }
        public int Amount { get; set; }
        public string BuyersName { get; set; }
        public string Address { get; set; }

        public Sale()
        {
            
        }
        public Sale(long fid, string n, int a, string adr, string bname)
        {
            FarmerID = fid;
            Name = n;
            Amount = a;
            Date = DateTime.Now;
            BuyersName = bname;
            Address = adr;
        }
    }
}