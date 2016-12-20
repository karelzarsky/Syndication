namespace SyndicateLogic.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class shingles : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Shingles",
                c => new
                    {
                        hash = c.Int(nullable: false),
                        text = c.String(),
                    })
                .PrimaryKey(t => t.hash);
            
            CreateTable(
                "dbo.ShingleUse",
                c => new
                    {
                        ShingleHash = c.Int(nullable: false),
                        ArticleHash = c.Binary(nullable: false, maxLength: 32),
                    })
                .PrimaryKey(t => t.ShingleHash)
                .ForeignKey("dbo.Articles", t => t.ArticleHash, cascadeDelete: true)
                .ForeignKey("dbo.Shingles", t => t.ShingleHash)
                .Index(t => t.ShingleHash)
                .Index(t => t.ArticleHash);
            
            AddColumn("dbo.Articles", "Processed1", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ShingleUse", "ShingleHash", "dbo.Shingles");
            DropForeignKey("dbo.ShingleUse", "ArticleHash", "dbo.Articles");
            DropIndex("dbo.ShingleUse", new[] { "ArticleHash" });
            DropIndex("dbo.ShingleUse", new[] { "ShingleHash" });
            DropColumn("dbo.Articles", "Processed1");
            DropTable("dbo.ShingleUse");
            DropTable("dbo.Shingles");
        }
    }
}
