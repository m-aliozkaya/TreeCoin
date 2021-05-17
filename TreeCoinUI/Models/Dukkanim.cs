using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TreeCoinUI.Models
{
    public class Dukkanim
    {
        public int Id { get; set; }
        public int SupplierId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public bool IsApproved { get; set; }
        public double Price { get; set; }
        public int QuantityValue { get; set; }
        public string Image { get; set; }
    }
}