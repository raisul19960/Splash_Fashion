using Splash_fashion_Bd.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace Splash_fashion_Bd.ViewModels
{
    public class CustomerEditModel
    {
        public int Id { get; set; }
        [Required, StringLength(50), Display(Name = "Customer Name")]
        public string CustomerName { get; set; }
        [Required, StringLength(50)]
        public string Address { get; set; }
        [Required, StringLength(50)]
        public string Email { get; set; }
        public bool DoYouBuyOurProducts { get; set; }
        
        public HttpPostedFileBase Picture { get; set; }

        public virtual List<Order> Orders { get; set; } = new List<Order>();
    }
}