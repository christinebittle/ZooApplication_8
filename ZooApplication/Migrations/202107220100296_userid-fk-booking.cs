namespace ZooApplication.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class useridfkbooking : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Bookings", "UserID", c => c.String(maxLength: 128));
            CreateIndex("dbo.Bookings", "UserID");
            AddForeignKey("dbo.Bookings", "UserID", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Bookings", "UserID", "dbo.AspNetUsers");
            DropIndex("dbo.Bookings", new[] { "UserID" });
            DropColumn("dbo.Bookings", "UserID");
        }
    }
}
