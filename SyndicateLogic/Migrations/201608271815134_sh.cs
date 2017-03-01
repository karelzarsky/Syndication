using System.Data.Entity.Migrations;

namespace SyndicateLogic.Migrations
{
    public partial class sh : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Shingles", "language", c => c.String(maxLength: 2));
            CreateIndex("dbo.Articles", "Processed1");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Articles", new[] { "Processed1" });
            AlterColumn("dbo.Shingles", "language", c => c.String(maxLength: 5));
        }
    }
}
