namespace SyndicateLogic.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class indexPrice : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Prices", "volume", c => c.Decimal(precision: 18, scale: 5));
            AlterColumn("dbo.Prices", "ex_dividend", c => c.Decimal(precision: 18, scale: 5));
            AlterColumn("dbo.Prices", "split_ratio", c => c.Decimal(precision: 18, scale: 5));
            AlterColumn("dbo.Prices", "adj_open", c => c.Decimal(precision: 18, scale: 5));
            AlterColumn("dbo.Prices", "adj_high", c => c.Decimal(precision: 18, scale: 5));
            AlterColumn("dbo.Prices", "adj_low", c => c.Decimal(precision: 18, scale: 5));
            AlterColumn("dbo.Prices", "adj_close", c => c.Decimal(precision: 18, scale: 5));
            AlterColumn("dbo.Prices", "adj_volume", c => c.Decimal(precision: 18, scale: 5));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Prices", "adj_volume", c => c.Decimal(nullable: false, precision: 18, scale: 5));
            AlterColumn("dbo.Prices", "adj_close", c => c.Decimal(nullable: false, precision: 18, scale: 5));
            AlterColumn("dbo.Prices", "adj_low", c => c.Decimal(nullable: false, precision: 18, scale: 5));
            AlterColumn("dbo.Prices", "adj_high", c => c.Decimal(nullable: false, precision: 18, scale: 5));
            AlterColumn("dbo.Prices", "adj_open", c => c.Decimal(nullable: false, precision: 18, scale: 5));
            AlterColumn("dbo.Prices", "split_ratio", c => c.Decimal(nullable: false, precision: 18, scale: 5));
            AlterColumn("dbo.Prices", "ex_dividend", c => c.Decimal(nullable: false, precision: 18, scale: 5));
            AlterColumn("dbo.Prices", "volume", c => c.Decimal(nullable: false, precision: 18, scale: 5));
        }
    }
}
