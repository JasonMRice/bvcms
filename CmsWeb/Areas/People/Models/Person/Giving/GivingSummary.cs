﻿namespace CmsWeb.Areas.People.Models
{
    public class GivingSummary
    {
        public int? FundOnlineSort { get; set; }
        public int FundId { get; set; }
        public string Fund { get; set; }
        public decimal AmountContributed { get; set; }
    }
}
