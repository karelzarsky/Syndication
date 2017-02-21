namespace SyndicateLogic.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class standardDeviation : DbMigration
    {
        public override void Up()
        {
            AddColumn("fact.shingleAction", "stddev", c => c.Double(nullable: false));
            AddColumn("fact.shingleAction", "mean", c => c.Double(nullable: false));
            AddColumn("fact.shingleAction", "variance", c => c.Double(nullable: false));
            AddColumn("fact.shingleAction", "median", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("fact.shingleAction", "median");
            DropColumn("fact.shingleAction", "variance");
            DropColumn("fact.shingleAction", "mean");
            DropColumn("fact.shingleAction", "stddev");
        }
    }
}
