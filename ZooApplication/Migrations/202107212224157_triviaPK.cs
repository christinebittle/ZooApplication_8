namespace ZooApplication.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class triviaPK : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.Trivias");
            AddColumn("dbo.Trivias", "TriviaID", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.Trivias", "TriviaTitle", c => c.String());
            AddPrimaryKey("dbo.Trivias", "TriviaID");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.Trivias");
            AlterColumn("dbo.Trivias", "TriviaTitle", c => c.String(nullable: false, maxLength: 128));
            DropColumn("dbo.Trivias", "TriviaID");
            AddPrimaryKey("dbo.Trivias", "TriviaTitle");
        }
    }
}
