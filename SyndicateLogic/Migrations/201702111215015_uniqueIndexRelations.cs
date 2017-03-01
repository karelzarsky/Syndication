using System.Data.Entity.Migrations;

namespace SyndicateLogic.Migrations
{
    public partial class uniqueIndexRelations : DbMigration
    {
        public override void Up()
        {
            CreateIndex("rss.articleRelations", new[] { "ArticleID", "InstrumentID" }, unique: true, name: "IX_ArticleAndInstrument");
        }
        
        public override void Down()
        {
            DropIndex("rss.articleRelations", "IX_ArticleAndInstrument");
        }
    }
}
