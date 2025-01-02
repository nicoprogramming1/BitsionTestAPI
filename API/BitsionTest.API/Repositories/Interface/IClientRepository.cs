﻿using BitsionTest.API.Domain.Entities;
using static ClientRepository;

namespace BitsionTest.API.Repositories.Interface
{
    public interface IClientRepository
    {
        Task<Client> CreateClientAsync(Client client);
        Task<Client> GetClientByIdAsync(Guid id);
        Task<Client> GetClientByEmailAsync(string email);
        Task<PaginatedList<Client>> GetClientsListAsync(int pageNumber, int pageSize);

        Task<bool> UpdateClientAsync(Client client);
        Task<bool> DeleteClientAsync(Guid id);
    }

}