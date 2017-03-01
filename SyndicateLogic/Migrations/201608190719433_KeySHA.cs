using System.Data.Entity.Migrations;

namespace SyndicateLogic.Migrations
{
    public partial class KeySHA : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.Articles");
            AlterColumn("dbo.Articles", "Sha256Hash", c => c.Binary(nullable: false, maxLength: 32));
            AddPrimaryKey("dbo.Articles", "Sha256Hash");
            DropColumn("dbo.Articles", "Hash");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Articles", "Hash", c => c.Int(nullable: false));
            DropPrimaryKey("dbo.Articles");
            AlterColumn("dbo.Articles", "Sha256Hash", c => c.Binary(maxLength: 32));
            AddPrimaryKey("dbo.Articles", "Hash");
        }
    }
}
