using System.Data.Entity.Migrations;

namespace SyndicateLogic.Migrations
{
    public partial class lastRecomputeDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("rss.shingles", "LastRecomputeDate", c => c.DateTime());
            CreateIndex("rss.shingles", "LastRecomputeDate");
        }
        
        public override void Down()
        {
            DropIndex("rss.shingles", new[] { "LastRecomputeDate" });
            DropColumn("rss.shingles", "LastRecomputeDate");
        }
    }
}
