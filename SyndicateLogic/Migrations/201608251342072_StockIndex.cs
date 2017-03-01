using System.Data.Entity.Migrations;

namespace SyndicateLogic.Migrations
{
    public partial class StockIndex : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.IndexComponents",
                c => new
                    {
                        symbol = c.String(nullable: false, maxLength: 10),
                        index_name = c.String(),
                        continent = c.String(),
                        country = c.String()
                    })
                .PrimaryKey(t => t.symbol);
            
            DropTable("dbo.IndicesList");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.IndicesList",
                c => new
                    {
                        symbol = c.String(nullable: false, maxLength: 128),
                        index_name = c.String()
                    })
                .PrimaryKey(t => t.symbol);
            
            DropTable("dbo.IndexComponents");
        }
    }
}
