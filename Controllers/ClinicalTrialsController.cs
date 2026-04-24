using ClinicalTrialsApi.Data;
using ClinicalTrialsApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicalTrialsApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClinicalTrialsController: ControllerBase
{
    private readonly ClinicalTrialsDbContext _context;

    public ClinicalTrialsController(ClinicalTrialsDbContext context)
    {
        _context = context;
    }

    // GET /api/clinicaltrials
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ClinicalTrial>>> GetAll()
    {
        var trials = await _context.ClinicalTrials
            .AsNoTracking() // importante para queries de lectura (más rápido)
            .ToListAsync();

        return Ok(trials);
    }

    // GET /api/clinicaltrials/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<ClinicalTrial>> GetById(int id)
    {
        var trial = await _context.ClinicalTrials
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id);

        if (trial is null)
            return NotFound();

        return Ok(trial);
    }

    // POST /api/clinicaltrials
    [HttpPost]
    public async Task<ActionResult<ClinicalTrial>> Create(ClinicalTrial trial)
    {
        _context.ClinicalTrials.Add(trial);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = trial.Id }, trial);
    }

    // PUT /api/clinicaltrials/{id}
    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, ClinicalTrial updatedTrial)
    {
        if (id != updatedTrial.Id)
            return BadRequest("El ID del path no coincide con el del body");

        var existingTrial = await _context.ClinicalTrials.FindAsync(id);
        if (existingTrial is null)
            return NotFound();

        // Actualizar campos
        existingTrial.Name = updatedTrial.Name;
        existingTrial.Phase = updatedTrial.Phase;
        existingTrial.PatientCount = updatedTrial.PatientCount;
        existingTrial.Status = updatedTrial.Status;
        existingTrial.StartDate = updatedTrial.StartDate;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE /api/clinicaltrials/{id}
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var trial = await _context.ClinicalTrials.FindAsync(id);
        if (trial is null)
            return NotFound();

        _context.ClinicalTrials.Remove(trial);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}