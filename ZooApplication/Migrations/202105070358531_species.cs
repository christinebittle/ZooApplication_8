namespace ZooApplication.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class species : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Species",
                c => new
                    {
                        SpeciesID = c.Int(nullable: false, identity: true),
                        SpeciesName = c.String(),
                        SpeciesEndangered = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.SpeciesID);
            
            AddColumn("dbo.Animals", "AnimalWeight", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Animals", "AnimalWeight");
            DropTable("dbo.Species");
        }
    }
}
