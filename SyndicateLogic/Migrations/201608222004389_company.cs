using System.Data.Entity.Migrations;

namespace SyndicateLogic.Migrations
{
    public partial class company : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Companies",
                c => new
                    {
                        ticker = c.String(nullable: false, maxLength: 128),
                        cik = c.String(),
                        name = c.String(),
                        template = c.String(),
                        standardized_active = c.Boolean(nullable: false),
                        valuation_active = c.Boolean(nullable: false)
                    })
                .PrimaryKey(t => t.ticker);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Companies");
        }
    }
}
