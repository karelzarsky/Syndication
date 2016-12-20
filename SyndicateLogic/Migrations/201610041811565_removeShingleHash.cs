namespace SyndicateLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removeShingleHash : DbMigration
    {
        public override void Up()
        {
            DropIndex("rss.shingles", new[] { "hash" });
            DropColumn("rss.shingles", "hash");
        }
        
        public override void Down()
        {
            AddColumn("rss.shingles", "hash", c => c.Int(nullable: false));
            CreateIndex("rss.shingles", "hash");
        }
    }
}
