using System.Data.Entity.Migrations;

namespace SyndicateLogic.Migrations
{
    public partial class shingleUseCount : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Shingles", "tokens", c => c.Byte(nullable: false));
            AddColumn("dbo.Shingles", "kind", c => c.Byte(nullable: false));
            AddColumn("dbo.ShingleUse", "Count", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ShingleUse", "Count");
            DropColumn("dbo.Shingles", "kind");
            DropColumn("dbo.Shingles", "tokens");
        }
    }
}
