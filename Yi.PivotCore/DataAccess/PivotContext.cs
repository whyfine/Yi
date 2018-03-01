using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yi.PivotCore.Models;

namespace Yi.PivotCore.DataAccess
{
    public class PivotContext : DbContext
    {
        //传入connection string name
        //SERVER=localhost;UID=root;PWD=123;DATABASE=monitor;charset=utf8;Convert Zero Datetime=True;
        public PivotContext()
            : base("PivotContext")
        {
            this.Configuration.AutoDetectChangesEnabled = false;
            this.Configuration.ValidateOnSaveEnabled = false;
        }
        //映射数据库中Condition表的数据集
        public DbSet<Overview> Overviews { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Address> Addresss { get; set; }
        public DbSet<Channel> Channels { get; set; }
        public DbSet<Content> Contents { get; set; }
        public DbSet<ApiInfo> ApiInfos { get; set; }
        public DbSet<Related> Relateds { get; set; }

        //Code First - MySQL - error can't find table
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Overview>().ToTable("overview");
            modelBuilder.Entity<Member>().ToTable("member");
            modelBuilder.Entity<Address>().ToTable("address");
            modelBuilder.Entity<Channel>().ToTable("channel");
            modelBuilder.Entity<Content>().ToTable("content");
            modelBuilder.Entity<ApiInfo>().ToTable("apiinfo");
            modelBuilder.Entity<Related>().ToTable("related");
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}
