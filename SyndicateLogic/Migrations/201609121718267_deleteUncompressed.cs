namespace SyndicateLogic.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class deleteUncompressed : DbMigration
    {
        public override void Up()
        {
            DropColumn("int.json", "Json");
        }
        
        public override void Down()
        {
            AddColumn("int.json", "Json", c => c.String());
        }
    }
}
