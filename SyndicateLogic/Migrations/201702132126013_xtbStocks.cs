namespace SyndicateLogic.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class xtbStocks : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "xtb.ETFStocksUS",
                c => new
                    {
                        Ticker = c.String(nullable: false, maxLength: 10, unicode: false),
                    })
                .PrimaryKey(t => t.Ticker);
            
        }
        
        public override void Down()
        {
            DropTable("xtb.ETFStocksUS");
        }
    }
}
