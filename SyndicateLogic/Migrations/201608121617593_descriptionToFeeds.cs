using System.Data.Entity.Migrations;

namespace SyndicateLogic.Migrations
{
    public partial class descriptionToFeeds : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Feeds", "Description", c => c.String());
        }

        public override void Down()
        {
            DropColumn("dbo.Feeds", "Description");
        }
    }
}