using System.Data.Entity.Migrations;

namespace SyndicateLogic.Migrations
{
    public partial class addSHA : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Articles", "Sha256Hash", c => c.Binary(maxLength: 32));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Articles", "Sha256Hash");
        }
    }
}
