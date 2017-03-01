using System.Data.Entity.Migrations;

namespace SyndicateLogic.Migrations
{
    public partial class logId : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.Logs");
            AddColumn("dbo.Logs", "Id", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.Logs", "Id");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.Logs");
            DropColumn("dbo.Logs", "Id");
            AddPrimaryKey("dbo.Logs", "Time");
        }
    }
}
