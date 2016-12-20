using System.Data.Entity.Migrations;

namespace SyndicateLogic.Migrations
{
    public partial class logs2 : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.Logs");
            AlterColumn("dbo.Logs", "Time", c => c.DateTime(false));
            AddPrimaryKey("dbo.Logs", "Time");
        }

        public override void Down()
        {
            DropPrimaryKey("dbo.Logs");
            AlterColumn("dbo.Logs", "Time", c => c.DateTime(false));
            AddPrimaryKey("dbo.Logs", "Time");
        }
    }
}