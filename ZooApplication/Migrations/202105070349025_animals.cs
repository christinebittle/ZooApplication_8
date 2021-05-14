namespace ZooApplication.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class animals : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Animals",
                c => new
                    {
                        AnimalID = c.Int(nullable: false, identity: true),
                        AnimalName = c.String(),
                    })
                .PrimaryKey(t => t.AnimalID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Animals");
        }
    }
}
