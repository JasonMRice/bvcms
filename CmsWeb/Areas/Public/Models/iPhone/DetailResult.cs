using System;
using System.Collections.Generic;
using System.Xml;
using System.Web.Mvc;
using System.Xml.Linq;
using UtilityExtensions;
using System.Linq;
using CmsData;

namespace CmsWeb.Models.iPhone
{
    public class DetailResult : ActionResult
    {
        private int PeopleId;
        public DetailResult(int PeopleId)
        {
            this.PeopleId = PeopleId;
        }
        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.ContentType = "text/xml";
            var settings = new XmlWriterSettings();
            settings.Encoding = new System.Text.UTF8Encoding(false);
            settings.Indent = true;

            using (var w = XmlWriter.Create(context.HttpContext.Response.OutputStream, settings))
            {
                var p = CurrentDatabase.LoadPersonById(PeopleId);
                w.WriteStartElement("Person");
                w.WriteAttributeString("peopleid", p.PeopleId.ToString());
                w.WriteAttributeString("first", p.PreferredName);
                w.WriteAttributeString("last", p.LastName);
                w.WriteAttributeString("address", p.PrimaryAddress);
                w.WriteAttributeString("citystatezip", p.CityStateZip);
                w.WriteAttributeString("zip", p.PrimaryZip);
                w.WriteAttributeString("age", Person.AgeDisplay(p.Age, p.PeopleId).ToString());
                w.WriteAttributeString("birthdate", p.BirthDate.FormatDate());
                w.WriteAttributeString("homephone", p.HomePhone);
                w.WriteAttributeString("cellphone", p.CellPhone);
                w.WriteAttributeString("workphone", p.WorkPhone);
                w.WriteAttributeString("memberstatus", p.MemberStatus.Description);
                w.WriteAttributeString("email", p.EmailAddress);
                w.WriteAttributeString("haspicture", p.PictureId.HasValue ? "1" : "0");

                foreach (var m in p.Family.People.Where(mm => mm.PeopleId != PeopleId && mm.DeceasedDate == null))
                {
                    w.WriteStartElement("OtherMember");
                    w.WriteAttributeString("peopleid", m.PeopleId.ToString());
                    w.WriteAttributeString("name", m.Name);
                    w.WriteAttributeString("address", m.PrimaryAddress);
                    w.WriteAttributeString("citystatezip", m.CityStateZip);
                    w.WriteAttributeString("age", Person.AgeDisplay(m.Age, m.PeopleId).ToString());
                    w.WriteEndElement();
                }
                var q = from re in CurrentDatabase.RelatedFamilies
                        where re.FamilyId == p.FamilyId || re.RelatedFamilyId == p.FamilyId
                        let rf = re.RelatedFamilyId == p.FamilyId ? re.RelatedFamily1 : re.RelatedFamily2
                        select new { hohid = rf.HeadOfHouseholdId, description = re.FamilyRelationshipDesc };
                foreach (var rf in q)
                {
                    w.WriteStartElement("RelatedFamily");
                    w.WriteAttributeString("hohid", rf.hohid.ToString());
                    w.WriteAttributeString("description", rf.description);
                    w.WriteEndElement();
                }
                w.WriteEndElement();
            }
        }
    }
}
