namespace SyndicateLogic.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class indexCompanyName : DbMigration
    {
        public override void Up()
        {
            AlterColumn("int.companyDetails", "name", c => c.String(maxLength: 100));
            CreateIndex("int.companyDetails", "name");
        }
        
        public override void Down()
        {
            DropIndex("int.companyDetails", new[] { "name" });
            AlterColumn("int.companyDetails", "name", c => c.String());
        }
    }
}
