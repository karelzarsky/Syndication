using System.Data.Entity.Migrations;

namespace SyndicateLogic.Migrations
{
    public partial class EAHash : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Articles", "Hash", c => c.Long(nullable: false));
            AddColumn("dbo.Articles", "ReceivedUTC", c => c.DateTime(nullable: false));
            AddColumn("dbo.Articles", "AffectedInstruments", c => c.String());
            CreateIndex("dbo.Articles", "Hash");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Articles", new[] { "Hash" });
            DropColumn("dbo.Articles", "AffectedInstruments");
            DropColumn("dbo.Articles", "ReceivedUTC");
            DropColumn("dbo.Articles", "Hash");
        }
    }
}
