using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PBL3.Data;
using PBL3.Models;
using System.Linq;
using System.Threading.Tasks;

namespace PBL3.Controllers
{
    public class VehicleInfoController : Controller
    {
        private readonly AppDBContext _context;

        public VehicleInfoController(AppDBContext context)
        {
            _context = context;
        }

        // GET: VehicleInfo
        public async Task<IActionResult> Index()
        {
            return View(await _context.VehicleInfos.ToListAsync());
        }

        // GET: VehicleInfo/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var vehicleInfo = await _context.VehicleInfos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vehicleInfo == null) return NotFound();

            return View(vehicleInfo);
        }

        // GET: VehicleInfo/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: VehicleInfo/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,BienSo,IdChu,TenChu,HetHan,NgayRa,OgiuXe")] VehicleInfo vehicleInfo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(vehicleInfo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(vehicleInfo);
        }

        // GET: VehicleInfo/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var vehicleInfo = await _context.VehicleInfos.FindAsync(id);
            if (vehicleInfo == null) return NotFound();

            return View(vehicleInfo);
        }

        // POST: VehicleInfo/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BienSo,IdChu,TenChu,HetHan,NgayRa,OgiuXe")] VehicleInfo vehicleInfo)
        {
            if (id != vehicleInfo.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(vehicleInfo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VehicleInfoExists(vehicleInfo.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(vehicleInfo);
        }

        // GET: VehicleInfo/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var vehicleInfo = await _context.VehicleInfos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vehicleInfo == null) return NotFound();

            return View(vehicleInfo);
        }

        // POST: VehicleInfo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var vehicleInfo = await _context.VehicleInfos.FindAsync(id);
            if (vehicleInfo != null)
            {
                _context.VehicleInfos.Remove(vehicleInfo);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool VehicleInfoExists(int id)
        {
            return _context.VehicleInfos.Any(e => e.Id == id);
        }

        [HttpGet]
        [Route("api/vehicleinfo/all")]
        public async Task<IActionResult> GetAll()
        {
            var vehicles = await _context.VehicleInfos.ToListAsync();
            return Json(vehicles);
        }

        [HttpPost]
        [Route("api/vehicleinfo/add")]
        public async Task<IActionResult> Add([FromBody] VehicleInfo model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.VehicleInfos.Add(model);
            await _context.SaveChangesAsync();
            return Ok(model);
        }

        [HttpDelete]
        [Route("api/vehicleinfo/delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var vehicle = await _context.VehicleInfos.FindAsync(id);
            if (vehicle == null)
                return NotFound();

            _context.VehicleInfos.Remove(vehicle);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPut]
        [Route("api/vehicleinfo/update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] VehicleInfo model)
        {
            if (id != model.Id)
                return BadRequest("ID mismatch");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var vehicle = await _context.VehicleInfos.FindAsync(id);
            if (vehicle == null)
                return NotFound();

            vehicle.BienSo = model.BienSo;
            vehicle.IdChu = model.IdChu;
            vehicle.TenChu = model.TenChu;
            vehicle.HetHan = model.HetHan;
            vehicle.NgayRa = model.NgayRa;
            vehicle.OgiuXe = model.OgiuXe;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(vehicle);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VehicleInfoExists(id))
                    return NotFound();
                else
                    throw;
            }
        }

        [HttpGet]
        [Route("api/vehicleinfo/search")]
        public async Task<IActionResult> Search([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return Json(await _context.VehicleInfos.ToListAsync());

            var vehicles = await _context.VehicleInfos
                .Where(v => v.BienSo.Contains(query) || 
                           v.IdChu.Contains(query) || 
                           v.TenChu.Contains(query))
                .ToListAsync();

            return Json(vehicles);
        }
    }
}
