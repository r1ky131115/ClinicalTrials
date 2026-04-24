using ClinicalTrialsApi.Services;
using Microsoft.AspNetCore.Mvc;
namespace ClinicalTrialsApi.Controllers;


[ApiController]
[Route("api/[controller]")]
public class OperationsController : ControllerBase
{
    private readonly ITransientOperation _transientOperation1;
    private readonly ITransientOperation _transientOperation2;

    private readonly IScopedOperation _scopedOperation1;
    private readonly IScopedOperation _scopedOperation2;

    private readonly ISingletonOperation _singletonOperation1;
    private readonly ISingletonOperation _singletonOperation2;

    public OperationsController(ITransientOperation transientOperation1, ITransientOperation transientOperation2, IScopedOperation scopedOperation1, IScopedOperation scopedOperation2, ISingletonOperation singletonOperation1, ISingletonOperation singletonOperation2)
    {
        _transientOperation1 = transientOperation1;
        _transientOperation2 = transientOperation2;

        _scopedOperation1 = scopedOperation1;
        _scopedOperation2 = scopedOperation2;

        _singletonOperation1 = singletonOperation1;
        _singletonOperation2 = singletonOperation2;
    }

    [HttpGet]
    public IActionResult GetOperations()
    {
        return Ok(new
        {
            TransientOperation1 = _transientOperation1.OperationId,
            TransientOperation2 = _transientOperation2.OperationId,
            ScopedOperation1 = _scopedOperation1.OperationId,
            ScopedOperation2 = _scopedOperation2.OperationId,
            SingletonOperation1 = _singletonOperation1.OperationId,
            SingletonOperation2 = _singletonOperation2.OperationId
        });
    }

    [HttpGet("slow-sync")]
    public IActionResult GetSlotSync()
    {
        // Patrón malo: bloquea el hilo
        Task.Delay(2000).Wait();
        return Ok(new { Message = "Terminé después de 2 segundos (bloqueando hilo)" });
    }

    [HttpGet("slow-async")]
    public async Task<IActionResult> GetSlowAsync()
    {
        // Patrón bueno: libera el hilo
        await Task.Delay(2000);

        return Ok(new { Message = "Terminé después de 2 segundos (sin bloquear)" });
    }

    [HttpGet("good-pattern")]
    public async Task<IActionResult> GoodPatternAsync()
    {
        var result = await ComputeSomethingAsync();

        return Ok(result);
    }

    private async Task<object> ComputeSomethingAsync()
    {
        await Task.Delay(500);

        return new { Value = 42, ComputedAt = DateTime.UtcNow };
    }
}