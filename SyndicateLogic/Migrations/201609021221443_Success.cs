namespace SyndicateLogic.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class Success : DbMigration
    {
        public override void Up()
        {
            AddColumn("rss.servers", "Success", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("rss.servers", "Success");
        }
    }
}
