using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Data.SqlClient;
using System.Data.Sql;
using System.Data;
using System.Text;

namespace QAUtil.Controllers {
    [Authorize]
    public class GivingController : Controller {
        //
        // GET: /Giving/

        public ActionResult Index() {
            return View();
        }

        public ActionResult Batches_RemoteDepositCapture() {
            return View();
        }

        public ActionResult MockUpRDCBatch(FormCollection form) {
            StringBuilder query = new StringBuilder("DECLARE @batchName VARCHAR(50) ");
            query.Append("DECLARE @amount MONEY, @totalAmount MONEY ");
            query.Append("DECLARE @batchId INT, @churchID INT, @ppMerchantAccountID INT, @locationID INT, @referenceImageID INT, @batchItemCount INT, @index INT ");
            query.AppendFormat("SET @amount = {0} ", form["batchItemAmount"]);
            query.AppendFormat("SET @churchID = {0} ", form["churchId"]);
            query.AppendFormat("SET @batchName = '{0}' ", form["batchName"]);
            query.AppendFormat("SET @batchItemCount = '{0}' ", form["batchItemCount"]);
            query.Append("SET @totalAmount = @amount * @batchItemCount ");
            query.Append("SET @index = 0 ");
            query.Append("SET @ppMerchantAccountID = (SELECT TOP 1 PP_MERCHANT_ACCOUNT_ID FROM ChmContribution.dbo.PP_MERCHANT_ACCOUNT WITH (NOLOCK) WHERE CHURCH_ID = @churchID AND PP_MERCHANT_ACCOUNT_CODE = 'ftone') ");
            query.Append("SET @locationID = (SELECT TOP 1 Location_ID FROM ChmContribution.dbo.PP_ProfitStars_Merchant_Location WITH (NOLOCK) WHERE Church_ID = @churchID) ");
            query.Append("SET @referenceImageID = (SELECT TOP 1 reference_image_id FROM ChmContribution.dbo.REFERENCE_IMAGE WITH (NOLOCK) WHERE church_Id = @churchID) ");
            query.Append("INSERT INTO ChmContribution.dbo.BATCH (CHURCH_ID, BATCH_NAME, BATCH_DATE, BATCH_AMOUNT, BatchStatusID, BatchTypeID) ");
            query.Append("VALUES(@churchID, @batchName, CURRENT_TIMESTAMP, @totalAmount, 4, 3) ");
            query.Append("SET @batchId = (SELECT SCOPE_IDENTITY()) ");
            query.Append("INSERT INTO ChmContribution.Pmt.RDCBatch (ChurchID, BatchID, BatchName, BatchCreatedDate, ItemCount, BatchAmount, PPMerchantAccountID, LocationID) ");
            query.Append("VALUES(@churchID, @batchId, @batchName, CURRENT_TIMESTAMP, 1, @totalAmount, @ppMerchantAccountID, @locationID) ");
            query.Append("WHILE @index < @batchItemCount ");
            query.Append("BEGIN ");
            query.Append("INSERT INTO ChmContribution.Pmt.RDCBatchItem (ChurchID, BatchID, ItemCreatedDate, AccountNumber, RoutingNumber, Amount, ReferenceImageID, ReferenceNumber, PPMerchantAccountID, LocationID, CreatedDate, CheckNumber) ");
            query.Append("VALUES(@churchID, @batchId, CURRENT_TIMESTAMP, '1234567890', '111000025', @amount, @referenceImageID, 'T:65H8W0X7w', @ppMerchantAccountID, @locationID, CURRENT_TIMESTAMP, 1099) ");
            query.Append("SET @index = @index + 1 ");
            query.Append("END");
            Common.ExecuteDBQuery(form["dataSource"], query.ToString());

            return RedirectToAction("Index");
        }

        public ActionResult BatchesDelete(int batchTypeID) {
            ViewData["batchTypeID"] = batchTypeID;
            ViewData["Message"] = batchTypeID == 1 ? "Delete regular batches and contribution receipts if they exist." : "Delete Remote Deposit Capture batches and contributions if they exist";
            return View();
        }

        public ActionResult DeleteBatches(bool deleteAccounts, FormCollection form) {
            int churchID = Convert.ToInt16(form["churchID"]);
            string fromDate = form["fromDate"].ToString();
            int batchTypeID = Convert.ToInt16(form["batchTypeID"]);

            StringBuilder query = new StringBuilder("CREATE TABLE #Batches(ID INT IDENTITY(1,1), BatchID INT, processed INT DEFAULT 0) ");
            query.Append("DECLARE @churchID INT, @batchID INT, @batchTypeID INT, @startid INT, @endid INT, @date DATETIME ");
            query.AppendFormat("SET @churchID = {0} ", churchID);
            query.AppendFormat("SET @date = '{0}' ", fromDate);
            query.AppendFormat("SET @batchTypeID = '{0}' ", batchTypeID);
            query.Append("INSERT INTO #Batches (BatchID) SELECT BATCH_ID FROM ChmContribution.dbo.BATCH WITH (NOLOCK) WHERE CHURCH_ID = @churchID AND BATCH_DATE > @date AND BatchTypeID = @batchTypeID ORDER BY BATCH_DATE DESC ");
            query.Append("SET @startid = (SELECT MIN(ID) FROM #Batches WHERE processed = 0) ");
            query.Append("SET @endid = (SELECT MAX(ID) FROM #Batches WHERE processed = 0) ");
            query.Append("WHILE (@startid <= @endid) ");
            query.Append("BEGIN ");
            query.Append("SELECT @batchID = BatchID FROM #Batches WHERE ID = @startid ");
            query.Append("DELETE FROM ChmContribution.dbo.CONTRIBUTION_RECEIPT WHERE CHURCH_ID = @churchID AND BATCH_ID = @batchID ");

            if (batchTypeID == 3) {
                query.Append("DELETE FROM ChmContribution.Pmt.RDCBatchItemDetail WHERE ChurchID = @churchID AND BatchID = @batchID ");
                query.Append("DELETE FROM ChmContribution.Pmt.RDCBatchItem WHERE ChurchID = @churchID AND BatchID = @batchID ");
                query.Append("DELETE FROM ChmContribution.Pmt.RDCBatch WHERE ChurchID = @churchID AND BatchID = @batchID ");
            }

            query.Append("DELETE FROM ChmContribution.dbo.BATCH WHERE CHURCH_ID = @churchID AND BATCH_ID = @batchID ");
            query.Append("UPDATE #Batches SET processed = 1 WHERE id = @startid ");
            query.Append("SET @startid = @startid + 1 ");
            query.Append("END ");
            query.Append("DROP TABLE #Batches ");

            if (deleteAccounts) {
                query.AppendFormat("DELETE c_r FROM ChmContribution.dbo.CONTRIBUTION_RECEIPT c_r WITH (NOLOCK) INNER JOIN ChmContribution.dbo.ACCOUNT ac WITH (NOLOCK) ON c_r.ACCOUNT_ID = ac.ACCOUNT_ID WHERE c_r.CHURCH_ID = {0} AND ac.ACCOUNT = '{1}' AND ac.ROUTING_NO = '{2}'", churchID, form["accountNumber"], form["routingNumber"]);
                query.AppendFormat("DELETE FROM ChmContribution.dbo.ACCOUNT WHERE CHURCH_ID = {0} AND ACCOUNT = '{1}' AND ROUTING_NO = '{2}'", churchID, form["accountNumber"], form["routingNumber"]);
            }

            Common.ExecuteDBQuery(form["dataSource"], query.ToString());
            return RedirectToAction("Index");
        }

        public ActionResult DeleteContributionReceipts() {
            //DELETE FROM ChmContribution.dbo.CONTRIBUTION_RECEIPT WHERE CHURCH_ID = 15 AND INDIVIDUAL_ID = (SELECT TOP 1 INDIVIDUAL_ID FROM ChmPeople.dbo.INDIVIDUAL WHERE CHURCH_ID = 15 AND INDIVIDUAL_NAME = 'Matthew Sneeden') AND BATCH_ID IS NULL AND MEMO IS NULL
            return View("Index");
        }

        public ActionResult PaymentTypes(string dataSource, string churchId, string feature = "Online Giving 1.0") {
            int church = Convert.ToInt32(churchId);
            int featureID = feature == "Online Giving 1.0" ? 6 : 10;
            ViewData["dataSource"] = dataSource;
            ViewData["churchID"] = church;

            var dataContext = new PaymentTypesDataContext(Common.FetchDBConnectionString(dataSource));
            var paymentTypes = from types in dataContext.PP_TYPEs
                               orderby types.PP_TYPE_ID
                               join payTypes in dataContext.PP_CHURCH_TYPE_PROCESSORs on new { types.PP_TYPE_ID, f1 = church, f2 = featureID} equals new {payTypes.PP_TYPE_ID, f1 = payTypes.CHURCH_ID, f2 = payTypes.FEATURE_ID} into mod
                               from x in mod.DefaultIfEmpty()

                               select new QAUtil.Models.PaymentTypes {
                                   ENABLED = x.PP_TYPE_ID != null ? true : false,
                                   PP_TYPE_ID = types.PP_TYPE_ID,
                                   PaymentType = types.PP_TYPE_NAME,
                                   FeatureID = featureID
                               };

            return View(paymentTypes);
        }

        public ActionResult UpdatePaymentTypes(List<QAUtil.Models.PaymentTypes> grid, FormCollection form) {
            int churchId = Convert.ToInt32(form["church"]);
            int featureID = grid[0].FeatureID;
            ViewData["churchId"] = churchId;
            ViewData["dataSource"] = form["dataSource"];
            var dataContext = new PaymentTypesDataContext(Common.FetchDBConnectionString(form["dataSource"]));
            foreach (var item in grid) {
                bool enabled = item.ENABLED;
                int pp_type_id = item.PP_TYPE_ID;

                // Enable or disable the module
                if (enabled) {
                    if (!dataContext.PP_CHURCH_TYPE_PROCESSORs.Any(c => c.PP_TYPE_ID == item.PP_TYPE_ID && c.CHURCH_ID == churchId && c.FEATURE_ID == item.FeatureID)) {
                        PP_CHURCH_TYPE_PROCESSOR cm = new PP_CHURCH_TYPE_PROCESSOR() {
                            CHURCH_ID = churchId,
                            PP_TYPE_ID = item.PP_TYPE_ID,
                            FEATURE_ID = item.FeatureID,
                            CREATED_DATE = DateTime.Now,
                            LAST_UPDATED_DATE = DateTime.Now,
                            CREATED_BY_LOGIN = HttpContext.User.Identity.Name,
                            PP_Merchant_Provider_ID = item.PP_TYPE_ID == 19 ? 2 : 1
                        };
                        dataContext.PP_CHURCH_TYPE_PROCESSORs.InsertOnSubmit(cm);
                        dataContext.SubmitChanges();
                    }
                }
                else {
                    if (dataContext.PP_CHURCH_TYPE_PROCESSORs.Any(c => c.PP_TYPE_ID == item.PP_TYPE_ID && c.CHURCH_ID == churchId && c.FEATURE_ID == item.FeatureID)) {
                        dataContext.PP_CHURCH_TYPE_PROCESSORs.DeleteOnSubmit(dataContext.PP_CHURCH_TYPE_PROCESSORs.Where(cm => cm.PP_TYPE_ID == item.PP_TYPE_ID && cm.CHURCH_ID == churchId && cm.FEATURE_ID == featureID).Select(cm => cm).Single());
                        dataContext.SubmitChanges();
                    }
                }
            }
            return RedirectToAction("PaymentTypes", new { dataSource = ViewData["dataSource"], churchId = ViewData["churchId"], feature = featureID == 6 ? "Online Giving 1.0" : "Online Giving 2.0" });
        }

        public ActionResult Payments(FormCollection form) {
            int churchID = Convert.ToInt32(form["churchID"]);

            //SELECT TOP 50 stat.PaymentStatusName, pay.HouseholdID, pay.IndividualID, pay.Amount, rea.ReasonCode, rea.Description FROM Payment pay WITH (NOLOCK)
            //INNER JOIN ProviderRequest req WITH (NOLOCK)
            //ON pay.PaymentID = req.PaymentID
            //INNER JOIN PaymentReason rea WITH (NOLOCK)
            //ON rea.PaymentReasonID = req.PaymentReasonID
            //INNER JOIN PaymentStatus stat WITH (NOLOCK)
            //ON pay.PaymentStatusID = stat.PaymentStatusID
            //WHERE pay.ChurchID = 15 ORDER BY pay.CreatedDate DESC

            var dataContext = new PaymentsDataContext(Common.FetchDBConnectionString(form["dataSource"]));
            var payments = from pay in dataContext.Payments.Where(c => c.ChurchID == churchID).OrderByDescending(c => c.CreatedDate).Take(20)
                           join req in dataContext.ProviderRequests on pay.PaymentID equals req.PaymentID
                           join rea in dataContext.PaymentReasons on req.PaymentReasonID equals rea.PaymentReasonID// into payData
                           join stat in dataContext.PaymentStatus on pay.PaymentStatusID equals stat.PaymentStatusID into payData
                           from final in payData
                           select new QAUtil.Models.Payments {
                               CreatedDate = pay.CreatedDate.ToString(),
                               PaymentStatusName = final.PaymentStatusName,
                               HouseholdID = pay.HouseholdID,
                               IndividualID = pay.IndividualID,
                               Amount = string.Format("{0:c}", pay.Amount),
                               ReasonCode = rea.ReasonCode,
                               Description = rea.Description,
                               ClientApplication = pay.ClientApplication
                           };

            return View(payments);
        }

        public PartialViewResult Render(string submitValue) {
            ViewData["submitValue"] = submitValue;
            return PartialView("../../Views/Shared/DataSourceChurchId");
        }
    }
}