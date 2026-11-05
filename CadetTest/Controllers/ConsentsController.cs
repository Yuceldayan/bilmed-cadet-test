using CadetTest.Models;
using CadetTest.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CadetTest.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ConsentsController : ControllerBase
    {
        private IDataService _dataService;

        public ConsentsController(IDataService dataService)
        {
            _dataService = dataService;
        }

        [HttpPost]
        public IActionResult Post(ConsentRequest request)
        {
            var cevap = _dataService.GetRangeById(request.StartId, request.Count);
            return Ok(cevap);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var consent = await _dataService.GetByIdAsync(id);
            if (consent == null) return NotFound();

            return Ok(consent);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create(ConsentSaveRequest request)
        {
            var consent = await _dataService.CreateAsync(request);

            return CreatedAtAction(nameof(GetById), new { id = consent.Id }, consent);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, ConsentSaveRequest request)
        {
            var consent = await _dataService.UpdateAsync(id, request);
            if (consent == null) return NotFound();

            return Ok(consent);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _dataService.DeleteAsync(id);
            if (!deleted) return NotFound();

            return NoContent();
        }
    }
}
