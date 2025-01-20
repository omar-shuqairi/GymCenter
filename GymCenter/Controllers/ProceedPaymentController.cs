using GymCenter.Models;
using GymCenter.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Numerics;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using System.IO;
using System.Net;
using System.Net.Mail;
namespace GymCenter.Controllers
{
    public class ProceedPaymentController : Controller
    {
        private readonly ModelContext _context;
        private readonly EmailService _emailService;
        public ProceedPaymentController(ModelContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }
        public async Task<IActionResult> Index(int PlanId)
        {
            var selectedPlan = await _context.Workoutplans.FirstOrDefaultAsync(p => p.Planid == PlanId);
            return View(selectedPlan);
        }
        [HttpPost]
        public async Task<IActionResult> Index([Bind("Cardnumber,Cvv,Expirydate")] Creditcard MemberCreditCard, decimal amount, int PlanID, decimal Durationinmonths)
        {
            string MemberEmail = HttpContext.Session.GetString("MemberEmail");
            int? memberUserId = HttpContext.Session.GetInt32("MemberuserId");
            var dbCreditCard = await _context.Creditcards.FirstOrDefaultAsync(c => c.Cardnumber == MemberCreditCard.Cardnumber);
            if (dbCreditCard == null || dbCreditCard.Cvv != MemberCreditCard.Cvv || dbCreditCard.Expirydate.Value.ToShortDateString() != MemberCreditCard.Expirydate.Value.ToShortDateString())
            {
                TempData["ErrorCard"] = "Credit card not found!";
                return RedirectToAction(nameof(Index), new { PlanId = PlanID });
            }
            if (dbCreditCard.Balance < amount)
            {
                TempData["ErrorBalance"] = "Insufficient balance!";
                return RedirectToAction(nameof(Index), new { PlanId = PlanID });
            }
            dbCreditCard.Balance -= amount;
            _context.Creditcards.Update(dbCreditCard);
            await _context.SaveChangesAsync();

            var payment = new Payment
            {
                Paymentdate = DateTime.Now,
                Amountpaid = amount,
                Userid = memberUserId,
            };
            await _context.Payments.AddAsync(payment);
            await _context.SaveChangesAsync();
            var member = await _context.Members.FirstOrDefaultAsync(m => m.Userid == memberUserId);
            if (member == null)
            {
                return NotFound("Member not found.");
            }
            member.SubscriptionStart = DateTime.Now;
            member.SubscriptionEnd = DateTime.Now.AddMonths((int)Durationinmonths);

            member.Planid = PlanID;
            _context.Members.Update(member);
            await _context.SaveChangesAsync();

            var userlogin = await _context.UserLogins
                .Where(u => u.Userid == memberUserId)
                .FirstOrDefaultAsync();

            var user = await _context.Users
                .Where(u => u.Userid == memberUserId)
                .FirstOrDefaultAsync();
            var plan = await _context.Workoutplans
               .Where(u => u.Planid == PlanID)
               .FirstOrDefaultAsync();

            byte[] pdfBytes = GenerateInvoicePdf(payment, member, userlogin, user, plan);
            var invoice = new Invoice
            {
                Userid = memberUserId,
                Paymentid = payment.Paymentid,
                Pdfdata = pdfBytes,
                Createddate = DateTime.Now
            };
            await _context.Invoices.AddAsync(invoice);
            await _context.SaveChangesAsync();
            string subject = "Your Gym Center Invoice";
            string body = "<p>Thank you for your payment. Please find your invoice attached.</p>";
            await _emailService.SendEmailAsync(MemberEmail, subject, body, pdfBytes, "Invoice.pdf");
            TempData["SuccessfulPayment"] = "Your payment was successful! Thank you for your subscribing!";
            return RedirectToAction("MyPlan", "Member");
        }
        private byte[] GenerateInvoicePdf(Payment payment, Member member, UserLogin userlogin, User user, Workoutplan plan)
        {
            using (var ms = new MemoryStream())
            {
                var writer = new PdfWriter(ms);
                var pdf = new PdfDocument(writer);
                var document = new Document(pdf);


                var boldFont = iText.Kernel.Font.PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA_BOLD);
                var regularFont = iText.Kernel.Font.PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA);


                var title = new Paragraph("Invoice")
                    .SetFont(boldFont)
                    .SetFontSize(24)
                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                    .SetMarginBottom(20);
                document.Add(title);

                var lineSeparator = new iText.Layout.Element.LineSeparator(new iText.Kernel.Pdf.Canvas.Draw.SolidLine());
                lineSeparator.SetMarginBottom(20);
                document.Add(lineSeparator);


                var memberInfoTable = new Table(2).UseAllAvailableWidth();
                memberInfoTable.AddCell(new Cell().Add(new Paragraph("Member Name:").SetFont(boldFont).SetFontSize(12)));
                memberInfoTable.AddCell(new Cell().Add(new Paragraph($"{user.Fname} {user.Lname}").SetFont(regularFont).SetFontSize(12)));
                memberInfoTable.AddCell(new Cell().Add(new Paragraph("Username:").SetFont(boldFont).SetFontSize(12)));
                memberInfoTable.AddCell(new Cell().Add(new Paragraph(userlogin.Username).SetFont(regularFont).SetFontSize(12)));
                memberInfoTable.AddCell(new Cell().Add(new Paragraph("Email:").SetFont(boldFont).SetFontSize(12)));
                memberInfoTable.AddCell(new Cell().Add(new Paragraph(user.Email).SetFont(regularFont).SetFontSize(12)));
                document.Add(memberInfoTable.SetMarginBottom(20));


                var paymentInfoTable = new Table(2).UseAllAvailableWidth();
                paymentInfoTable.AddCell(new Cell().Add(new Paragraph("Invoice ID:").SetFont(boldFont).SetFontSize(12)));
                paymentInfoTable.AddCell(new Cell().Add(new Paragraph(payment.Paymentid.ToString()).SetFont(regularFont).SetFontSize(12)));
                paymentInfoTable.AddCell(new Cell().Add(new Paragraph("Amount Paid:").SetFont(boldFont).SetFontSize(12)));
                paymentInfoTable.AddCell(new Cell().Add(new Paragraph($"{payment.Amountpaid:C}").SetFont(regularFont).SetFontSize(12)));
                paymentInfoTable.AddCell(new Cell().Add(new Paragraph("Payment Date:").SetFont(boldFont).SetFontSize(12)));
                paymentInfoTable.AddCell(new Cell().Add(new Paragraph(payment.Paymentdate.Value.ToString("g")).SetFont(regularFont).SetFontSize(12)));
                document.Add(paymentInfoTable.SetMarginBottom(20));


                var subscriptionInfoTable = new Table(2).UseAllAvailableWidth();
                subscriptionInfoTable.AddCell(new Cell().Add(new Paragraph("Subscription Start:").SetFont(boldFont).SetFontSize(12)));
                subscriptionInfoTable.AddCell(new Cell().Add(new Paragraph(member.SubscriptionStart.Value.ToShortDateString()).SetFont(regularFont).SetFontSize(12)));
                subscriptionInfoTable.AddCell(new Cell().Add(new Paragraph("Subscription End:").SetFont(boldFont).SetFontSize(12)));
                subscriptionInfoTable.AddCell(new Cell().Add(new Paragraph(member.SubscriptionEnd.Value.ToShortDateString()).SetFont(regularFont).SetFontSize(12)));
                document.Add(subscriptionInfoTable.SetMarginBottom(20));

                var planTable = new Table(2).UseAllAvailableWidth();
                planTable.AddCell(new Cell(1, 2).Add(new Paragraph("Workout Plan Details").SetFont(boldFont).SetFontSize(14).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)).SetBackgroundColor(iText.Kernel.Colors.ColorConstants.LIGHT_GRAY));
                planTable.AddCell(new Cell().Add(new Paragraph("Plan Name:").SetFont(boldFont).SetFontSize(12)));
                planTable.AddCell(new Cell().Add(new Paragraph(plan.Planname).SetFont(regularFont).SetFontSize(12)));
                planTable.AddCell(new Cell().Add(new Paragraph("Exercise Routines:").SetFont(boldFont).SetFontSize(12)));
                planTable.AddCell(new Cell().Add(new Paragraph(plan.ExerciseRoutines).SetFont(regularFont).SetFontSize(12)));
                planTable.AddCell(new Cell().Add(new Paragraph("Plan Goals:").SetFont(boldFont).SetFontSize(12)));
                planTable.AddCell(new Cell().Add(new Paragraph(plan.Goals).SetFont(regularFont).SetFontSize(12)));
                document.Add(planTable.SetMarginBottom(20));

                var footer = new Paragraph("Thank you for choosing Our Gym !")
                    .SetFont(boldFont)
                    .SetFontSize(12)
                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                    .SetMarginTop(30);
                document.Add(footer);

                document.Close();
                return ms.ToArray();
            }
        }

    }
}
