using System.Data.Entity.Migrations;

namespace SyndicateLogic.Migrations
{
    public partial class tableCurrencies : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "app.currencies",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Entity = c.String(),
                        CurrencyName = c.String(),
                        AplhabeticCode = c.String(maxLength: 3, unicode: false),
                        NumericCode = c.Short(),
                        MinorUnit = c.String(),
                        WithdrawalDate = c.String()
                    })
                .PrimaryKey(t => t.ID)
                .Index(t => t.AplhabeticCode);
            
        }
        
        public override void Down()
        {
            DropIndex("app.currencies", new[] { "AplhabeticCode" });
            DropTable("app.currencies");
        }
    }
}
