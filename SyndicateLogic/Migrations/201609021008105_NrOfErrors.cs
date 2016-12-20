namespace SyndicateLogic.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class NrOfErrors : DbMigration
    {
        public override void Up()
        {
            AddColumn("rss.servers", "Errors", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("rss.servers", "Errors");
        }
    }
}
