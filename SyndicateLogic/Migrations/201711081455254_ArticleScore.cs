namespace SyndicateLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ArticleScore : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("fact.articleScore");
            AddColumn("fact.articleScore", "ModelNr", c => c.Int(nullable: false));
            AddPrimaryKey("fact.articleScore", new[] { "ModelNr", "ArticleID", "Interval" });
        }
        
        public override void Down()
        {
            DropPrimaryKey("fact.articleScore");
            DropColumn("fact.articleScore", "ModelNr");
            AddPrimaryKey("fact.articleScore", new[] { "articleID", "interval" });
        }
    }
}
