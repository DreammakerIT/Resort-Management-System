using System;
using System.Linq;
using System.Threading.Tasks;
using LuxuryResort.Data;
using LuxuryResort.Areas.Admin.Models; // Đảm bảo đã có using này
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LuxuryResort.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RoomInstancesController : Controller
    {
        private readonly LuxuryResortContext _context;

        public RoomInstancesController(LuxuryResortContext context)
        {
            _context = context;
        }

        // GET: Admin/RoomInstances
        public async Task<IActionResult> Index()
        {
            var roomInstances = _context.RoomInstances.Include(r => r.Room);
            return View(await roomInstances.ToListAsync());
        }

        // GET: Admin/RoomInstances/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var roomInstance = await _context.RoomInstances
                .Include(r => r.Room)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (roomInstance == null) return NotFound();

            return View(roomInstance);
        }

        // GET: Admin/RoomInstances/Create
        public IActionResult Create()
        {
            // Chuẩn bị dropdown list cho Hạng phòng
            ViewData["RoomId"] = new SelectList(_context.Rooms, "Id", "Type");
            return View();
        }

        // POST: Admin/RoomInstances/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RoomNumber,RoomId")] RoomInstance roomInstance)
        {
            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (string.IsNullOrWhiteSpace(roomInstance.RoomNumber))
                {
                    ModelState.AddModelError("RoomNumber", "Vui lòng nhập số phòng.");
                }

                if (roomInstance.RoomId <= 0)
                {
                    ModelState.AddModelError("RoomId", "Vui lòng chọn hạng phòng.");
                }

                // Kiểm tra số phòng trùng lặp
                if (!string.IsNullOrWhiteSpace(roomInstance.RoomNumber))
                {
                    var existingRoom = await _context.RoomInstances
                        .FirstOrDefaultAsync(ri => ri.RoomNumber == roomInstance.RoomNumber);

                    if (existingRoom != null)
                    {
                        ModelState.AddModelError("RoomNumber", "Số phòng này đã tồn tại.");
                    }
                }

                if (ModelState.IsValid)
                {
                    roomInstance.Status = "Available";
                    _context.Add(roomInstance);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Tạo phòng mới thành công!";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra khi tạo phòng: " + ex.Message);
            }

            // Nếu có lỗi, load lại ViewBag và trả về view
            ViewData["RoomId"] = new SelectList(_context.Rooms, "Id", "Type", roomInstance.RoomId);
            return View(roomInstance);
        }

        // GET: Admin/RoomInstances/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var roomInstance = await _context.RoomInstances.FindAsync(id);
            if (roomInstance == null) return NotFound();

            ViewData["RoomId"] = new SelectList(_context.Rooms, "Id", "Type", roomInstance.RoomId);
            return View(roomInstance);
        }

        // POST: Admin/RoomInstances/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,RoomNumber,RoomId,Status")] RoomInstance roomInstance)
        {
            if (id != roomInstance.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(roomInstance);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RoomInstanceExists(roomInstance.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["RoomId"] = new SelectList(_context.Rooms, "Id", "Type", roomInstance.RoomId);
            return View(roomInstance);
        }

        // GET: Admin/RoomInstances/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var roomInstance = await _context.RoomInstances
                .Include(r => r.Room)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (roomInstance == null) return NotFound();

            return View(roomInstance);
        }

        // POST: Admin/RoomInstances/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var roomInstance = await _context.RoomInstances.FindAsync(id);
            if (roomInstance != null)
            {
                _context.RoomInstances.Remove(roomInstance);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool RoomInstanceExists(int id)
        {
            return _context.RoomInstances.Any(e => e.Id == id);
        }
    }
}