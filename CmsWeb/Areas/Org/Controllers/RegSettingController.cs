using System;
using System.Globalization;
using System.Threading;
using System.Web.Mvc;
using CmsData;
using RegistrationSettingsParser;
using UtilityExtensions;

namespace CmsWeb.Areas.Org.Controllers
{
    [ValidateInput(false)]
    [RouteArea("Org", AreaPrefix="RegSettings"), Route("{action=index}/{id?}")]
    public class RegSettingController : CmsStaffController
    {
        [HttpGet, Route("~/RegSettings/{id:int}")]
        public ActionResult Index(int id)
        {
            var org = DbUtil.Db.LoadOrganizationById(id);
            var regsetting = (string)TempData["regsetting"];
            if (!regsetting.HasValue())
                regsetting = org.RegSetting;

            ViewData["lines"] = Parser.SplitLines(regsetting);
            ViewData["regsetting"] = regsetting;
            ViewData["OrganizationId"] = id;
            ViewData["orgname"] = org.OrganizationName;
            return View();
        }
        [HttpPost]
        [Authorize(Roles="Edit")]
        public ActionResult Edit(int id, string regsetting)
        {
            var org = DbUtil.Db.LoadOrganizationById(id);
            ViewData["OrganizationId"] = id;
            ViewData["orgname"] = org.OrganizationName;
            if (regsetting.HasValue())
                ViewData["text"] = regsetting;
            else
                ViewData["text"] = org.RegSetting;
            return View();
        }

        [HttpPost]
        [Authorize(Roles="Edit")]
        public ActionResult Update(int id, string text)
        {
            var org = DbUtil.Db.LoadOrganizationById(id);
            try
            {
                var os = DbUtil.Db.CreateRegistrationSettings(text, id);
                org.RegSetting = text;
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                TempData["regsetting"] = text;
                return Redirect("/RegSettings/" + id);
            }
            DbUtil.Db.SubmitChanges();
            return Redirect("/RegSettings/" + id);
        }
        [HttpGet]
        [Authorize(Roles="Edit")]
        public ActionResult EditXml(int id)
        {
            var org = DbUtil.Db.LoadOrganizationById(id);
            ViewData["OrganizationId"] = id;
            ViewData["orgname"] = org.OrganizationName;
            var os = DbUtil.Db.CreateRegistrationSettings(id);
            var x = Util.Serialize(os);
            ViewData["text"] = x;
            return View();
        }

//        [HttpPost]
//        [Authorize(Roles="Edit")]
//        public ActionResult UpdateXml(int id, string text)
//        {
//            var org = DbUtil.Db.LoadOrganizationById(id);
//            try
//            {
//                var os = Settings.CreateSettings(text, DbUtil.Db, id);
//                org.RegSetting = text;
//            }
//            catch (Exception ex)
//            {
//                TempData["error"] = ex.Message;
//                TempData["regsetting"] = text;
//                return Redirect("/RegSettings/" + id);
//            }
//            DbUtil.Db.SubmitChanges();
//            return Redirect("/RegSettings/" + id);
//        }
        public ActionResult ConvertFromMdy(int id)
        {
            var cul = "en-US";
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(cul);
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(cul);
            var org = DbUtil.Db.LoadOrganizationById(id);
            var m = DbUtil.Db.CreateRegistrationSettings(id);
            var os = DbUtil.Db.CreateRegistrationSettings(m.ToString(), id);
            m.org.RegSetting = os.ToString();
            DbUtil.Db.SubmitChanges();
            return Redirect("/RegSettings/" + id);
        }
        public ActionResult ConvertFromDmy(int id)
        {
            var cul = "en-GB";
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(cul);
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(cul);
            var org = DbUtil.Db.LoadOrganizationById(id);
            var m = DbUtil.Db.CreateRegistrationSettings(id);
            var os = DbUtil.Db.CreateRegistrationSettings(m.ToString(), id);
            m.org.RegSetting = os.ToString();
            DbUtil.Db.SubmitChanges();
            return Redirect("/RegSettings/" + id);
        }
    }
}