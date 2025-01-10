using BitsionTest.API.Domain.Contracts;
using BitsionTest.API.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BitsionTest.API.Controllers
{
    
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly IClientService _clientService;
        private readonly ILogger<ClientsController> _logger;

        public ClientsController(IClientService clientService, ILogger<ClientsController> logger)
        {
            _clientService = clientService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterClient([FromBody] ClientRegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Model State no es válido para el registro.");
                return BadRequest(ModelState);  // Los BadRequest son capturados por el GlobalExceptionHandler
            }

            try
            {
                var response = await _clientService.RegisterClientAsync(request);
                return Ok(response);
            }
            catch (InvalidOperationException ex) // si ya hay ese email registrado
            {
                _logger.LogWarning(ex, "No se pudo registrar el cliente con email {Email}.", request.Email);
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar cliente con email {Email}.", request.Email);
                return StatusCode(500, new { message = "Ocurrió un error al registrar al cliente." });
            }
        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClient(Guid id)
        {
            try
            {
                var result = await _clientService.DeleteClientAsync(id);
                if (result)
                    return NoContent();  // Si todo ok devuelve NoCOntent

                _logger.LogWarning("Cliente no encontrado con ID {id}.", id);
                return NotFound(new { message = "Cliente no encontrado." });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Cliente no encontrado con ID {id}.", id);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al intentar eliminar cliente con ID {id}.", id);
                return StatusCode(500, new { message = "Ocurrió un error al intentar eliminar al cliente." });
            }
        }



        [HttpGet("{id}")]
        public async Task<IActionResult> GetClientById(Guid id)
        {
            if (id == Guid.Empty)
            {
                _logger.LogWarning("Se ha provisto un ID no válido: {id}.", id);
                return BadRequest(new { message = "Se debe proveer un ID válido." });
            }

            try
            {
                var response = await _clientService.GetClientByIdAsync(id);
                return Ok(response);
            }
            catch (ArgumentException ex) // ID inváludo
            {
                _logger.LogWarning(ex, "El ID no es válido: {id}.", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex) // no existe el client
            {
                _logger.LogWarning(ex, "No se encontró cliente con ID: {id}.", id);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener cliente con ID: {id}.", id);
                return StatusCode(500, new { message = "Ocurrió un error al obtener el cliente." });
            }
        }



        [HttpGet]
        public async Task<IActionResult> GetClientsList([FromQuery] int pageNumber, [FromQuery] string longName = "", [FromQuery] string email = "")
        {
            int pageSize = 10;  // la cantidad de clientes por página será 10 predeterminado
            try
            {
                var response = await _clientService.GetClientsListAsync(pageNumber, pageSize, longName, email);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la lista de clientes con página {pageNumber}, tamaño {pageSize}.", pageNumber, pageSize);
                return StatusCode(500, new { message = "Ocurrió un error al obtener la lista de clientes." });
            }
        }




        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateClient(Guid id, [FromBody] ClientUpdateRequest request)
        {
            if (id == Guid.Empty)
            {
                _logger.LogWarning("El ID no es válido: {id}.", id);
                return BadRequest(new { message = "Se debe proveer un ID válido." });
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Los datos del cliente provistos son inválidos.");
                return BadRequest(ModelState);
            }

            try
            {
                var response = await _clientService.UpdateClientAsync(id, request);
                return Ok(response);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "No se encuentra el cliente con ID: {id}.", id);
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Error de validación para cliente con ID: {id}.", id);
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar cliente con ID: {id}.", id);
                return StatusCode(500, new { message = "Ocurrió un error al actualizar el cliente." });
            }
        }
    }
}
