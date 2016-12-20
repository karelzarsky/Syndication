namespace SyndicateLogic.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class hash64 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ShingleUse", "ArticleHash", "dbo.Articles");
            DropForeignKey("dbo.ShingleUse", "ShingleHash", "dbo.Shingles");
            DropIndex("dbo.Articles", new[] { "Hash" });
            DropIndex("dbo.ShingleUse", new[] { "ShingleHash" });
            DropIndex("dbo.ShingleUse", new[] { "ArticleHash" });
            DropPrimaryKey("dbo.Articles");
            DropPrimaryKey("dbo.Shingles");
            DropPrimaryKey("dbo.ShingleUse");
            AlterColumn("dbo.Shingles", "hash", c => c.Long(nullable: false));
            AlterColumn("dbo.ShingleUse", "ShingleHash", c => c.Long(nullable: false));
            AlterColumn("dbo.ShingleUse", "ArticleHash", c => c.Long(nullable: false));
            AddPrimaryKey("dbo.Articles", "Hash");
            AddPrimaryKey("dbo.Shingles", "hash");
            AddPrimaryKey("dbo.ShingleUse", new[] { "ShingleHash", "ArticleHash" });
            CreateIndex("dbo.ShingleUse", "ShingleHash");
            CreateIndex("dbo.ShingleUse", "ArticleHash");
            AddForeignKey("dbo.ShingleUse", "ArticleHash", "dbo.Articles", "Hash", cascadeDelete: true);
            AddForeignKey("dbo.ShingleUse", "ShingleHash", "dbo.Shingles", "hash", cascadeDelete: true);
            DropColumn("dbo.Articles", "Sha256Hash");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Articles", "Sha256Hash", c => c.Binary(nullable: false, maxLength: 32));
            DropForeignKey("dbo.ShingleUse", "ShingleHash", "dbo.Shingles");
            DropForeignKey("dbo.ShingleUse", "ArticleHash", "dbo.Articles");
            DropIndex("dbo.ShingleUse", new[] { "ArticleHash" });
            DropIndex("dbo.ShingleUse", new[] { "ShingleHash" });
            DropPrimaryKey("dbo.ShingleUse");
            DropPrimaryKey("dbo.Shingles");
            DropPrimaryKey("dbo.Articles");
            AlterColumn("dbo.ShingleUse", "ArticleHash", c => c.Binary(nullable: false, maxLength: 32));
            AlterColumn("dbo.ShingleUse", "ShingleHash", c => c.Int(nullable: false));
            AlterColumn("dbo.Shingles", "hash", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.ShingleUse", new[] { "ShingleHash", "ArticleHash" });
            AddPrimaryKey("dbo.Shingles", "hash");
            AddPrimaryKey("dbo.Articles", "Sha256Hash");
            CreateIndex("dbo.ShingleUse", "ArticleHash");
            CreateIndex("dbo.ShingleUse", "ShingleHash");
            CreateIndex("dbo.Articles", "Hash");
            AddForeignKey("dbo.ShingleUse", "ShingleHash", "dbo.Shingles", "hash", cascadeDelete: true);
            AddForeignKey("dbo.ShingleUse", "ArticleHash", "dbo.Articles", "Sha256Hash", cascadeDelete: true);
        }
    }
}
