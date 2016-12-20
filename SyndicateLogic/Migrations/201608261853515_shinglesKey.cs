namespace SyndicateLogic.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class shinglesKey : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ShingleUse", "ShingleHash", "dbo.Shingles");
            DropPrimaryKey("dbo.ShingleUse");
            AddPrimaryKey("dbo.ShingleUse", new[] { "ShingleHash", "ArticleHash" });
            AddForeignKey("dbo.ShingleUse", "ShingleHash", "dbo.Shingles", "hash", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ShingleUse", "ShingleHash", "dbo.Shingles");
            DropPrimaryKey("dbo.ShingleUse");
            AddPrimaryKey("dbo.ShingleUse", "ShingleHash");
            AddForeignKey("dbo.ShingleUse", "ShingleHash", "dbo.Shingles", "hash");
        }
    }
}
