using System.Data.Entity.Migrations;

namespace SyndicateLogic.Migrations
{
    public partial class removeRSSID : DbMigration
    {
        public override void Up()
        {
            DropColumn("rss.articles", "RSS_ID");
        }
        
        public override void Down()
        {
            AddColumn("rss.articles", "RSS_ID", c => c.String());
        }
    }
}
