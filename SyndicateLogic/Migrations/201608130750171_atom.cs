using System.Data.Entity.Migrations;

namespace SyndicateLogic.Migrations
{
    public partial class atom : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Articles", "ATOM10", c => c.String());
        }

        public override void Down()
        {
            DropColumn("dbo.Articles", "ATOM10");
        }
    }
}