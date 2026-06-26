using ConfigReader.Api.Requests;
using ConfigReader.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace ConfigReader.Api.Controllers;



/// Konfigürasyon kayıtlarını yönetmek için kullanılan REST API controller'ıdır

[ApiController]
[Route("api/configurations")]
public sealed class ConfigurationsController : ControllerBase
{
    private readonly ConfigurationAdminService _service;

    public ConfigurationsController(ConfigurationAdminService service)
    {
        _service = service;
    }


    /// Tüm konfigürasyon kayıtlarını listeler
    
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var items = await _service.GetAllAsync(cancellationToken);
        return Ok(items);
    }


    /// Yeni konfigürasyon kaydı oluşturur

    [HttpPost]
    public async Task<IActionResult> Create(CreateConfigurationRequest request, CancellationToken cancellationToken)
    {
        var created = await _service.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetAll), new { id = created.Id }, created);
    }

    /// Mevcut konfigürasyon kaydını günceller
    
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, UpdateConfigurationRequest request, CancellationToken cancellationToken)
    {
        var updated = await _service.UpdateAsync(id, request, cancellationToken);

        if (!updated)
            return Conflict("Configuration başka bir kullanıcı tarafından değiştirildi veya mevcut değil.");

        return NoContent();
    }

    /// Mevcut konfigürasyon kaydını siler

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken)
    {
        var deleted = await _service.DeleteAsync(id, cancellationToken);
        if (!deleted)
            return NotFound();

        return NoContent();
    }
}