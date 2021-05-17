using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TreeCoinUI.Models
{
    public class AdminProductConfirm
    {
        public int Id { get; set; }
        public string QuantityType { get; set; }
        public string Name { get; set; }
        public bool IsApproved { get; set; }
        public string Image { get; set; }
    }
}