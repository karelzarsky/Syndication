namespace SyndicateLogic.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class indexComponent : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "app.indexComponents",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IndexSymbol = c.String(maxLength: 20),
                        StockTicker = c.String(maxLength: 10),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.IndexSymbol)
                .Index(t => t.StockTicker);
            
        }
        
        public override void Down()
        {
            DropIndex("app.indexComponents", new[] { "StockTicker" });
            DropIndex("app.indexComponents", new[] { "IndexSymbol" });
            DropTable("app.indexComponents");
        }
    }
}
