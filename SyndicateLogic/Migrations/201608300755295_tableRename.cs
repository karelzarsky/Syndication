using System.Data.Entity.Migrations;

namespace SyndicateLogic.Migrations
{
    public partial class tableRename : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.Articles", newName: "articles");
            RenameTable(name: "dbo.IntrinioJson", newName: "json");
            MoveTable(name: "dbo.companies", newSchema: "int");
            MoveTable(name: "dbo.companyDetails", newSchema: "int");
            MoveTable(name: "dbo.securities", newSchema: "int");
            MoveTable(name: "dbo.articles", newSchema: "rss");
            MoveTable(name: "dbo.feeds", newSchema: "rss");
            MoveTable(name: "dbo.json", newSchema: "int");
            MoveTable(name: "dbo.logs", newSchema: "app");
            MoveTable(name: "dbo.prices", newSchema: "int");
            MoveTable(name: "dbo.shingles", newSchema: "rss");
            MoveTable(name: "dbo.shingleUse", newSchema: "rss");
            MoveTable(name: "dbo.stockIndices", newSchema: "int");
        }
        
        public override void Down()
        {
            MoveTable(name: "int.stockIndices", newSchema: "dbo");
            MoveTable(name: "rss.shingleUse", newSchema: "dbo");
            MoveTable(name: "rss.shingles", newSchema: "dbo");
            MoveTable(name: "int.prices", newSchema: "dbo");
            MoveTable(name: "app.logs", newSchema: "dbo");
            MoveTable(name: "int.json", newSchema: "dbo");
            MoveTable(name: "rss.feeds", newSchema: "dbo");
            MoveTable(name: "rss.articles", newSchema: "dbo");
            MoveTable(name: "int.securities", newSchema: "dbo");
            MoveTable(name: "int.companyDetails", newSchema: "dbo");
            MoveTable(name: "int.companies", newSchema: "dbo");
            RenameTable(name: "dbo.json", newName: "IntrinioJson");
            RenameTable(name: "dbo.articles", newName: "Articles");
        }
    }
}
