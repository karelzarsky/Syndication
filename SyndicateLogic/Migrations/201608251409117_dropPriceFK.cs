namespace SyndicateLogic.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class dropPriceFK : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Prices", "ticker", "dbo.Companies");
            DropIndex("dbo.Prices", new[] { "ticker" });
            DropPrimaryKey("dbo.Prices");
            AlterColumn("dbo.Prices", "ticker", c => c.String(nullable: false, maxLength: 10));
            AddPrimaryKey("dbo.Prices", new[] { "ticker", "date" });
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.Prices");
            AlterColumn("dbo.Prices", "ticker", c => c.String(nullable: false, maxLength: 128));
            AddPrimaryKey("dbo.Prices", new[] { "ticker", "date" });
            CreateIndex("dbo.Prices", "ticker");
            AddForeignKey("dbo.Prices", "ticker", "dbo.Companies", "ticker", cascadeDelete: true);
        }
    }
}
