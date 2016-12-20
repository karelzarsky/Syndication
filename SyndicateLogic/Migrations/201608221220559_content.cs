namespace SyndicateLogic.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class content : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Articles", "Content");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Articles", "Content", c => c.String());
        }
    }
}
