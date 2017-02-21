namespace SyndicateLogic.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class backtestKey : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("fact.backtest");
            AlterColumn("fact.backtest", "StrategyNr", c => c.Int(nullable: false));
            AddPrimaryKey("fact.backtest", "StrategyNr");
        }
        
        public override void Down()
        {
            DropPrimaryKey("fact.backtest");
            AlterColumn("fact.backtest", "StrategyNr", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("fact.backtest", "StrategyNr");
        }
    }
}
