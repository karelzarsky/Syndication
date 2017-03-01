using System.Data.Entity.Migrations;

namespace SyndicateLogic.Migrations
{
    public partial class indexShingleText : DbMigration
    {
        public override void Up()
        {
            DropIndex("rss.shingles", new[] { "language" });
            AlterColumn("rss.shingles", "text", c => c.String(nullable: false, maxLength: 200));
            AlterColumn("rss.shingles", "language", c => c.String(nullable: false, maxLength: 2, unicode: false));
            CreateIndex("rss.shingles", "text");
            CreateIndex("rss.shingles", "language");
        }
        
        public override void Down()
        {
            DropIndex("rss.shingles", new[] { "language" });
            DropIndex("rss.shingles", new[] { "text" });
            AlterColumn("rss.shingles", "language", c => c.String(nullable: false, maxLength: 2));
            AlterColumn("rss.shingles", "text", c => c.String(nullable: false));
            CreateIndex("rss.shingles", "language");
        }
    }
}
