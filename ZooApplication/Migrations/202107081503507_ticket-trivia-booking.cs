namespace ZooApplication.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tickettriviabooking : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Animals", "AnimalSex", c => c.Int(nullable: false));
            AddColumn("dbo.Species", "SpeciesAvailable", c => c.Boolean(nullable: false));
            AddColumn("dbo.Species", "SpeciesFeatured", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Species", "SpeciesFeatured");
            DropColumn("dbo.Species", "SpeciesAvailable");
            DropColumn("dbo.Animals", "AnimalSex");
        }
    }
}
