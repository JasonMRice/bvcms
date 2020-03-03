using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CmsWeb.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using UtilityExtensions;
using System.Web.Mvc;
using Rectangle = iTextSharp.text.Rectangle;
using CmsWeb.Lifecycle;

namespace CmsWeb.Areas.Reports.Models
{
    public class RollLabelsResult : ActionResult
    {
        public Guid qid { get; set; }
        public string format { get; set; }
        public bool titles { get; set; }
        public bool useMailFlags { get; set; }
        public bool usephone { get; set; }

        public bool? sortzip;
        public string sort
        {
            get { return (sortzip ?? false) ? "Zip" : "Name"; }
        }

        protected float H = .925f;
        protected float W = 3f;

        protected PdfContentByte dc;
        private Font font = FontFactory.GetFont(FontFactory.HELVETICA, 10);
        private Font smfont = FontFactory.GetFont(FontFactory.HELVETICA, 8);
        private IRequestManager _requestManager;
        public IRequestManager RequestManager
        {
            get => _requestManager;
            set=> _requestManager = value;             
        }

        public RollLabelsResult(IRequestManager requestManager)
        {
            RequestManager = requestManager;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            var Response = context.HttpContext.Response;
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", $"filename={format}-Labels.pdf");

            var document = new Document();
            document.SetPageSize(new Rectangle(72 * W, 72 * H));
            document.SetMargins(14f, 0f, 3.6f, 1f);
            var w = PdfWriter.GetInstance(document, Response.OutputStream);
            document.Open();
            dc = w.DirectContent;

            var ctl = new MailingController(RequestManager) {UseTitles = titles, UseMailFlags = useMailFlags};

            IEnumerable<MailingController.MailingInfo> q = null;
            switch (format)
            {
                case "Individual":
                case "GroupAddress":
                    q = ctl.FetchIndividualList(sort, qid);
                    break;
                case "FamilyMembers":
                case "Family":
                    q = ctl.FetchFamilyList(sort, qid);
                    break;
                case "ParentsOf":
                    q = ctl.FetchParentsOfList(sort, qid);
                    break;
                case "CouplesEither":
                    q = ctl.FetchCouplesEitherList(sort, qid);
                    break;
                case "CouplesBoth":
                    q = ctl.FetchCouplesBothList(sort, qid);
                    break;
            }
            AddLabel(document, "=========", $"{Util.UserName}\n{q.Count()},{DateTime.Now:g}", String.Empty);
            foreach (var m in q)
            {
                var label = m.LabelName;
                if (m.CoupleName.HasValue() && format.StartsWith("Couples"))
                    label = m.CoupleName;
                var address = "";
                if (m.MailingAddress.HasValue())
                    address = m.MailingAddress;
                else
                {
                    var sb = new StringBuilder(m.Address);
                    if (m.Address2.HasValue())
                        sb.AppendFormat("\n{0}", m.Address2);
                    sb.AppendFormat("\n{0}", m.CSZ);
                    address = sb.ToString();
                }
                AddLabel(document, label, address, Util.PickFirst(m.CellPhone.FmtFone("C "), m.HomePhone.FmtFone("H ")));
            }
            document.Close();
        }

        public void AddLabel(Document d, string name, string address, string phone)
        {
            var t2 = new PdfPTable(1);
            t2.WidthPercentage = 100f;
            t2.DefaultCell.Border = PdfPCell.NO_BORDER;

            var c2 = new PdfPCell(new Phrase(name, font));
            c2.Border = PdfPCell.NO_BORDER;
            if (usephone)
            {
                var nt = new PdfPTable(new float[] { 20f, 10f });
                nt.WidthPercentage = 100f;
                nt.DefaultCell.Padding = 0;
                nt.DefaultCell.Border = PdfPCell.NO_BORDER;
                c2.Padding = 0.0F;
                nt.AddCell(c2);
                c2 = new PdfPCell(new Phrase(phone, smfont));
                c2.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
                c2.Border = PdfPCell.NO_BORDER;
                c2.Padding = 0.0F;
                c2.PaddingRight = 3F;
                nt.AddCell(c2);
                t2.AddCell(nt);
            }
            else
                t2.AddCell(c2);

            foreach (var line in address.SplitLines())
            {
                var cc = new PdfPCell(new Phrase(line, font));
                cc.Border = PdfPCell.NO_BORDER;
                t2.AddCell(cc);
            }

            d.Add(t2);
            d.NewPage();
        }

    }
}

