using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using System.Data.Entity;

namespace Splash_fashion_Bd.Models
{
    public enum Status { Pending = 1, Delivered, Cancelled }
    public abstract class EntityBase
    {
        [Key]
        public int Id { get; set; }
    }
    public class Customer:EntityBase
    {
        [Required, StringLength(50), Display(Name = "Customer Name")]
        public string CustomerName { get; set; }
        [Required, StringLength(50)]
        public string Address { get; set; }
        [Required, StringLength(50)]
        public string Email { get; set; }
        public bool DoYouBuyOurProducts{ get; set; }
        [Required, StringLength(150)]
        public string Picture { get; set; }

        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }
    public class Order : EntityBase
    {
        [Required, Column(TypeName = "date"),
            Display(Name = "Order Date")]
        public DateTime OrderDate { get; set; }
        [Column(TypeName = "date"),
            Display(Name = "Delivery Date")]
        public DateTime DeliveryDate { get; set; }
        [Required, EnumDataType(typeof(Status))]
        public Status Status { get; set; }
        [Required, ForeignKey("Customer")]
        public int CustomerId { get; set; }
        public virtual Customer Customer { get; set; }

    }
    public class CustomerDbContext : DbContext
    {
        public CustomerDbContext()
        {
            Database.SetInitializer(new DbInitializer());
        }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
    }
    public class DbInitializer : DropCreateDatabaseIfModelChanges<CustomerDbContext>
    {
        protected override void Seed(CustomerDbContext context)
        {
            Customer c = new Customer { CustomerName = "Raisul Islam", Address = "Shewrapara", Email = "raisul@gmail.com", DoYouBuyOurProducts = true, Picture = "e1.jpg" };
            c.Orders.Add(new Order { OrderDate =DateTime.Parse("10-10-2202"), DeliveryDate= DateTime.Parse("10-10-2202"), Status = Status.Pending});
            context.Customers.Add(c);
            context.SaveChanges();
        }
    }
}