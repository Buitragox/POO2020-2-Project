using System;
using System.ComponentModel.DataAnnotations;

namespace Project.Models
{
    public class Purchase
    {
        [Key]
        public long ID { get; set; }
        public long ClientID { get; set; }
        public DateTime Date { get; set; }
        public string Name { get; set; }
        public int Amount { get; set; }

        public Purchase()
        {
            
        }
        public Purchase(long cid, string n, int a)
        {
            ClientID = cid;
            Name = n;
            Amount = a;
            Date = DateTime.Now;
        }
    }
}