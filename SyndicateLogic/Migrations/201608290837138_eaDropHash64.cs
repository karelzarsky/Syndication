using System.Data.Entity.Migrations;

namespace SyndicateLogic.Migrations
{
    public partial class eaDropHash64 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Articles", "Hash64");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Articles", "Hash64", c => c.Long(nullable: false));
        }
    }
}
