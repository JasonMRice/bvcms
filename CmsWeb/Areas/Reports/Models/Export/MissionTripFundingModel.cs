using CmsData;
using CmsData.View;
using CmsWeb.Areas.Search.Models;
using CmsWeb.Constants;
using OfficeOpenXml;
using OfficeOpenXml.Table;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CmsWeb.Models
{
    public class MissionTripFundingModel : IDbBinder
    {
        public CMSDataContext CurrentDatabase { get; set; }

        [Obsolete(Errors.ModelBindingConstructorError, error: true)]
        public MissionTripFundingModel() { }

        public MissionTripFundingModel(CMSDataContext db)
        {
            CurrentDatabase = db;
        }

        public static List<MissionTripTotal> List(int id, CMSDataContext db)
        {
            var q = from t in db.ViewMissionTripTotals
                    where t.OrganizationId == id
                    orderby t.OrganizationId, t.SortOrder
                    select t;
            return q.ToList();
        }

        public static List<MissionTripTotal> List(OrgSearchModel m)
        {
            var orgids = string.Join(",", m.FetchOrgs().Select(mm => mm.OrganizationId));
            var q = from t in m.CurrentDatabase.ViewMissionTripTotals
                    join i in m.CurrentDatabase.SplitInts(orgids) on t.OrganizationId equals i.ValueX
                    orderby t.OrganizationId, t.SortOrder
                    select t;
            return q.ToList();
        }

        public static decimal TotalDue(CMSDataContext db, int? pid, int? oid)
        {
            var tt = (from t in db.ViewMissionTripTotals
                      where t.PeopleId == pid && t.OrganizationId == oid
                      select t).SingleOrDefault();
            if (tt == null)
            {
                return 0;
            }

            return tt.Due ?? 0;
        }
        public static EpplusResult Result(OrgSearchModel m)
        {
            var q = List(m);
            var cols = typeof(MissionTripTotal).GetProperties();
            var count = q.Count;
            var ep = new ExcelPackage();
            var ws = ep.Workbook.Worksheets.Add("Sheet1");
            if (count == 0)
            {
                ws.Cells["A1"].Value = "nothing to report";
                return new EpplusResult(ep, "MissionTripFunding.xlsx");
            }
            ws.Cells["A2"].LoadFromCollection(q);
            var range = ws.Cells[1, 1, count + 1, cols.Length];
            var table = ws.Tables.Add(range, "Trips");
            table.TableStyle = TableStyles.Light9;
            table.ShowFilter = false;
            for (var i = 0; i < cols.Length; i++)
            {
                var col = i + 1;
                var name = cols[i].Name;
                table.Columns[i].Name = name;
                var colrange = ws.Cells[1, col, count + 2, col];
            }
            ws.Cells[ws.Dimension.Address].AutoFitColumns();
            return new EpplusResult(ep, "MissionTripFunding.xlsx");
        }
    }
}
