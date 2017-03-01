using System.Data.Entity.Migrations;

namespace SyndicateLogic.Migrations
{
    public partial class shingleID : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ShingleUse", "ArticleHash", "dbo.Articles");
            DropForeignKey("dbo.ShingleUse", "ShingleHash", "dbo.Shingles");
            DropIndex("dbo.ShingleUse", new[] { "ShingleHash" });
            DropIndex("dbo.ShingleUse", new[] { "ArticleHash" });
            RenameColumn(table: "dbo.ShingleUse", name: "ArticleHash", newName: "ArticleID");
            RenameColumn(table: "dbo.ShingleUse", name: "ShingleHash", newName: "ShingleID");
            DropPrimaryKey("dbo.Articles");
            DropPrimaryKey("dbo.Shingles");
            DropPrimaryKey("dbo.ShingleUse");
            AddColumn("dbo.Articles", "ID", c => c.Int(nullable: false, identity: true));
            AddColumn("dbo.Articles", "Hash64", c => c.Long(nullable: false));
            AddColumn("dbo.Articles", "Hash32", c => c.Int(nullable: false));
            AddColumn("dbo.Shingles", "ID", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.Shingles", "hash", c => c.Int(nullable: false));
            AlterColumn("dbo.Shingles", "text", c => c.String(nullable: false));
            AlterColumn("dbo.Shingles", "language", c => c.String(nullable: false, maxLength: 2));
            AlterColumn("dbo.ShingleUse", "ShingleID", c => c.Int(nullable: false));
            AlterColumn("dbo.ShingleUse", "ArticleID", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.Articles", "ID");
            AddPrimaryKey("dbo.Shingles", "ID");
            AddPrimaryKey("dbo.ShingleUse", new[] { "ShingleID", "ArticleID" });
            CreateIndex("dbo.Articles", "Hash32");
            CreateIndex("dbo.ShingleUse", "ShingleID");
            CreateIndex("dbo.ShingleUse", "ArticleID");
            AddForeignKey("dbo.ShingleUse", "ArticleID", "dbo.Articles", "ID", cascadeDelete: true);
            AddForeignKey("dbo.ShingleUse", "ShingleID", "dbo.Shingles", "ID", cascadeDelete: true);
            DropColumn("dbo.Articles", "Hash");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Articles", "Hash", c => c.Long(nullable: false));
            DropForeignKey("dbo.ShingleUse", "ShingleID", "dbo.Shingles");
            DropForeignKey("dbo.ShingleUse", "ArticleID", "dbo.Articles");
            DropIndex("dbo.ShingleUse", new[] { "ArticleID" });
            DropIndex("dbo.ShingleUse", new[] { "ShingleID" });
            DropIndex("dbo.Articles", new[] { "Hash32" });
            DropPrimaryKey("dbo.ShingleUse");
            DropPrimaryKey("dbo.Shingles");
            DropPrimaryKey("dbo.Articles");
            AlterColumn("dbo.ShingleUse", "ArticleID", c => c.Long(nullable: false));
            AlterColumn("dbo.ShingleUse", "ShingleID", c => c.Long(nullable: false));
            AlterColumn("dbo.Shingles", "language", c => c.String(maxLength: 2));
            AlterColumn("dbo.Shingles", "text", c => c.String());
            AlterColumn("dbo.Shingles", "hash", c => c.Long(nullable: false));
            DropColumn("dbo.Shingles", "ID");
            DropColumn("dbo.Articles", "Hash32");
            DropColumn("dbo.Articles", "Hash64");
            DropColumn("dbo.Articles", "ID");
            AddPrimaryKey("dbo.ShingleUse", new[] { "ShingleHash", "ArticleHash" });
            AddPrimaryKey("dbo.Shingles", "hash");
            AddPrimaryKey("dbo.Articles", "Hash");
            RenameColumn(table: "dbo.ShingleUse", name: "ShingleID", newName: "ShingleHash");
            RenameColumn(table: "dbo.ShingleUse", name: "ArticleID", newName: "ArticleHash");
            CreateIndex("dbo.ShingleUse", "ArticleHash");
            CreateIndex("dbo.ShingleUse", "ShingleHash");
            AddForeignKey("dbo.ShingleUse", "ShingleHash", "dbo.Shingles", "hash", cascadeDelete: true);
            AddForeignKey("dbo.ShingleUse", "ArticleHash", "dbo.Articles", "Hash", cascadeDelete: true);
        }
    }
}
