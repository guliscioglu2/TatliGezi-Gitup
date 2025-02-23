using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TatliGezi.Models;
using TatliGezi.Models.Shop;

using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
namespace TatliGezi.Context
{
    public class TatliGeziContext : DbContext

    {
        public TatliGeziContext() : base("name=TatliGezi")
        {
            Database.Connection.ConnectionString = "Data Source=94.73.170.34; Initial Catalog= u1901958_tgezi; Persist Security Info=True; User ID=u1901958_usertg; Password=05359579170ZGul;MultipleActiveResultSets=True";

            //Database.Connection.ConnectionString = @"server=DESKTOP-8KBSEG5\MSSQLSERVER1; database=TatiGezi; UID=sa; PWD=1234567?; integrated security= true";

        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
        }
        public DbSet<Article> Articles { get; set; }

        public DbSet<Format> Formats { get; set; }


        public DbSet<Gallery> Galleries { get; set; }

        public DbSet<ArticleCategory> ArticleCategories { get; set; }
        public DbSet<Preview> Previews { get; set; }
        public DbSet<HostUser> HostUsers { get; set; }

        public DbSet<Message> Messages { get; set; }

        public DbSet<ArticleComment> ArticleComments { get; set; }
        public DbSet<ArticleTag> ArticleTags { get; set; }

        public DbSet<User> Users { get; set; }
        public DbSet<Cargo> Cargos { get; set; }

        public DbSet<ForgetPassword> ForgetPasswords { get; set; }

        public DbSet<ArticleCategoryJoin> ArticleCategoryJoins { get; set; }


        public DbSet<Basket> Baskets { get; set; }
        public DbSet<Slider> Sliders { get; set; }

        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<ProductComment> ProductComments { get; set; }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderPayment> OrderPayments { get; set; }
        public DbSet<OrderProduct> OrderProducts { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductTag> ProductTags { get; set; }

        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<KDVRate> KDVRates { get; set; }

        public DbSet<UserAdress> UserAdresses { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Town> Towns { get; set; }

        public DbSet<Sale> Sales { get; set; }



    }



}