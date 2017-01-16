namespace SyndicateLogic.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class removeArticleHash : DbMigration
    {
        public override void Up()
        {
            DropIndex("rss.articles", new[] { "Hash32" });
            AddColumn("rss.articles", "Processed", c => c.Byte(nullable: false));
            AlterColumn("rss.articles", "Title", c => c.String(maxLength: 500));
            CreateIndex("rss.articles", "Title");
            CreateIndex("rss.articles", "Processed");
            DropColumn("rss.articles", "Hash32");
        }
        
        public override void Down()
        {
            AddColumn("rss.articles", "Hash32", c => c.Int(nullable: false));
            DropIndex("rss.articles", new[] { "Processed" });
            DropIndex("rss.articles", new[] { "Title" });
            AlterColumn("rss.articles", "Title", c => c.String());
            DropColumn("rss.articles", "Processed");
            CreateIndex("rss.articles", "Hash32");
        }
    }
}
