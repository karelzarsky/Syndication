using System.Data.Entity.Migrations;

namespace SyndicateLogic.Migrations
{
    public partial class priceSmallmoney : DbMigration
    {
        public override void Up()
        {
            //DropPrimaryKey("int.prices");
            AlterColumn("int.prices", "ticker", c => c.String(nullable: false, maxLength: 8));
            AlterColumn("int.prices", "date", c => c.DateTime(nullable: false, storeType: "date"));
            AlterColumn("int.prices", "open", c => c.Decimal(nullable: false, storeType: "smallmoney"));
            AlterColumn("int.prices", "high", c => c.Decimal(nullable: false, storeType: "smallmoney"));
            AlterColumn("int.prices", "low", c => c.Decimal(nullable: false, storeType: "smallmoney"));
            AlterColumn("int.prices", "close", c => c.Decimal(nullable: false, storeType: "smallmoney"));
            AlterColumn("int.prices", "volume", c => c.Long());
            AlterColumn("int.prices", "ex_dividend", c => c.Decimal(storeType: "smallmoney"));
            AlterColumn("int.prices", "split_ratio", c => c.Decimal(storeType: "smallmoney"));
            AlterColumn("int.prices", "adj_open", c => c.Decimal(storeType: "smallmoney"));
            AlterColumn("int.prices", "adj_high", c => c.Decimal(storeType: "smallmoney"));
            AlterColumn("int.prices", "adj_low", c => c.Decimal(storeType: "smallmoney"));
            AlterColumn("int.prices", "adj_close", c => c.Decimal(storeType: "smallmoney"));
            AlterColumn("int.prices", "adj_volume", c => c.Long());
            AddPrimaryKey("int.prices", new[] { "ticker", "date" });
        }
        
        public override void Down()
        {
            DropPrimaryKey("int.prices");
            AlterColumn("int.prices", "adj_volume", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("int.prices", "adj_close", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("int.prices", "adj_low", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("int.prices", "adj_high", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("int.prices", "adj_open", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("int.prices", "split_ratio", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("int.prices", "ex_dividend", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("int.prices", "volume", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("int.prices", "close", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("int.prices", "low", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("int.prices", "high", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("int.prices", "open", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("int.prices", "date", c => c.DateTime(nullable: false));
            AlterColumn("int.prices", "ticker", c => c.String(nullable: false, maxLength: 10));
            AddPrimaryKey("int.prices", new[] { "ticker", "date" });
        }
    }
}
