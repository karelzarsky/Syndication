namespace SyndicateLogic.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class lastPriceUpdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("app.instruments", "LastPriceUpdate", c => c.DateTime());
            CreateIndex("app.instruments", "LastPriceUpdate");
        }
        
        public override void Down()
        {
            DropIndex("app.instruments", new[] { "LastPriceUpdate" });
            DropColumn("app.instruments", "LastPriceUpdate");
        }
    }
}
