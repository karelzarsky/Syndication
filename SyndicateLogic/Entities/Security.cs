using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SyndicateLogic.Entities
{
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
}
