using System.Data.Entity.Migrations;

namespace SyndicateLogic.Migrations
{
    public partial class feedCategories : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Feeds", "Categories", c => c.String());
            AddColumn("dbo.Feeds", "Links", c => c.String());
        }

        public override void Down()
        {
            DropColumn("dbo.Feeds", "Links");
            DropColumn("dbo.Feeds", "Categories");
        }
    }
}