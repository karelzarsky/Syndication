using System.Data.Entity.Migrations;

namespace SyndicateLogic.Migrations
{
    public partial class securityKey : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Securities", "companyTicker", c => c.String(maxLength: 128));
            CreateIndex("dbo.Securities", "companyTicker");
            AddForeignKey("dbo.Securities", "companyTicker", "dbo.CompanyDetails", "ticker");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Securities", "companyTicker", "dbo.CompanyDetails");
            DropIndex("dbo.Securities", new[] { "companyTicker" });
            DropColumn("dbo.Securities", "companyTicker");
        }
    }
}
