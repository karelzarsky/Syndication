using System.Data.Entity.Migrations;

namespace SyndicateLogic.Migrations
{
    public partial class problematicShortcut : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "app.problematicShortcut",
                c => new
                    {
                        language = c.String(nullable: false, maxLength: 2, unicode: false),
                        text = c.String(nullable: false, maxLength: 50)
                    })
                .PrimaryKey(t => new { t.language, t.text });
            
        }
        
        public override void Down()
        {
            DropTable("app.problematicShortcut");
        }
    }
}
