namespace SyndicateLogic.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class tableCompanyNames : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "app.companyNames",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        name = c.String(maxLength: 100),
                        ticker = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.name);
            
        }
        
        public override void Down()
        {
            DropIndex("app.companyNames", new[] { "name" });
            DropTable("app.companyNames");
        }
    }
}
