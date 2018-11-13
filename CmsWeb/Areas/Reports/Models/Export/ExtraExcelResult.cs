using System;
using System.Web.Mvc;
using System.Xml.Linq;
using UtilityExtensions;
using System.Linq;
using CmsData;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Data.SqlClient;

namespace CmsWeb.Models
{
    public class ExtraValueExcelResult
    {
        public static EpplusResult ExtraValueExcel(Guid qid)
        {
            var tag = CurrentDatabase.PopulateSpecialTag(qid, DbUtil.TagTypeId_ExtraValues);

            var roles = CMSRoleProvider.provider.GetRolesForUser(Util.UserName);
            var xml = XDocument.Parse(CurrentDatabase.Content("StandardExtraValues2", "<Fields/>"));
            var fields = (from ff in xml.Root.Descendants("Value")
                          let vroles = ff.Attribute("VisibilityRoles")
                          where vroles != null && vroles.Value.HasValue() && (vroles.Value.Split(',').All(rr => !roles.Contains(rr)))
                          select ff.Attribute("Name").Value);
            var nodisplaycols = string.Join("|", fields);

            var cmd = new SqlCommand("dbo.ExtraValues @p1, @p2, @p3");
			cmd.Parameters.AddWithValue("@p1", tag.Id);
			cmd.Parameters.AddWithValue("@p2", "");
			cmd.Parameters.AddWithValue("@p3", nodisplaycols);
            cmd.Connection = new SqlConnection(Util.ConnectionString);
            cmd.Connection.Open();

            return cmd.ExecuteReader().ToExcel("ExtraExcelResult.xlsx");
        }
    }
}