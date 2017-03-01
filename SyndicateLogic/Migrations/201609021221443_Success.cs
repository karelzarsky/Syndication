using System.Data.Entity.Migrations;

namespace SyndicateLogic.Migrations
{
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
