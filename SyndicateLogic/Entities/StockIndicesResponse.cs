namespace SyndicateLogic.Entities
{
    public class StockIndicesResponse
    {
        public StockIndex[] data { get; set; }
        public int result_count { get; set; }
        public int page_size { get; set; }
        public int current_page { get; set; }
        public int total_pages { get; set; }
        public int api_call_credits { get; set; }
    }
}
