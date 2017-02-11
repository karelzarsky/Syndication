using System;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using SyndicateLogic.Entities;

namespace SyndicateLogic
{
    public class Db : DbContext
    {
        public Db() : base(ReadConnectionString("SyndicateDb"))
        {
        }

        public Db(string connectionString)
            : base(connectionString)
        {
        }

        public DbSet<Company> Companies { get; set; }
        public DbSet<CompanyDetail> CompanyDetails { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<IntrinioJson> IntrinioJsons { get; set; }
        public DbSet<Feed> Feeds { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<Price> Prices { get; set; }
        public DbSet<Security> Securities { get; set; }
        public DbSet<Shingle> Shingles { get; set; }
        public DbSet<ShingleUse> ShingleUses { get; set; }
        public DbSet<StockIndex> StockIndicesIntrinio { get; set; }
        public DbSet<Instrument> Instruments { get; set; }
        public DbSet<InstrumentIdentifier> InstrumentIdentifiers { get; set; }
        public DbSet<ArticleRelation> ArticleRelations { get; set; }
        public DbSet<indexComponent> IndexComponents { get; set; }
        public DbSet<RSSServer> RssServers { get; set; }
        public DbSet<shingleAction> ShingleActions { get; set; }
        public DbSet<CompanyName> CompanyNames { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<StockTicker> StockTickers { get; set; }
        public DbSet<CommonWord> CommonWords { get; set; }
        public DbSet<ArticleScore> ArticleScores { get; set; }
        public DbSet<Alert> Alerts { get; set; }

        public static string ReadConnectionString(string key)
        {
            var res = "";
            try
            {
                var cs = ConfigurationManager.ConnectionStrings[key];
                if (cs != null)
                    res = cs.ToString();
            }
            catch (ConfigurationErrorsException e)
            {
                if (Environment.UserInteractive)
                    Console.WriteLine("Error reading app settings. " + e);
            }
            return res;
        }

        public static void InicializeDb(Db db) // Don't need when using migrations
        {
            string dbCreationScript = ((IObjectContextAdapter) db).ObjectContext.CreateDatabaseScript();
            db.Database.ExecuteSqlCommand(dbCreationScript);
            db.SaveChanges();
        }
    }
}