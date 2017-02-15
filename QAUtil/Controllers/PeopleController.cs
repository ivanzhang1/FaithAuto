using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Text;

namespace QAUtil.Controllers
{
    public class PeopleController : Controller
    {
        //
        // GET: /People/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult InsertCommValue(FormCollection form) {
            StringBuilder query = new StringBuilder("DECLARE ");
            query.AppendFormat("@householdID int = {0} ", form["hsdID"]);
            query.AppendFormat("@individualID int = {0} ", form["indID"]);
            query.AppendFormat("@churchID int = {0} ", form["churchID"]);
            query.AppendFormat("@commTypeID int = {0} ", form["commType"]);
            query.AppendFormat("@commValue varchar(200) = {0} ", form["commValue"]);
            query.Append("begin ");
            query.Append("insert into [ChmPeople].[dbo].[HOUSEHOLD_COMMUNICATION]([HOUSEHOLD_ID],[INDIVIDUAL_ID],[CHURCH_ID],[COMMUNICATION_TYPE_ID],[COMMUNICATION_VALUE],[ISPRIMARY]) ");
            query.Append("values (@householdID,@individualID,@churchID,@commTypeID,@commValue,0) ");
            query.Append("end");
            
            Common.ExecuteDBQuery(form["dataSource"], query.ToString());

            return RedirectToAction("Index");
        }

    }
}
