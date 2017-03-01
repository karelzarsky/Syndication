using System.Data.Entity.Migrations;

namespace SyndicateLogic.Migrations
{
    public partial class feedErrors : DbMigration
    {
        public override void Up()
        {
            AddColumn("rss.feeds", "LastError", c => c.DateTime());
            AddColumn("rss.feeds", "ErrorMessage", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("rss.feeds", "ErrorMessage");
            DropColumn("rss.feeds", "LastError");
        }
    }
}
