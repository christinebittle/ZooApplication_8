namespace ZooApplication.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tickettriviabooking2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Bookings",
                c => new
                    {
                        BookingID = c.Int(nullable: false, identity: true),
                        BookingName = c.String(),
                        BookingPhone = c.String(),
                        BookingDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.BookingID);
            
            CreateTable(
                "dbo.BookingxTickets",
                c => new
                    {
                        BookingXTicketID = c.Int(nullable: false, identity: true),
                        TicketID = c.Int(nullable: false),
                        BookingID = c.Int(nullable: false),
                        TicketQty = c.Int(nullable: false),
                        TicketPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.BookingXTicketID)
                .ForeignKey("dbo.Bookings", t => t.BookingID, cascadeDelete: true)
                .ForeignKey("dbo.Tickets", t => t.TicketID, cascadeDelete: true)
                .Index(t => t.TicketID)
                .Index(t => t.BookingID);
            
            CreateTable(
                "dbo.Tickets",
                c => new
                    {
                        TicketID = c.Int(nullable: false, identity: true),
                        TicketType = c.String(),
                        TicketPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.TicketID);
            
            CreateTable(
                "dbo.Trivias",
                c => new
                    {
                        TriviaTitle = c.String(nullable: false, maxLength: 128),
                        TriviaDesc = c.String(),
                        SpeciesID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.TriviaTitle)
                .ForeignKey("dbo.Species", t => t.SpeciesID, cascadeDelete: true)
                .Index(t => t.SpeciesID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Trivias", "SpeciesID", "dbo.Species");
            DropForeignKey("dbo.BookingxTickets", "TicketID", "dbo.Tickets");
            DropForeignKey("dbo.BookingxTickets", "BookingID", "dbo.Bookings");
            DropIndex("dbo.Trivias", new[] { "SpeciesID" });
            DropIndex("dbo.BookingxTickets", new[] { "BookingID" });
            DropIndex("dbo.BookingxTickets", new[] { "TicketID" });
            DropTable("dbo.Trivias");
            DropTable("dbo.Tickets");
            DropTable("dbo.BookingxTickets");
            DropTable("dbo.Bookings");
        }
    }
}
