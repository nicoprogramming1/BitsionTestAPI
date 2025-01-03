using AutoMapper;
using BitsionTest.API.Domain.Contracts;
using BitsionTest.API.Domain.Entities;
using BitsionTest.API.Repositories.Interface;
using BitsionTest.API.Services.Interface;

namespace BitsionTest.API.Services.Implementation
{
    public class ClientServiceImpl : IClientService
    {

        private readonly IClientRepository _clientRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ClientServiceImpl> _logger;

        public ClientServiceImpl(IClientRepository clientRepository, IMapper mapper, ILogger<ClientServiceImpl> logger)
        {
            _clientRepository = clientRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ClientResponse> RegisterClientAsync(ClientRegisterRequest request)
        {
            try
            {
                _logger.LogInformation("Iniciando proceso de registro de cliente.");

                // comprobamos si el cliente ya existe por el email
                var existingClient = await _clientRepository.GetClientByEmailAsync(request.Email);
                if (existingClient != null)
                {
                    _logger.LogError("El cliente con el email {Email} ya está registrado.", request.Email);
                    throw new InvalidOperationException("El cliente con ese email ya está registrado.");
                }

                var newClient = _mapper.Map<Client>(request);

                var createdClient = await _clientRepository.CreateClientAsync(newClient);

                _logger.LogInformation("Cliente creado exitosamente con el email {Email}.", request.Email);

                // mapeamos la respuesta a ClientResponse
                return _mapper.Map<ClientResponse>(createdClient);
            }
            catch (InvalidOperationException ex) // Los errores los captura el Exceptions/GlobalExceptionHandler
            {
                _logger.LogWarning(ex, "Error durante el registro del cliente con el email {Email}.", request.Email);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocurrió un error durante el registro de cliente con el email {Email}.", request.Email);
                throw new Exception("Ocurrió un error al registrar el cliente.", ex);
            }
        }




        public async Task<bool> DeleteClientAsync(Guid id)
        {
            // recuperamos el cliente primero
            var client = await _clientRepository.GetClientByIdAsync(id);
            if (client == null)
            {
                _logger.LogError("Cliente no encontrado: {id}", id);
                throw new KeyNotFoundException("Cliente no encontrado.");
            }

            try
            {
                // si todo ok, lo intentamos eliminar
                var result = await _clientRepository.DeleteClientAsync(id);
                if (result)
                {
                    _logger.LogInformation("Cliente eliminado exitosamente: {id}", id);
                    return true;
                }

                _logger.LogError("No se pudo eliminar el cliente: {id}", id);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocurrió un error al intentar eliminar el cliente: {id}", id);
                throw new Exception("Ocurrió un error al intentar eliminar el cliente.", ex);
            }
        }




        public async Task<ClientResponse> GetClientByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                _logger.LogWarning("ID del cliente no válido: {id}.");
                throw new ArgumentException("Se debe proveer un ID");
            }

            try
            {
                _logger.LogInformation("Obteniendo cliente con ID: {id}", id);

                var client = await _clientRepository.GetClientByIdAsync(id);

                var clientResponse = _mapper.Map<ClientResponse>(client);

                return clientResponse;
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, "Cliente no encontrado. ID: {id}", id);
                throw new Exception("No se encontró el cliente con el ID especificado.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el cliente con ID: {id}", id);
                throw new Exception("Ocurrió un error al obtener el cliente.");
            }
        }





        // recibe parámetros para paginar y devuelve la lista de clientes paginada
        public async Task<ClientsListResponse> GetClientsListAsync(int pageNumber, int pageSize, string longName, string email)
        {
            if (pageNumber <= 0)
            {
                _logger.LogWarning("Número de página no válido: {pageNumber}. Se ha ajustado a 1.");
                pageNumber = 1; // Pone por defecto valor 1
            }

            if (pageSize <= 0 || pageSize > 100) // limitamos a 100
            {
                _logger.LogWarning("Tamaño de página no válido: {pageSize}. Se ha ajustado a 10.");
                pageSize = 10; // Por defecto 10
            }

            try
            {
                _logger.LogInformation("Obteniendo lista de clientes (página: {pageNumber}, tamaño: {pageSize})", pageNumber, pageSize);

                // recuperamos la lista de clientes filtrada y paginada
                var paginatedClients = await _clientRepository.GetClientsListAsync(pageNumber, pageSize, longName, email);

                if (paginatedClients.TotalCount == 0)
                {
                    _logger.LogInformation("No se encontraron clientes para la página: {pageNumber}", pageNumber);
                }

                var clientResponses = _mapper.Map<ClientResponse[]>(paginatedClients.Items);

                // devolvemos un ClientsListResponse con los metadatos y los clientes enlistados
                return new ClientsListResponse
                {
                    Clients = clientResponses,
                    TotalCount = paginatedClients.TotalCount,
                    PageNumber = paginatedClients.PageNumber,
                    PageSize = paginatedClients.PageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la lista de clientes (página: {pageNumber}, tamaño: {pageSize})", pageNumber, pageSize);
                throw new Exception("Ocurrió un error al obtener la lista de clientes.");
            }
        }





        public async Task<ClientResponse> UpdateClientAsync(Guid id, ClientUpdateRequest request)
        {
            try
            {
                _logger.LogInformation("Iniciando actualización de cliente.");

                // mapeamos la request a Client
                var clientToUpdate = _mapper.Map<Client>(request);
                clientToUpdate.Id = id;  // le asignamos el id que llega por req al cliente

                var result = await _clientRepository.UpdateClientAsync(clientToUpdate);
                if (!result)
                {
                    _logger.LogError("No se pudo actualizar el cliente con el ID {id}.", id);
                    throw new Exception("No se pudo actualizar el cliente.");
                }

                _logger.LogInformation("Cliente actualizado exitosamente con el ID {id}.", id);

                var updatedClientResponse = _mapper.Map<ClientResponse>(clientToUpdate);

                return updatedClientResponse;
            }
            catch (KeyNotFoundException ex)  // si no encuentra el cliente
            {
                _logger.LogWarning(ex, "Error al actualizar cliente con ID {id}.", id);
                throw;
            }
            catch (InvalidOperationException ex)  // si hay conflicto de email
            {
                _logger.LogWarning(ex, "Error de validación para el cliente con el ID {id}.", id);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocurrió un error al actualizar al cliente con el ID {id}.", id);
                throw new Exception("Ocurrió un error al actualizar el cliente.", ex);
            }
        }


    }
}
