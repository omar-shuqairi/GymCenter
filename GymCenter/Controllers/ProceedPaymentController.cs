using GymCenter.Models;
using GymCenter.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Numerics;

namespace GymCenter.Controllers
{
    public class ProceedPaymentController : Controller
    {
        private readonly ModelContext _context;
        public ProceedPaymentController(ModelContext context)
        {
            _context = context;

        }
        public async Task<IActionResult> Index(int PlanId)
        {
            var selectedPlan = await _context.Workoutplans.FirstOrDefaultAsync(p => p.Planid == PlanId);
            TempData["PlanID"] = selectedPlan.Planid.ToString();
            TempData["Durationinmonths"] = selectedPlan.Durationinmonths.ToString();
            return View(selectedPlan);
        }
        [HttpPost]
        public async Task<IActionResult> Index([Bind("Cardnumber,Cvv,Expirydate")] Creditcard MemberCreditCard, decimal amount)
        {

            int PlanID = Convert.ToInt32(TempData["PlanID"]);
            int Durationinmonths = Convert.ToInt32(TempData["Durationinmonths"]);
            int? memberUserId = HttpContext.Session.GetInt32("MemberuserId");
            var dbCreditCard = await _context.Creditcards.FirstOrDefaultAsync(c => c.Cardnumber == MemberCreditCard.Cardnumber);
            if (dbCreditCard == null)
            {
                return NotFound("Credit card not found.");
            }
            if (dbCreditCard.Balance < amount)
            {
                return BadRequest("Insufficient balance.");
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
                return NotFound("Credit card not found.");
            }
            member.SubscriptionStart = DateTime.Now;
            member.SubscriptionEnd = DateTime.Now.AddMonths(Durationinmonths);
            member.Planid = PlanID;
            _context.Members.Update(member);
            await _context.SaveChangesAsync();
            return RedirectToAction("Home", "Member");
        }

    }
}
