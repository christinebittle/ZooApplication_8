namespace ZooApplication.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class keepersanimals : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Keepers",
                c => new
                    {
                        KeeperID = c.Int(nullable: false, identity: true),
                        KeeperFirstName = c.String(),
                        KeeperLastName = c.String(),
                    })
                .PrimaryKey(t => t.KeeperID);
            
            CreateTable(
                "dbo.KeeperAnimals",
                c => new
                    {
                        Keeper_KeeperID = c.Int(nullable: false),
                        Animal_AnimalID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Keeper_KeeperID, t.Animal_AnimalID })
                .ForeignKey("dbo.Keepers", t => t.Keeper_KeeperID, cascadeDelete: true)
                .ForeignKey("dbo.Animals", t => t.Animal_AnimalID, cascadeDelete: true)
                .Index(t => t.Keeper_KeeperID)
                .Index(t => t.Animal_AnimalID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.KeeperAnimals", "Animal_AnimalID", "dbo.Animals");
            DropForeignKey("dbo.KeeperAnimals", "Keeper_KeeperID", "dbo.Keepers");
            DropIndex("dbo.KeeperAnimals", new[] { "Animal_AnimalID" });
            DropIndex("dbo.KeeperAnimals", new[] { "Keeper_KeeperID" });
            DropTable("dbo.KeeperAnimals");
            DropTable("dbo.Keepers");
        }
    }
}
