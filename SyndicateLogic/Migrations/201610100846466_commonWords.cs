namespace SyndicateLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class commonWords : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "app.commonWords",
                c => new
                    {
                        language = c.String(nullable: false, maxLength: 2, unicode: false),
                        text = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => new { t.language, t.text });
            
            DropColumn("rss.shingleUse", "Count");
        }
        
        public override void Down()
        {
            AddColumn("rss.shingleUse", "Count", c => c.Int(nullable: false));
            DropTable("app.commonWords");
        }
    }
}
