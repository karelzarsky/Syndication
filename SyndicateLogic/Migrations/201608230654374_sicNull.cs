using System.Data.Entity.Migrations;

namespace SyndicateLogic.Migrations
{
    public partial class sicNull : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.CompanyDetails", "sic", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.CompanyDetails", "sic", c => c.Int(nullable: false));
        }
    }
}
