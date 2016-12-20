namespace SyndicateLogic.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class shingleLang : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Shingles", "language", c => c.String(maxLength: 5));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Shingles", "language");
        }
    }
}
