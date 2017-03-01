using System.Data.Entity.Migrations;

namespace SyndicateLogic.Migrations
{
    public partial class companyDetails : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CompanyDetails",
                c => new
                    {
                        ticker = c.String(nullable: false, maxLength: 128),
                        name = c.String(),
                        valuation_active = c.Boolean(nullable: false),
                        legal_name = c.String(),
                        stock_exchange = c.String(),
                        sic = c.Int(nullable: false),
                        short_description = c.String(),
                        long_description = c.String(),
                        ceo = c.String(),
                        company_url = c.String(),
                        business_address = c.String(),
                        mailing_address = c.String(),
                        business_phone_no = c.String(),
                        cik = c.String(),
                        template = c.String(),
                        standardized_active = c.Boolean(nullable: false),
                        hq_state = c.String(),
                        hq_country = c.String(),
                        inc_state = c.String(),
                        inc_country = c.String(),
                        employees = c.Int(nullable: false),
                        sector = c.String(),
                        industry_category = c.String(),
                        industry_group = c.String()
                    })
                .PrimaryKey(t => t.ticker);
            
            CreateTable(
                "dbo.Securities",
                c => new
                    {
                        ticker = c.String(nullable: false, maxLength: 128),
                        security_name = c.String(),
                        security_type = c.String(),
                        stock_exchange = c.String(),
                        listing_exchange = c.String(),
                        market_category = c.String(),
                        etf = c.Boolean(nullable: false),
                        round_lot_size = c.Int(nullable: false),
                        financial_status = c.String(),
                        primary_security = c.Boolean(nullable: false),
                        delisted_security = c.Boolean(nullable: false),
                        last_crsp_adj_date = c.DateTime(nullable: false)
                    })
                .PrimaryKey(t => t.ticker);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Securities");
            DropTable("dbo.CompanyDetails");
        }
    }
}
