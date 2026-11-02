
using Microsoft.AspNetCore.Mvc;
using TechDesk.Data;
using TechDesk.Models;

namespace TechDesk.Controllers
{
    public class DevicesController : Controller
    {
        private readonly AppDbContext _db;
        public DevicesController(AppDbContext db) { _db = db; }

        [Route("devices")]
        public IActionResult Index(string? name, bool? availableOnly)
        {
            var query = _db.Devices.AsQueryable();
            if (!string.IsNullOrWhiteSpace(name))
                query = query.Where(d => d.Name.Contains(name));
            if (availableOnly == true)
                query = query.Where(d => !d.IsOnLoan);
            query = query.OrderBy(d => d.AssetTag);
            var devices = query.ToList();
            ViewBag.Count = devices.Count;
            ViewBag.Name = name;
            ViewBag.AvailableOnly = availableOnly;
            return View(devices);
        }

        [Route("devices/{id:int}")]
        public IActionResult Details(int id)
        {
            var device = _db.Devices.FirstOrDefault(d => d.Id == id);
            if (device == null) return NotFound();
            return View(device);
        }

        [Route("devices/add")]
        [HttpGet]
        public IActionResult Add()
        {
            return View(new Device());
        }

        [Route("devices/add")]
        [HttpPost]
        public IActionResult Add(Device device)
        {
            if (_db.Devices.Any(d => d.AssetTag == device.AssetTag))
            {
                ModelState.AddModelError("AssetTag", "Another device already has that asset tag.");
            }

            if (!ModelState.IsValid)
            {
                return View(device); 
            }

            _db.Devices.Add(device);
            _db.SaveChanges();
            TempData["Message"] = $"{device.AssetTag} {device.Name} was added";
            return RedirectToAction("Index");
        }

        [Route("devices/edit/{id:int}")]
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var device = _db.Devices.FirstOrDefault(d => d.Id == id);
            if (device == null) return NotFound();
            return View(device);
        }

        [Route("devices/edit/{id:int}")]
        [HttpPost]
        public IActionResult Edit(int id, Device device)
        {
            if (id != device.Id) return NotFound();

            if (_db.Devices.Any(d => d.AssetTag == device.AssetTag && d.Id != device.Id))
            {
                ModelState.AddModelError("AssetTag", "Another device already has that asset tag");
            }

            if (!ModelState.IsValid)
            {
                return View(device);
            }

            var existing = _db.Devices.FirstOrDefault(d => d.Id == id);
            if (existing == null) return NotFound();

            existing.AssetTag = device.AssetTag;
            existing.Name = device.Name;
            existing.Category = device.Category;
            existing.PurchaseYear = device.PurchaseYear;
            existing.IsOnLoan = device.IsOnLoan;

            _db.SaveChanges();
            TempData["Message"] = $"{device.AssetTag} {device.Name} was updated";
            return RedirectToAction("Index");
        }

        [Route("devices/delete/{id:int}")]
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var device = _db.Devices.FirstOrDefault(d => d.Id == id);
            if (device == null) return NotFound();
            return View(device); 
        }

        [Route("devices/delete/{id:int}")]
        [HttpPost]
        public IActionResult DeleteConfirmed(int id)
        {
            var device = _db.Devices.FirstOrDefault(d => d.Id == id);
            if (device == null)
            {
                TempData["Message"] = "That device was already deleted";
                return RedirectToAction("Index");
            }

            _db.Devices.Remove(device);
            _db.SaveChanges();
            TempData["Message"] = $"{device.AssetTag} {device.Name} was deleted";
            return RedirectToAction("Index");
        }
    }
}