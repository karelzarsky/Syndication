namespace SyndicateLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class exitReason : DbMigration
    {
        public override void Up()
        {
            AddColumn("fact.predictions", "Comment", c => c.String());
            AddColumn("fact.predictions", "Exit", c => c.Byte(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("fact.predictions", "Exit");
            DropColumn("fact.predictions", "Comment");
        }
    }
}
