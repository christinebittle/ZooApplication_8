namespace ZooApplication.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class animalspecies : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Animals", "SpeciesID", c => c.Int(nullable: false));
            CreateIndex("dbo.Animals", "SpeciesID");
            AddForeignKey("dbo.Animals", "SpeciesID", "dbo.Species", "SpeciesID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Animals", "SpeciesID", "dbo.Species");
            DropIndex("dbo.Animals", new[] { "SpeciesID" });
            DropColumn("dbo.Animals", "SpeciesID");
        }
    }
}
