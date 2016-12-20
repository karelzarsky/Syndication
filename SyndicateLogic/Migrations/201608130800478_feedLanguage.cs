using System.Data.Entity.Migrations;

namespace SyndicateLogic.Migrations
{
    public partial class feedLanguage : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Feeds", "Language", c => c.String());
        }

        public override void Down()
        {
            DropColumn("dbo.Feeds", "Language");
        }
    }
}