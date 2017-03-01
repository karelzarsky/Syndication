using System.Data.Entity.Migrations;

namespace SyndicateLogic.Migrations
{
    public partial class CompanyIndexing : DbMigration
    {
        public override void Up()
        {
            AlterColumn("int.companies", "name", c => c.String(maxLength: 100));
            AlterColumn("int.companyDetails", "legal_name", c => c.String(maxLength: 100));
            AlterColumn("int.companyDetails", "ceo", c => c.String(maxLength: 50));
            CreateIndex("int.companies", "name");
            CreateIndex("int.companyDetails", "legal_name");
            CreateIndex("int.companyDetails", "ceo");
        }
        
        public override void Down()
        {
            DropIndex("int.companyDetails", new[] { "ceo" });
            DropIndex("int.companyDetails", new[] { "legal_name" });
            DropIndex("int.companies", new[] { "name" });
            AlterColumn("int.companyDetails", "ceo", c => c.String());
            AlterColumn("int.companyDetails", "legal_name", c => c.String());
            AlterColumn("int.companies", "name", c => c.String(maxLength: 8000, unicode: false));
        }
    }
}
