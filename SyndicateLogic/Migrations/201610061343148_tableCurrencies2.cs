namespace SyndicateLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tableCurrencies2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("app.currencies", "Entity", c => c.String(maxLength: 100));
            AlterColumn("app.currencies", "CurrencyName", c => c.String(maxLength: 100));
            AlterColumn("app.currencies", "MinorUnit", c => c.Byte(nullable: false));
            CreateIndex("app.currencies", "Entity");
            CreateIndex("app.currencies", "CurrencyName");
            DropColumn("app.currencies", "WithdrawalDate");
        }
        
        public override void Down()
        {
            AddColumn("app.currencies", "WithdrawalDate", c => c.String());
            DropIndex("app.currencies", new[] { "CurrencyName" });
            DropIndex("app.currencies", new[] { "Entity" });
            AlterColumn("app.currencies", "MinorUnit", c => c.String());
            AlterColumn("app.currencies", "CurrencyName", c => c.String());
            AlterColumn("app.currencies", "Entity", c => c.String());
        }
    }
}
