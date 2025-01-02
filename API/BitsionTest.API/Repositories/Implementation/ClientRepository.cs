using BitsionTest.API.Domain.Contracts;
using BitsionTest.API.Domain.Entities;
using BitsionTest.API.Infrastructure.Context;
using BitsionTest.API.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

public class ClientRepository : IClientRepository
{
    private readonly ApplicationDbContext dbContext;

    public ClientRepository(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<Client> CreateClientAsync(Client client)
    {
        if (string.IsNullOrEmpty(client.Email))
        {
            throw new ArgumentException("El email es obligatorio.");
        }

        var existingClient = await dbContext.Clients.FirstOrDefaultAsync(c => c.Email == client.Email);
        if (existingClient != null)
        {
            throw new InvalidOperationException("Ya existe un cliente con este email.");
        }

        await dbContext.Clients.AddAsync(client);
        await dbContext.SaveChangesAsync(); // los SaveChangesAsync son los que aseguran la persistencia asíncrona
        return client;
    }

    public async Task<bool> DeleteClientAsync(Guid id)
    {
        var client = await dbContext.Clients.FirstOrDefaultAsync(c => c.Id == id);
        if (client == null)
        {
            throw new KeyNotFoundException("No se encontró el cliente con el ID especificado.");
        }

        dbContext.Clients.Remove(client);
        await dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<Client> GetClientByEmailAsync(string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            throw new ArgumentException("El email es obligatorio.");
        }

        return await dbContext.Clients.FirstOrDefaultAsync(c => c.Email == email)
               ?? throw new KeyNotFoundException("No se encontró un cliente con este email.");
    }

    public async Task<Client> GetClientByIdAsync(Guid id)
    {
        return await dbContext.Clients.FirstOrDefaultAsync(c => c.Id == id)
               ?? throw new KeyNotFoundException("No se encontró un cliente con el ID especificado.");
    }

    public async Task<PaginatedList<Client>> GetClientsListAsync(int pageNumber, int pageSize)
    {
        var totalCount = await dbContext.Clients.CountAsync();

        // filtra los clientes correspondientes a la página solicitada
        var clients = await dbContext.Clients
            .Skip((pageNumber - 1) * pageSize)  // salta los registros de las páginas anteriores
            .Take(pageSize)                     // acá toma solo la cantidad de clientes correspondiente a la página
            .ToListAsync();

        // instanciamos una nueva PaginatedList y la retornamos
        return new PaginatedList<Client>(clients, totalCount, pageNumber, pageSize);
    }


    public async Task<bool> UpdateClientAsync(Client client)
    {
        var existingClient = await dbContext.Clients.FirstOrDefaultAsync(c => c.Id == client.Id);
        if (existingClient == null)
        {
            throw new KeyNotFoundException("El cliente especificado no existe.");
        }

        // verificamos si el email ya existe exceptuando el del cliente que estamos actualizando
        var emailConflict = await dbContext.Clients.AnyAsync(c => c.Email == client.Email && c.Id != client.Id);
        if (emailConflict)
        {
            throw new InvalidOperationException("Ya existe un cliente con este email.");
        }

        existingClient.LongName = client.LongName;
        existingClient.Age = client.Age;
        existingClient.Gender = client.Gender;
        existingClient.Email = client.Email;
        existingClient.Nationality = client.Nationality;
        existingClient.State = client.State;
        existingClient.Phone = client.Phone;
        existingClient.CanDrive = client.CanDrive;
        existingClient.WearGlasses = client.WearGlasses;
        existingClient.IsDiabetic = client.IsDiabetic;
        existingClient.OtherDiseases = client.OtherDiseases;

        dbContext.Clients.Update(existingClient);
        await dbContext.SaveChangesAsync();

        return true;
    }


}
