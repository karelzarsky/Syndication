using System.Data.Entity.Migrations;

namespace SyndicateLogic.Migrations
{
    public partial class shIndexes : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Shingles", "hash");
            CreateIndex("dbo.Shingles", "tokens");
            CreateIndex("dbo.Shingles", "kind");
            CreateIndex("dbo.Shingles", "language");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Shingles", new[] { "language" });
            DropIndex("dbo.Shingles", new[] { "kind" });
            DropIndex("dbo.Shingles", new[] { "tokens" });
            DropIndex("dbo.Shingles", new[] { "hash" });
        }
    }
}
