namespace SyndicateLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class nn_tables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "nn.trainSchema",
                c => new
                    {
                        NetworkID = c.Int(nullable: false),
                        NeuronID = c.Int(nullable: false),
                        Input = c.Boolean(nullable: false),
                        ShingleID = c.Int(),
                        ForecastDays = c.Int(),
                    })
                .PrimaryKey(t => new { t.NetworkID, t.NeuronID });
            
            CreateTable(
                "nn.trainValue",
                c => new
                    {
                        NetworkID = c.Int(nullable: false),
                        NeuronID = c.Int(nullable: false),
                        ArticleID = c.Int(nullable: false),
                        Value = c.Single(nullable: false),
                    })
                .PrimaryKey(t => new { t.NetworkID, t.NeuronID, t.ArticleID });
            
        }
        
        public override void Down()
        {
            DropTable("nn.trainValue");
            DropTable("nn.trainSchema");
        }
    }
}
