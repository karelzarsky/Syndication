using System.Data.Entity.Migrations;

namespace SyndicateLogic.Migrations
{
    public partial class json : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.IntrinioJson",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        time = c.DateTime(nullable: false),
                        Json = c.String(),
                        Url = c.String(),
                        ClassT = c.String()
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.IntrinioJson");
        }
    }
}
