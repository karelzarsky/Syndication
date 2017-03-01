using System.Data.Entity.Migrations;

namespace SyndicateLogic.Migrations
{
    public partial class feedAffected : DbMigration
    {
        public override void Up()
        {
            AddColumn("rss.feeds", "AffectedInstrument_ID", c => c.Int());
            CreateIndex("rss.feeds", "AffectedInstrument_ID");
            AddForeignKey("rss.feeds", "AffectedInstrument_ID", "app.instruments", "ID");
        }
        
        public override void Down()
        {
            DropForeignKey("rss.feeds", "AffectedInstrument_ID", "app.instruments");
            DropIndex("rss.feeds", new[] { "AffectedInstrument_ID" });
            DropColumn("rss.feeds", "AffectedInstrument_ID");
        }
    }
}
