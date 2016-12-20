using System.Data.Entity.Migrations;

namespace SyndicateLogic.Migrations
{
    public partial class logs : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Logs",
                c => new
                {
                    Time = c.DateTime(false),
                    Severity = c.Byte(false),
                    Message = c.String()
                })
                .PrimaryKey(t => t.Time);
        }

        public override void Down()
        {
            DropTable("dbo.Logs");
        }
    }
}