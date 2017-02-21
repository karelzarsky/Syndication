namespace SyndicateLogic.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class articleLinks : DbMigration
    {
        public override void Up()
        {
            AddColumn("rss.articles", "RSS_ID", c => c.String());
            AddColumn("rss.articles", "URI_links", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("rss.articles", "URI_links");
            DropColumn("rss.articles", "RSS_ID");
        }
    }
}
