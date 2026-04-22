using ClinicalTrialsApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace ClinicalTrialsApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClinicalTrialsController: ControllerBase
{
    // Datos "en memoria" por ahora.
    private static readonly List<ClinicalTrial> _trials = new()
    {
        new ClinicalTrial
        {
            Id = 1,
            Name = "GSK-ASTHMA-2026-A",
            Phase = "III",
            PatientCount = 450,
            Status = "Recruiting",
            StartDate = new DateTime(2026, 1, 15)
        },
        new ClinicalTrial
        {
            Id = 2,
            Name = "GSK-ONC-2025-B",
            Phase = "II",
            PatientCount = 120,
            Status = "Active",
            StartDate = new DateTime(2025, 9, 1)
        }
    };

    [HttpGet]
    public ActionResult<IEnumerable<ClinicalTrial>> GetAll()
    {
        return Ok(_trials);
    }

    [HttpGet("id")]
    public ActionResult<ClinicalTrial> GetById(int Id)
    {
        var trial = _trials.FirstOrDefault(t => t.Id == Id);
        
        if (trial is null)
            return NotFound();
        
        return Ok(trial);
    }
}