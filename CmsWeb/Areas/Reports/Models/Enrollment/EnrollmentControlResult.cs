using System;
using iTextSharp.text;
using iTextSharp.text.pdf;
using UtilityExtensions;
using System.Web.Mvc;
using CmsWeb.Areas.Search.Models;

namespace CmsWeb.Areas.Reports.Models
{
    public class EnrollmentControlResult : ActionResult
    {
        public bool UseCurrentTag { get; set; }

        public OrgSearchModel OrgSearch { get; set; }

        public override void ExecuteResult(ControllerContext context)
        {
            var Response = context.HttpContext.Response;


            string scheduletext = string.Empty;
            var sdt = CmsData.Organization.GetDateFromScheduleId(OrgSearch.ScheduleId ?? 0);
            if (sdt.HasValue)
                scheduletext = sdt.Value.ToString("dddd h:mm tt");

            var headtext = $"Enrollment Control Report {scheduletext}";
            var filename = headtext.SlugifyString("-", false);
            Response.Clear();
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", $"filename={filename}.pdf");
            var doc = new Document(PageSize.LETTER, 36, 36, 36, 42);
            var w = PdfWriter.GetInstance(doc, Response.OutputStream);
            w.PageEvent = new HeadFoot(headtext);

            var boldfont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 8);

            doc.Open();

            var t = new PdfPTable(4);
            t.HeaderRows = 1;
            t.WidthPercentage = 100;
            t.SetWidths(new int[] { 20, 30, 10, 15 });

            var font = FontFactory.GetFont(FontFactory.HELVETICA, 8);
            t.AddCell(new Phrase("Name", boldfont));
            t.AddCell(new Phrase("Organization", boldfont));
            t.AddCell(new Phrase("Location", boldfont));
            t.AddCell(new Phrase("Member Type", boldfont));

            foreach (var m in EnrollmentControlModel.List(OrgSearch, usecurrenttag:UseCurrentTag))
            {
                t.AddCell(new Phrase(m.Name, font));
                t.AddCell(new Phrase(m.Organization, font));
                t.AddCell(new Phrase(m.Location, font));
                t.AddCell(new Phrase(m.MemberType, font));
            }
            if (t.Rows.Count > 1)
                doc.Add(t);
            else
                doc.Add(new Phrase("no data"));
            doc.Close();
        }
        class HeadFoot : PdfPageEventHelper
        {
            private PdfTemplate tpl;
            private PdfContentByte dc;
            private BaseFont font;
            private string sText;

            public HeadFoot(string headertext)
            {
                sText = headertext;
            }

            public override void OnOpenDocument(PdfWriter writer, Document document)
            {
                base.OnOpenDocument(writer, document);
                font = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                dc = writer.DirectContent;
                tpl = dc.CreateTemplate(50, 50);
            }
            public override void OnEndPage(PdfWriter writer, Document document)
            {
                base.OnEndPage(writer, document);

                float fLen;

                //---Column 1: Title
                fLen = font.GetWidthPoint(sText, 8);
                dc.BeginText();
                dc.SetFontAndSize(font, 8);
                dc.SetTextMatrix(30, 30);
                dc.ShowText(sText);
                dc.EndText();

                //---Column 2: Date/Time
                sText = Util.Now.ToShortDateString();
                fLen = font.GetWidthPoint(sText, 8);
                dc.BeginText();
                dc.SetFontAndSize(font, 8);
                dc.SetTextMatrix(document.PageSize.Width / 2 - fLen / 2, 30);
                dc.ShowText(sText);
                dc.EndText();

                //---Column 3: Page Number
                sText = "Page " + writer.PageNumber + " of ";
                fLen = font.GetWidthPoint(sText, 8);
                dc.BeginText();
                dc.SetFontAndSize(font, 8);
                dc.SetTextMatrix(document.PageSize.Width - 90, 30);
                dc.ShowText(sText);
                dc.EndText();
                dc.AddTemplate(tpl, document.PageSize.Width - 90 + fLen, 30);
            }
            public override void OnCloseDocument(PdfWriter writer, Document document)
            {
                tpl.BeginText();
                tpl.SetFontAndSize(font, 8);
                tpl.ShowText((writer.PageNumber - 1).ToString());
                tpl.EndText();
                base.OnCloseDocument(writer, document);
            }
        }
    }
}

