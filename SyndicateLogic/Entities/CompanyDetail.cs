using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SyndicateLogic.Entities
{
    [Table("int.companyDetails")]
    public class CompanyDetail
    {
        [Key, Column(TypeName = "varchar")]
        public string ticker { get; set; }  // the ticker market ticker symbol associated with the companies common ticker securities
        [MaxLength(100), Index]
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
        public virtual List<Security> securities { get; set; }
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
}