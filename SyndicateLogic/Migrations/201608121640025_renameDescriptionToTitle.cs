using System.Data.Entity.Migrations;

namespace SyndicateLogic.Migrations
{
    public partial class renameDescriptionToTitle : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Feeds", "Title", c => c.String());
            DropColumn("dbo.Feeds", "Description");
        }

        public override void Down()
        {
            AddColumn("dbo.Feeds", "Description", c => c.String());
            DropColumn("dbo.Feeds", "Title");
        }
    }
}