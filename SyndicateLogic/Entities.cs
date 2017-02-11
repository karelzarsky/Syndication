using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SyndicateLogic.Entities
{
    [Table("app.currencies")]
    public class Currency
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Index, MaxLength(100)]
        public string Entity { get; set; }
        [Index, MaxLength(100)]
        public string CurrencyName { get; set; }
        [Index, MaxLength(3), Column(TypeName = "varchar")]
        public string AplhabeticCode { get; set; }
        public Int16? NumericCode { get; set; }
        public byte MinorUnit { get; set; }
    }

    [Table("app.instruments")]
    public class Instrument
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Index, MaxLength(30), Column(TypeName = "varchar")]
        public string Ticker { get; set; }
        [Index, MaxLength(10), Column(TypeName = "varchar")]
        public string StockExchange { get; set; }
        [Index]
        public InstrumentType Type { get; set; }
        [Index]
        public DateTime? LastPriceUpdate { get; set; }
    }

    [Table("app.instrumentIdentifiers")]
    public class InstrumentIdentifier
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Index, ForeignKey("Instrument")]
        public int InstrumentID { get; set; }
        public virtual Instrument Instrument { get; set; }
        [Index, MaxLength(30)]
        public string Text { get; set; }
    }

    [Table("app.logs")]
    public class Log
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Index]
        public DateTime Time { get; set; } = DateTime.Now;
        [Index]
        public byte Severity { get; set; }
        public string Message { get; set; }
    }

    [Table("app.indexComponents")]
    public class indexComponent
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Index, MaxLength(20), Column(TypeName = "varchar")]
        public string IndexSymbol { get; set; }
        [Index, MaxLength(10), Column(TypeName = "varchar")]
        public string StockTicker { get; set; }
    }

    [Table("app.companyNames")]
    public class CompanyName
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Index, MaxLength(100)]
        public string name { get; set; }
        public string ticker { get; set; }
    }

    [Table("app.stockTickers")]
    public class StockTicker
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Index, MaxLength(20), Column(TypeName = "varchar")]
        public string ticker { get; set; }
    }

    [Table("app.commonWords")]
    public class CommonWord
    {
        [Required, MaxLength(2), Key, Column(TypeName = "varchar", Order = 0)]
        public string language { get; set; }
        [Required, MaxLength(50), Key, Column(Order = 1)]
        public string text { get; set; }
    }

    [Table("fact.shingleAction")]
    public class shingleAction
    {
        [Key, Column(Order = 0), Required, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int shingleID { get; set; }
        [Key, Column(Order = 1), Required, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public byte interval { get; set; }
        [Column(TypeName = "smallmoney")]
        public decimal? down { get; set; }
        [Column(TypeName = "smallmoney")]
        public decimal? up { get; set; }
        [Column(TypeName = "Date")]
        public DateTime dateComputed { get; set; }
        public int samples { get; set; }
        public int tickers { get; set; }
    }

    [Table("fact.articleScore")]
    public class ArticleScore
    {
        [Key, Column(Order = 0), Required, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int articleID { get; set; }
        [Key, Column(Order = 1), Required, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public byte interval { get; set; }
        [Column(TypeName = "smallmoney")]
        public decimal? score { get; set; }
        [Column(TypeName = "Date")]
        public DateTime dateComputed { get; set; }
    }

    [Table("fact.alerts")]
    public class Alert
    {
        [Key, Column(Order = 0, TypeName = "varchar"), Required, DatabaseGenerated(DatabaseGeneratedOption.None), MaxLength(10)]
        public string ticker { get; set; } // Stock market ticker symbol associated with the companies common ticker securities
        [Index]
        [Key, Column(Order = 1), Required, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public DateTime issued { get; set; }
        public decimal scoreMin { get; set; }
        public decimal scoreMax { get; set; }
        [ForeignKey("Article")]
        public int ArticleID { get; set; }
        public virtual Article Article { get; set; }
    }

    [Table("int.companies")]
    public class Company
    {
        [Key, Column(TypeName = "varchar")]
        public string ticker { get; set; } // Stock market ticker symbol associated with the companies common ticker securities
        [Column(TypeName = "varchar")]
        public string cik { get; set; } // Central Index Key issued by the SEC
        [Column(TypeName = "nvarchar"), Index, MaxLength(100)]
        public string name { get; set; } // short form
        [Column(TypeName = "varchar")]
        public string template { get; set; }
        public bool standardized_active { get; set; }
        public bool valuation_active { get; set; }
    }

    public class CompaniesResponse
    {
        public int result_count { get; set; }
        public int page_size { get; set; }
        public int current_page { get; set; }
        public int total_pages { get; set; }
        public int api_call_credits { get; set; }
        public Company[] data { get; set; }
    }

    [Table("int.companyDetails")]
    public class CompanyDetail
    {
        [Key, Column(TypeName = "varchar")]
        public string ticker { get; set; }  // the ticker market ticker symbol associated with the companies common ticker securities
        [MaxLength(100),Index]
        public string name { get; set; }  // the company name in shorter form
        public bool valuation_active { get; set; } // if true, the company is currently available on the intrinio valuation webapp, otherwise it is not available.
        [Index, MaxLength(100)]
        public string legal_name { get; set; } // the company's official legal name
        [Column(TypeName = "varchar")]
        public string stock_exchange { get; set; } // the Stock Exchange where the company's common ticker is primarily traded
        public int? sic { get; set; } // the Standard Industrial Classification (SIC) determined by the company filed with the SEC 
        public string short_description { get; set; } // a one or two sentance description of the company's operations
        public string long_description { get; set; }  // a one paragraph description of the company's operations and other corporate actions
        [Index, MaxLength(50)]
        public string ceo { get; set; } // the Chief Executive Officer of the company
        [Column(TypeName = "varchar")]
        public string company_url { get; set; }
        public string business_address { get; set; }
        public string mailing_address { get; set; }
        public string business_phone_no { get; set; }
        public object lei { get; set; }
        public object hq_address1 { get; set; }
        public object hq_address2 { get; set; }
        public object hq_address_city { get; set; }
        public object hq_address_postal_code { get; set; }
        public object entity_legal_form { get; set; }
        public virtual ICollection<Security> securities { get; set; }
        [Column(TypeName = "varchar")]
        public string cik { get; set; } // the Central Index Key issued by the SEC
        [Column(TypeName = "varchar")]
        public string template { get; set; }  // the financial statement template used by Intrinio to standardize the as reported data
        public bool standardized_active { get; set; } // if true, the company has standardized and as reported fundamental data via the Intrinio API; if false, the company has as reported data only.
        [Column(TypeName = "varchar")]
        public string hq_state { get; set; } // the state (US & Canada Only) where the company headquarters is located
        public string hq_country { get; set; } // the country where the company headquarters is located
        [Column(TypeName = "varchar")]
        public string inc_state { get; set; } // the state (US & Canada Only) where the company is incorporated
        public string inc_country { get; set; } // the country where the company is incorporated
        public int? employees { get; set; }
        public object entity_status { get; set; }
        [Column(TypeName = "varchar")]
        public string sector { get; set; }
        [Column(TypeName = "varchar")]
        public string industry_category { get; set; }
        [Column(TypeName = "varchar")]
        public string industry_group { get; set; }
    }

    [Table("int.calls")]
    public class IntrinioAPICalls
    {
        public DateTime Date { get; set; }
        public bool CutOff { get; set; }
        public int NumberOfCalls { get; set; }
        public DateTime LastTry { get; set; }
        public string LastError { get; set; }
    }

    [Table("int.json")]
    public class IntrinioJson
    {
        public IntrinioJson()
        {
            time = DateTime.Now;
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime time { get; set; }
        [Column(TypeName = "varchar")]
        public string Url { get; set; }
        [Column(TypeName = "varchar")]
        public string ClassT { get; set; }
        public byte[] Compressed { get; set; }
        public string Json()
        { return DataLayer.Unzip(Compressed); }
    }

    [Table("int.prices")]
    public class Price
    {
        [Column(Order = 0, TypeName = "varchar"), Key, MaxLength(8)]
        public string ticker { get; set; }
        [Column(Order = 1, TypeName = "Date"), Key]
        public DateTime date { get; set; }
        [Column(TypeName = "smallmoney")]
        public decimal open { get; set; }
        [Column(TypeName = "smallmoney")]
        public decimal high { get; set; }
        [Column(TypeName = "smallmoney")]
        public decimal low { get; set; }
        [Column(TypeName = "smallmoney")]
        public decimal close { get; set; }
        public long? volume { get; set; }
        [Column(TypeName = "smallmoney")]
        public decimal? ex_dividend { get; set; }
        [Column(TypeName = "smallmoney")]
        public decimal? split_ratio { get; set; }
        [Column(TypeName = "smallmoney")]
        public decimal? adj_open { get; set; }
        [Column(TypeName = "smallmoney")]
        public decimal? adj_high { get; set; }
        [Column(TypeName = "smallmoney")]
        public decimal? adj_low { get; set; }
        [Column(TypeName = "smallmoney")]
        public decimal? adj_close { get; set; }
        public long? adj_volume { get; set; }
    }

    public class PricesResponse
    {
        public Price[] data { get; set; }
        public int result_count { get; set; }
        public int page_size { get; set; }
        public int current_page { get; set; }
        public int total_pages { get; set; }
        public int api_call_credits { get; set; }
    }

    [Table("int.securities")]
    public class Security
    {
        [Key, Column(TypeName = "varchar")]
        public string ticker { get; set; }
        [ForeignKey("CompanyDetail"), Column(TypeName = "varchar")]
        public string companyTicker { get; set; }
        public virtual CompanyDetail CompanyDetail { get; set; }
        [Column(TypeName = "varchar")]
        public string security_name { get; set; }  // the type of security, such as Common Stock, Preferred Stock, Warrants, Limited Partnership Interests, etc.
        [Column(TypeName = "varchar")]
        public string security_type { get; set; }
        public object security_class { get; set; }
        [Column(TypeName = "varchar")]
        public string stock_exchange { get; set; } // the Stock Exchange (and market category) where the company's common ticker is primarily traded
        [Column(TypeName = "varchar")]
        public string listing_exchange { get; set; } // the Stock Exchange where the company's common ticker is primarily traded
        [Column(TypeName = "varchar")]
        public string market_category { get; set; }
        public bool etf { get; set; } // true or false, depending on whether the security is an ETF or not.
        public int round_lot_size { get; set; }
        [Column(TypeName = "varchar")]
        public string financial_status { get; set; } // indicates when an issuer has failed to submit its regulatory filings on a timely basis, has failed to meet NASDAQ's continuing listing standards, and/or has filed for bankruptcy. Values include: D = Deficient: Issuer Failed to Meet NASDAQ Continued Listing Requirements, E = Delinquent: Issuer Missed Regulatory Filing Deadline, Q = Bankrupt: Issuer Has Filed for Bankruptcy, N = Normal (Default): Issuer Is NOT Deficient, Delinquent, or Bankrupt., G = Deficient and Bankrupt, H = Deficient and Delinquent, J = Delinquent and Bankrupt, K = Deficient, Delinquent, and Bankrupt
        public bool primary_security { get; set; }  // true if the subject security is the primary security for the company. If a security is the default, the ticker symbol for the security is the same as the ticker for the company (see /Companies endpoint).
        public bool delisted_security { get; set; }  // if the security is no longer traded on public exchanges, the security will be considered delisted and the security no longer will report pricing data.
        public DateTime? last_crsp_adj_date { get; set; } // the last recorded date ("YYYY-MM-DD") of an CRSP adjustment made to prior prices due to a ticker split or dividend event.
    }

    [Table("int.stockIndices")]
    public class StockIndex
    {
        [Key, MaxLength(10), Column(TypeName = "varchar")]
        public string symbol { get; set; }
        [Column(TypeName = "varchar")]
        public string index_name { get; set; }
        [Column(TypeName = "varchar")]
        public string continent { get; set; }
        [Column(TypeName = "varchar")]
        public string country { get; set; }
    }

    public class StockIndicesResponse
    {
        public StockIndex[] data { get; set; }
        public int result_count { get; set; }
        public int page_size { get; set; }
        public int current_page { get; set; }
        public int total_pages { get; set; }
        public int api_call_credits { get; set; }
    }

    [Table("rss.articles")]
    public class Article
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [ForeignKey("Feed")]
        [Index]
        public int FeedID { get; set; }
        public virtual Feed Feed { get; set; }
        [Index, MaxLength(500)]
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Categories { get; set; }
        [Index]
        public DateTime PublishedUTC { get; set; }
        public DateTime ReceivedUTC { get; set; }
        [Index]
        public ProcessState Processed { get; set; }

        public int ComputeHash()
        { return Text().ToLower().GetHashCode(); }

        public string Text()
        { return Title + " | " + Summary; }
    }

    [Table("rss.articleRelations")]
    public class ArticleRelation
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Index, ForeignKey("Article")]
        [Index("IX_ArticleAndInstrument", 1, IsUnique = true)]
        public int ArticleID { get; set; }
        public virtual Article Article { get; set; }
        [Index, ForeignKey("Instrument")]
        [Index("IX_ArticleAndInstrument", 2, IsUnique = true)]
        public int InstrumentID { get; set; }
        public virtual Instrument Instrument { get; set; }
    }

    [Table("rss.feeds")]
    public class Feed
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public DateTime? LastCheck { get; set; }
        public DateTime? LastArticleReceived { get; set; }
        public DateTime? LastError { get; set; }
        public bool Active { get; set; }
        [Column(TypeName = "varchar")]
        public string Url { get; set; }
        public string Title { get; set; }
        public string ErrorMessage { get; set; }
        [Column(TypeName = "varchar")]
        public string Language { get; set; }
        public string Categories { get; set; }
        [Column(TypeName = "varchar")]
        public string Links { get; set; }
        //[Index, ForeignKey("RSSServer")]
        //public int RSSServerID { get; set; }
        public virtual RSSServer RSSServer { get; set; }
        public virtual Instrument AffectedInstrument { get; set; }
        public string HostName()
        {
            Uri myUri = new Uri(Url);
            return myUri.Host;
        }
    }

    [Table("rss.servers")]
    public class RSSServer
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Index, MaxLength(40)]
        [Column(TypeName = "varchar")]
        public string HostName { get; set; }
        [Index]
        public DateTime? LastCheck { get; set; }
        public int Interval { get; set; } = 15;
        [Index]
        public DateTime? NextRun { get; set; }
        public int Errors { get; set; }
        public int Success { get; set; }
    }

    [Table("rss.shingles")]
    public class Shingle
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Index]
        public byte tokens { get; set; }
        [Index]
        public ShingleKind kind { get; set; }
        [Required, MaxLength(200), Index]
        public string text { get; set; }
        [Required, MaxLength(2), Index, Column(TypeName = "varchar")]
        public string language { get; set; }
        [Index]
        public DateTime? LastRecomputeDate { get; set; }
    }

    [Table("rss.shingleUse")]
    public class ShingleUse
    {
        [Key, Column(Order = 0), Required, ForeignKey("Shingle"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ShingleID { get; set; }
        public virtual Shingle Shingle { get; set; }
        [Key, Column(Order = 1), Required, ForeignKey("Article"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ArticleID { get; set; }
        public virtual Article Article { get; set; }
    }
}
