namespace SyndicateLogic.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class sicNull : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.CompanyDetails", "sic", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.CompanyDetails", "sic", c => c.Int(nullable: false));
        }
    }
}
