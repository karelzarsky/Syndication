namespace SyndicateLogic.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class emplyeesNull : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.CompanyDetails", "employees", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.CompanyDetails", "employees", c => c.Int(nullable: false));
        }
    }
}
