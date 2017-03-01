using System.Data.Entity.Migrations;

namespace SyndicateLogic.Migrations
{
    public partial class securityDate : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Securities", "last_crsp_adj_date", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Securities", "last_crsp_adj_date", c => c.DateTime(nullable: false));
        }
    }
}
