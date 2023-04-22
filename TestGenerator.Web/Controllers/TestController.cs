using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestGenerator.DAL.Models;
using TestGenerator.Web.Repositories;

namespace TestGenerator.Web.Controllers;

public class TestController : Controller
{
    private readonly ITestRepository _testRepository;

    public TestController(ITestRepository testRepository)
    {
        _testRepository = testRepository;
    }

    public async Task<IActionResult> Index()
    {
        var tests = await _testRepository.GetTestsAsync();
        return View(tests);
    }

    [HttpGet]
    public async Task<IActionResult> Add()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(Test test)
    {
        if (!ModelState.IsValid)
        {
            return View(test);
        }

        ViewBag.NumberOfQuestions = test.NumberOfQuestions;
        ViewBag.NumberOfAnswersPerQuestion = test.NumberOfAnswersPerQuestion;

        return View(nameof(AddTest));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddTest(Test test)
    {
        if (!ModelState.IsValid)
        {
            return View(test);
        }

        await _testRepository.AddTestAsync(test);

        return RedirectToAction(nameof(Index));
    }

    //// GET: Customers/Details/5
    //public async Task<IActionResult> Details(int? id, int? orderId)
    //{
    //  if (id == null)
    //  {
    //    return NotFound();
    //  }

    //  var viewModel = new CustomerIndexData();
    //  viewModel.Customer = await _context.Customers
    //    .Include(c => c.Address)
    //    .Include(c => c.Orders)
    //    .ThenInclude(o => o.OrderItems)
    //    .ThenInclude(oi => oi.Product)
    //    .AsNoTracking()
    //    .FirstOrDefaultAsync(c => c.CustomerId == id);

    //  ViewData["CustomerID"] = id.Value;
    //  viewModel.Orders = viewModel.Customer.Orders;

    //  if (orderId != null)
    //  {
    //    ViewData["OrderID"] = orderId.Value;
    //    viewModel.OrderItems = viewModel.Orders.Single(o => o.OrderId == orderId).OrderItems;
    //  }

    //  return View(viewModel);
    //}

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var test = await _testRepository.FindAsync(id);

        return View(test);
    }

    // POST: Customers/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("TestId,Name,Description,NumberOfQuestions,NumberOfAnswersPerQuestion")] Test test)
    {
        if (id != test.TestId)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(test);
        }

        try
        {
            await _testRepository.UpdateTestAsync(test);
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_testRepository.TestExists(test.TestId))
            {
                return NotFound();
            }

            throw;
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Test test)
    {
        if (!ModelState.IsValid)
        {
            return View(test);
        }

        await _testRepository.UpdateTestAsync(test);

        return RedirectToAction(nameof(Index));
    }

    // GET: Test/Delete/5
    public IActionResult Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var test = _testRepository.FindAsync(id);

        return View(test);
    }

    // POST: Test/Delete/5
    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _testRepository.DeleteTestAsync(id);

        return RedirectToAction("Index");
    }
}