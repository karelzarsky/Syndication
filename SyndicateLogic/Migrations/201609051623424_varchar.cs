using System.Data.Entity.Migrations;

namespace SyndicateLogic.Migrations
{
    public partial class varchar : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("int.securities", "companyTicker", "int.companyDetails");
            DropIndex("app.instruments", new[] { "Ticker" });
            DropIndex("app.instruments", new[] { "StockExchange" });
            DropIndex("rss.servers", new[] { "HostName" });
            DropIndex("int.securities", new[] { "companyTicker" });
            DropIndex("app.indexComponents", new[] { "IndexSymbol" });
            DropIndex("app.indexComponents", new[] { "StockTicker" });
            //DropPrimaryKey("int.companies");
            //DropPrimaryKey("int.companyDetails");
            //DropPrimaryKey("int.securities");
            //DropPrimaryKey("int.prices");
            //DropPrimaryKey("int.stockIndices");
            AlterColumn("rss.feeds", "Url", c => c.String(maxLength: 8000, unicode: false));
            AlterColumn("rss.feeds", "Language", c => c.String(maxLength: 8000, unicode: false));
            AlterColumn("rss.feeds", "Links", c => c.String(maxLength: 8000, unicode: false));
            AlterColumn("app.instruments", "Ticker", c => c.String(maxLength: 30, unicode: false));
            AlterColumn("app.instruments", "StockExchange", c => c.String(maxLength: 10, unicode: false));
            AlterColumn("rss.servers", "HostName", c => c.String(maxLength: 40, unicode: false));
            AlterColumn("int.companies", "ticker", c => c.String(nullable: false, maxLength: 128, unicode: false));
            AlterColumn("int.companies", "cik", c => c.String(maxLength: 8000, unicode: false));
            AlterColumn("int.companies", "name", c => c.String(maxLength: 8000, unicode: false));
            AlterColumn("int.companies", "template", c => c.String(maxLength: 8000, unicode: false));
            AlterColumn("int.companyDetails", "ticker", c => c.String(nullable: false, maxLength: 128, unicode: false));
            AlterColumn("int.companyDetails", "stock_exchange", c => c.String(maxLength: 8000, unicode: false));
            AlterColumn("int.companyDetails", "company_url", c => c.String(maxLength: 8000, unicode: false));
            AlterColumn("int.companyDetails", "cik", c => c.String(maxLength: 8000, unicode: false));
            AlterColumn("int.companyDetails", "template", c => c.String(maxLength: 8000, unicode: false));
            AlterColumn("int.companyDetails", "hq_state", c => c.String(maxLength: 8000, unicode: false));
            AlterColumn("int.companyDetails", "inc_state", c => c.String(maxLength: 8000, unicode: false));
            AlterColumn("int.companyDetails", "sector", c => c.String(maxLength: 8000, unicode: false));
            AlterColumn("int.companyDetails", "industry_category", c => c.String(maxLength: 8000, unicode: false));
            AlterColumn("int.companyDetails", "industry_group", c => c.String(maxLength: 8000, unicode: false));
            AlterColumn("int.securities", "ticker", c => c.String(nullable: false, maxLength: 128, unicode: false));
            AlterColumn("int.securities", "companyTicker", c => c.String(maxLength: 128, unicode: false));
            AlterColumn("int.securities", "security_name", c => c.String(maxLength: 8000, unicode: false));
            AlterColumn("int.securities", "security_type", c => c.String(maxLength: 8000, unicode: false));
            AlterColumn("int.securities", "stock_exchange", c => c.String(maxLength: 8000, unicode: false));
            AlterColumn("int.securities", "listing_exchange", c => c.String(maxLength: 8000, unicode: false));
            AlterColumn("int.securities", "market_category", c => c.String(maxLength: 8000, unicode: false));
            AlterColumn("int.securities", "financial_status", c => c.String(maxLength: 8000, unicode: false));
            AlterColumn("app.indexComponents", "IndexSymbol", c => c.String(maxLength: 20, unicode: false));
            AlterColumn("app.indexComponents", "StockTicker", c => c.String(maxLength: 10, unicode: false));
            AlterColumn("int.json", "Url", c => c.String(maxLength: 8000, unicode: false));
            AlterColumn("int.json", "ClassT", c => c.String(maxLength: 8000, unicode: false));
            AlterColumn("int.prices", "ticker", c => c.String(nullable: false, maxLength: 8, unicode: false));
            AlterColumn("int.stockIndices", "symbol", c => c.String(nullable: false, maxLength: 10, unicode: false));
            AlterColumn("int.stockIndices", "index_name", c => c.String(maxLength: 8000, unicode: false));
            AlterColumn("int.stockIndices", "continent", c => c.String(maxLength: 8000, unicode: false));
            AlterColumn("int.stockIndices", "country", c => c.String(maxLength: 8000, unicode: false));
            AddPrimaryKey("int.companies", "ticker");
            AddPrimaryKey("int.companyDetails", "ticker");
            AddPrimaryKey("int.securities", "ticker");
            AddPrimaryKey("int.prices", new[] { "ticker", "date" });
            AddPrimaryKey("int.stockIndices", "symbol");
            CreateIndex("app.instruments", "Ticker");
            CreateIndex("app.instruments", "StockExchange");
            CreateIndex("rss.servers", "HostName");
            CreateIndex("int.securities", "companyTicker");
            CreateIndex("app.indexComponents", "IndexSymbol");
            CreateIndex("app.indexComponents", "StockTicker");
            AddForeignKey("int.securities", "companyTicker", "int.companyDetails", "ticker");
            DropColumn("rss.ArticleRelations", "Reason");
            DropColumn("rss.articles", "AffectedInstruments");
        }
        
        public override void Down()
        {
            AddColumn("rss.articles", "AffectedInstruments", c => c.String());
            AddColumn("rss.ArticleRelations", "Reason", c => c.String());
            DropForeignKey("int.securities", "companyTicker", "int.companyDetails");
            DropIndex("app.indexComponents", new[] { "StockTicker" });
            DropIndex("app.indexComponents", new[] { "IndexSymbol" });
            DropIndex("int.securities", new[] { "companyTicker" });
            DropIndex("rss.servers", new[] { "HostName" });
            DropIndex("app.instruments", new[] { "StockExchange" });
            DropIndex("app.instruments", new[] { "Ticker" });
            DropPrimaryKey("int.stockIndices");
            DropPrimaryKey("int.prices");
            DropPrimaryKey("int.securities");
            DropPrimaryKey("int.companyDetails");
            DropPrimaryKey("int.companies");
            AlterColumn("int.stockIndices", "country", c => c.String());
            AlterColumn("int.stockIndices", "continent", c => c.String());
            AlterColumn("int.stockIndices", "index_name", c => c.String());
            AlterColumn("int.stockIndices", "symbol", c => c.String(nullable: false, maxLength: 10));
            AlterColumn("int.prices", "ticker", c => c.String(nullable: false, maxLength: 8));
            AlterColumn("int.json", "ClassT", c => c.String());
            AlterColumn("int.json", "Url", c => c.String());
            AlterColumn("app.indexComponents", "StockTicker", c => c.String(maxLength: 10));
            AlterColumn("app.indexComponents", "IndexSymbol", c => c.String(maxLength: 20));
            AlterColumn("int.securities", "financial_status", c => c.String());
            AlterColumn("int.securities", "market_category", c => c.String());
            AlterColumn("int.securities", "listing_exchange", c => c.String());
            AlterColumn("int.securities", "stock_exchange", c => c.String());
            AlterColumn("int.securities", "security_type", c => c.String());
            AlterColumn("int.securities", "security_name", c => c.String());
            AlterColumn("int.securities", "companyTicker", c => c.String(maxLength: 128));
            AlterColumn("int.securities", "ticker", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("int.companyDetails", "industry_group", c => c.String());
            AlterColumn("int.companyDetails", "industry_category", c => c.String());
            AlterColumn("int.companyDetails", "sector", c => c.String());
            AlterColumn("int.companyDetails", "inc_state", c => c.String());
            AlterColumn("int.companyDetails", "hq_state", c => c.String());
            AlterColumn("int.companyDetails", "template", c => c.String());
            AlterColumn("int.companyDetails", "cik", c => c.String());
            AlterColumn("int.companyDetails", "company_url", c => c.String());
            AlterColumn("int.companyDetails", "stock_exchange", c => c.String());
            AlterColumn("int.companyDetails", "ticker", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("int.companies", "template", c => c.String());
            AlterColumn("int.companies", "name", c => c.String());
            AlterColumn("int.companies", "cik", c => c.String());
            AlterColumn("int.companies", "ticker", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("rss.servers", "HostName", c => c.String(maxLength: 40));
            AlterColumn("app.instruments", "StockExchange", c => c.String(maxLength: 10));
            AlterColumn("app.instruments", "Ticker", c => c.String(maxLength: 30));
            AlterColumn("rss.feeds", "Links", c => c.String());
            AlterColumn("rss.feeds", "Language", c => c.String());
            AlterColumn("rss.feeds", "Url", c => c.String());
            AddPrimaryKey("int.stockIndices", "symbol");
            AddPrimaryKey("int.prices", new[] { "ticker", "date" });
            AddPrimaryKey("int.securities", "ticker");
            AddPrimaryKey("int.companyDetails", "ticker");
            AddPrimaryKey("int.companies", "ticker");
            CreateIndex("app.indexComponents", "StockTicker");
            CreateIndex("app.indexComponents", "IndexSymbol");
            CreateIndex("int.securities", "companyTicker");
            CreateIndex("rss.servers", "HostName");
            CreateIndex("app.instruments", "StockExchange");
            CreateIndex("app.instruments", "Ticker");
            AddForeignKey("int.securities", "companyTicker", "int.companyDetails", "ticker");
        }
    }
}
