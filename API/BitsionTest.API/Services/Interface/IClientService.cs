using BitsionTest.API.Domain.Contracts;

namespace BitsionTest.API.Services.Interface
{
    public interface IClientService
    {
        Task<ClientResponse> RegisterClientAsync(ClientRegisterRequest request);
        Task<ClientsListResponse> GetClientsListAsync(int pageNumber, int pageSize, string longName, string email);  // usa la lista de clientes con paginación
        Task<ClientResponse> GetClientByIdAsync(Guid id);
        Task<ClientResponse> UpdateClientAsync(Guid id, ClientUpdateRequest request);
        Task<bool> DeleteClientAsync(Guid id);
    }
}
