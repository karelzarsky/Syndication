namespace SyndicateLogic.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class shingleActionColumns : DbMigration
    {
        public override void Up()
        {
            AddColumn("fact.shingleAction", "down10", c => c.Int(nullable: false));
            AddColumn("fact.shingleAction", "down09", c => c.Int(nullable: false));
            AddColumn("fact.shingleAction", "down08", c => c.Int(nullable: false));
            AddColumn("fact.shingleAction", "down07", c => c.Int(nullable: false));
            AddColumn("fact.shingleAction", "down06", c => c.Int(nullable: false));
            AddColumn("fact.shingleAction", "down05", c => c.Int(nullable: false));
            AddColumn("fact.shingleAction", "down04", c => c.Int(nullable: false));
            AddColumn("fact.shingleAction", "down03", c => c.Int(nullable: false));
            AddColumn("fact.shingleAction", "down02", c => c.Int(nullable: false));
            AddColumn("fact.shingleAction", "down01", c => c.Int(nullable: false));
            AddColumn("fact.shingleAction", "down00", c => c.Int(nullable: false));
            AddColumn("fact.shingleAction", "up00", c => c.Int(nullable: false));
            AddColumn("fact.shingleAction", "up01", c => c.Int(nullable: false));
            AddColumn("fact.shingleAction", "up02", c => c.Int(nullable: false));
            AddColumn("fact.shingleAction", "up03", c => c.Int(nullable: false));
            AddColumn("fact.shingleAction", "up04", c => c.Int(nullable: false));
            AddColumn("fact.shingleAction", "up05", c => c.Int(nullable: false));
            AddColumn("fact.shingleAction", "up06", c => c.Int(nullable: false));
            AddColumn("fact.shingleAction", "up07", c => c.Int(nullable: false));
            AddColumn("fact.shingleAction", "up08", c => c.Int(nullable: false));
            AddColumn("fact.shingleAction", "up09", c => c.Int(nullable: false));
            AddColumn("fact.shingleAction", "up10", c => c.Int(nullable: false));
            AlterColumn("rss.feeds", "Url", c => c.String(maxLength: 300, unicode: false));
            AlterColumn("app.investorRSS", "Url", c => c.String(nullable: false, maxLength: 300, unicode: false));
            CreateIndex("rss.feeds", "Url", unique: true);
            CreateIndex("app.investorRSS", "Url", unique: true);
        }
        
        public override void Down()
        {
            DropIndex("app.investorRSS", new[] { "Url" });
            DropIndex("rss.feeds", new[] { "Url" });
            AlterColumn("app.investorRSS", "Url", c => c.String(nullable: false, maxLength: 8000, unicode: false));
            AlterColumn("rss.feeds", "Url", c => c.String(maxLength: 8000, unicode: false));
            DropColumn("fact.shingleAction", "up10");
            DropColumn("fact.shingleAction", "up09");
            DropColumn("fact.shingleAction", "up08");
            DropColumn("fact.shingleAction", "up07");
            DropColumn("fact.shingleAction", "up06");
            DropColumn("fact.shingleAction", "up05");
            DropColumn("fact.shingleAction", "up04");
            DropColumn("fact.shingleAction", "up03");
            DropColumn("fact.shingleAction", "up02");
            DropColumn("fact.shingleAction", "up01");
            DropColumn("fact.shingleAction", "up00");
            DropColumn("fact.shingleAction", "down00");
            DropColumn("fact.shingleAction", "down01");
            DropColumn("fact.shingleAction", "down02");
            DropColumn("fact.shingleAction", "down03");
            DropColumn("fact.shingleAction", "down04");
            DropColumn("fact.shingleAction", "down05");
            DropColumn("fact.shingleAction", "down06");
            DropColumn("fact.shingleAction", "down07");
            DropColumn("fact.shingleAction", "down08");
            DropColumn("fact.shingleAction", "down09");
            DropColumn("fact.shingleAction", "down10");
        }
    }
}
