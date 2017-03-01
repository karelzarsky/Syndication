using System.Data.Entity.Migrations;

namespace SyndicateLogic.Migrations
{
    public partial class removeMedian : DbMigration
    {
        public override void Up()
        {
            AlterColumn("fact.shingleAction", "dateComputed", c => c.DateTime(nullable: false));
            DropColumn("fact.shingleAction", "median");
        }
        
        public override void Down()
        {
            AddColumn("fact.shingleAction", "median", c => c.Double(nullable: false));
            AlterColumn("fact.shingleAction", "dateComputed", c => c.DateTime(nullable: false, storeType: "date"));
        }
    }
}
