namespace SyndicateLogic.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class logIndex : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Logs", "Time");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Logs", new[] { "Time" });
        }
    }
}
