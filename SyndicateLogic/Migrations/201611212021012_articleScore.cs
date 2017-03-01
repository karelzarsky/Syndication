using System.Data.Entity.Migrations;

namespace SyndicateLogic.Migrations
{
    public partial class articleScore : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "fact.articleScore",
                c => new
                    {
                        articleID = c.Int(nullable: false),
                        interval = c.Byte(nullable: false),
                        score = c.Decimal(storeType: "smallmoney"),
                        dateComputed = c.DateTime(nullable: false, storeType: "date")
                    })
                .PrimaryKey(t => new { t.articleID, t.interval });
            
        }
        
        public override void Down()
        {
            DropTable("fact.articleScore");
        }
    }
}
