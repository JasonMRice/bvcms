using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using UtilityExtensions;
using System.Web.UI.WebControls;
using System.Web.UI;
using CmsData;
using CmsData.API;
using Dapper;
using MoreLinq;

namespace CmsWeb.Models
{
    public class ContributionsExcelResult
    {
        public DateTime Dt1 { get; set; }
        public DateTime Dt2 { get; set; }
        public int fundid { get; set; }
    	public int campusid { get; set; }
    	public int Online { get; set; }
        public string TaxDedNonTax { get; set; }
        public bool IncUnclosedBundles { get; set; }
        public bool FilterByActiveTag { get; set; }
        public string FundSet { get; set; }

        public EpplusResult ToExcel(string type)
        {
            bool? nontaxdeductible = null; // both
            switch (TaxDedNonTax)
            {
                case "TaxDed":
                    nontaxdeductible = false;
                    break;
                case "NonTaxDed":
                    nontaxdeductible = true;
                    break;
            }

            var tagid = FilterByActiveTag ? DbUtil.Db.TagCurrent()?.Id : (int?)null;
            var fundids = APIContributionSearchModel.GetCustomFundSetList(DbUtil.Db, FundSet);
            var funds = fundids.JoinInts(",");

            switch (type)
            {
                case "ledgerincome":
                    var cd = new CommandDefinition("dbo.LedgerIncomeExport", new
                    {
                        fd = Dt1,
                        td = Dt2,
                        campusid,
                        nontaxded = nontaxdeductible,
                        includeunclosed = IncUnclosedBundles
                    }, commandType: CommandType.StoredProcedure);
                    return DbUtil.Db.Connection.ExecuteReader(cd).ToExcel("LedgerIncome.xlsx");
                case "donorfundtotals":
    				return ExportPeople.ExcelDonorFundTotals(Dt1, Dt2, fundid, campusid, false, nontaxdeductible, IncUnclosedBundles, tagid, funds)
                        .ToExcel("DonorFundTotals.xlsx");
                case "donortotals":
                    return ExportPeople.ExcelDonorTotals(Dt1, Dt2, campusid, false, nontaxdeductible, IncUnclosedBundles, tagid, funds)
                        .ToExcel("DonorTotals.xlsx");
                case "donordetails":
                    return ExportPeople.DonorDetails(Dt1, Dt2, fundid, campusid, false, nontaxdeductible, IncUnclosedBundles, tagid, funds)
                        .ToExcel("DonorDetails.xlsx");
            }
            return null;
        }
    }
}
