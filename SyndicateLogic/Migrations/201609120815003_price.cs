namespace SyndicateLogic.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class price : DbMigration
    {
        public override void Up()
        {
            AlterColumn("fact.tickerPriceAction", "changeUp", c => c.Single(nullable: false));
            AlterColumn("fact.tickerPriceAction", "changeDown", c => c.Single(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("fact.tickerPriceAction", "changeDown", c => c.Single(nullable: false));
            AlterColumn("fact.tickerPriceAction", "changeUp", c => c.Single(nullable: false));
        }
    }
}
