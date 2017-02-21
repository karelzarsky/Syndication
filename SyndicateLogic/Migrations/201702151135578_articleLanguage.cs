namespace SyndicateLogic.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class articleLanguage : DbMigration
    {
        public override void Up()
        {
            AddColumn("rss.articles", "language", c => c.String(maxLength: 2, unicode: false));
            CreateIndex("rss.articles", "language");
        }
        
        public override void Down()
        {
            DropIndex("rss.articles", new[] { "language" });
            DropColumn("rss.articles", "language");
        }
    }
}
