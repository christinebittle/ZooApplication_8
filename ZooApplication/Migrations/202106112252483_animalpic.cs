namespace ZooApplication.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class animalpic : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Animals", "AnimalHasPic", c => c.Boolean(nullable: false));
            AddColumn("dbo.Animals", "PicExtension", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Animals", "PicExtension");
            DropColumn("dbo.Animals", "AnimalHasPic");
        }
    }
}
