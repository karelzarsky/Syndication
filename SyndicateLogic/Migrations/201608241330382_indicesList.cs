namespace SyndicateLogic.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class indicesList : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.IndicesList",
                c => new
                    {
                        symbol = c.String(nullable: false, maxLength: 128),
                        index_name = c.String(),
                    })
                .PrimaryKey(t => t.symbol);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.IndicesList");
        }
    }
}
