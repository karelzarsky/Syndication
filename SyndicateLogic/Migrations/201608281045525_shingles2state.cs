using System.Data.Entity.Migrations;

namespace SyndicateLogic.Migrations
{
    public partial class shingles2state : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Articles", "Processed2", c => c.Byte(nullable: false));
            AddColumn("dbo.Articles", "Processed3", c => c.Byte(nullable: false));
            AddColumn("dbo.Articles", "Processed4", c => c.Byte(nullable: false));
            AddColumn("dbo.Articles", "Processed5", c => c.Byte(nullable: false));
            CreateIndex("dbo.Articles", "Processed2");
            CreateIndex("dbo.Articles", "Processed3");
            CreateIndex("dbo.Articles", "Processed4");
            CreateIndex("dbo.Articles", "Processed5");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Articles", new[] { "Processed5" });
            DropIndex("dbo.Articles", new[] { "Processed4" });
            DropIndex("dbo.Articles", new[] { "Processed3" });
            DropIndex("dbo.Articles", new[] { "Processed2" });
            DropColumn("dbo.Articles", "Processed5");
            DropColumn("dbo.Articles", "Processed4");
            DropColumn("dbo.Articles", "Processed3");
            DropColumn("dbo.Articles", "Processed2");
        }
    }
}
