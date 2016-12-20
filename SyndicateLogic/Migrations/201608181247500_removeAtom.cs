namespace SyndicateLogic.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class removeAtom : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Articles", "PublishedUTC");
            DropColumn("dbo.Articles", "ATOM10");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Articles", "ATOM10", c => c.String());
            DropIndex("dbo.Articles", new[] { "PublishedUTC" });
        }
    }
}
