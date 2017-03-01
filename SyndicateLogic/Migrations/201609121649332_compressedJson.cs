using System.Data.Entity.Migrations;

namespace SyndicateLogic.Migrations
{
    public partial class compressedJson : DbMigration
    {
        public override void Up()
        {
            AddColumn("int.json", "Compressed", c => c.Binary());
        }
        
        public override void Down()
        {
            DropColumn("int.json", "Compressed");
        }
    }
}
