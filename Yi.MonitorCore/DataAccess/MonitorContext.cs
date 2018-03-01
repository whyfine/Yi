using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yi.MonitorCore.Models;

namespace Yi.MonitorCore.DataAccess
{
    public class MonitorContext : DbContext
    {
        //传入connection string name
        //SERVER=localhost;UID=root;PWD=123;DATABASE=monitor;charset=utf8;Convert Zero Datetime=True;
        public MonitorContext()
            : base("MonitorContext")
        {
            this.Configuration.AutoDetectChangesEnabled = false;
            this.Configuration.ValidateOnSaveEnabled = false;
        }
        //映射数据库中Condition表的数据集
        public DbSet<Page> Pages { get; set; }
        public DbSet<UserInfo> UserInfos { get; set; }
        public DbSet<UserColl> UserColls { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<OrderMain> OrderMains { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }

        //Code First - MySQL - error can't find table
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Page>().ToTable("page");
            modelBuilder.Entity<UserInfo>().ToTable("userinfo");
            modelBuilder.Entity<UserColl>().ToTable("usercoll");
            modelBuilder.Entity<Comment>().ToTable("comment");
            modelBuilder.Entity<OrderMain>().ToTable("ordermain");
            modelBuilder.Entity<OrderDetail>().ToTable("orderdetail");
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}
