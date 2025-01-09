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


    // soft delete, cambia el isDeleted de un client a true de forma tal que sea filtrado
    // todas las querys quedando "marcado" como inactivo o borrado
    public async Task<bool> DeleteClientAsync(Guid id)
    {
        var client = await dbContext.Clients.FirstOrDefaultAsync(c => c.Id == id);
        if (client == null)
        {
            throw new KeyNotFoundException("No se encontró el cliente con el ID especificado.");
        }

        // cambiamos el estado a true para lograr la eliminación lógica
        client.isDeleted = true;

        // mapeamos a cliente los datos a actualizar
        dbContext.Entry(client).CurrentValues.SetValues(client);
        await dbContext.SaveChangesAsync();
        return true;
    }


    /**
     * 
     * Como vamos a implementar una eliminación lógica, no es necesario este método para un delete completo
     * pero lo lo había hecho en primera instancia y lo dejo a modo de ofrecer una alternativa viable
     * 
     * public async Task<bool> DeleteClientAsync(Guid id)
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
     */



    public async Task<Client?> GetClientByEmailAsync(string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            throw new ArgumentException("El email es obligatorio.");
        }

        // si no se encuentra un cliente, se devuelve null para no romper el flujo (antes largaba una excepción)
        return await dbContext.Clients.FirstOrDefaultAsync(c => c.Email == email);
    }




    public async Task<Client> GetClientByIdAsync(Guid id)
    {
        return await dbContext.Clients.FirstOrDefaultAsync(c => c.Id == id)
               ?? throw new KeyNotFoundException("No se encontró un cliente con el ID especificado.");
    }



    public async Task<PaginatedList<Client>> GetClientsListAsync(int pageNumber, int pageSize, string longName, string email)
    {
        var query = dbContext.Clients.AsQueryable();

        // esto filtra por nombre o email
        if (!string.IsNullOrEmpty(longName))
        {
            query = query.Where(c => c.LongName.Contains(longName));
        }

        if (!string.IsNullOrEmpty(email))
        {
            query = query.Where(c => c.Email.Contains(email));
        }

        var totalCount = await query.CountAsync();

        // filtra por página
        var clients = await query
            .Skip((pageNumber - 1) * pageSize)  // salta los registros de las páginas anteriores
            .Take(pageSize)                     // toma solo la cantidad de clientes correspondiente a la página
            .ToListAsync();

        return new PaginatedList<Client>(clients, totalCount, pageNumber, pageSize);
    }



    public async Task<bool> UpdateClientAsync(Client client)
    {
        var existingClient = await dbContext.Clients.FirstOrDefaultAsync(c => c.Id == client.Id);
        if (existingClient == null)
        {
            throw new KeyNotFoundException("El cliente especificado no existe.");
        }

        // comprobamos que el mail no esté registrado en otro cliente filtrando el cliente que actualizamos
        var emailConflict = await dbContext.Clients.AnyAsync(c => c.Email == client.Email && c.Id != client.Id);
        if (emailConflict)
        {
            throw new InvalidOperationException("Ya existe un cliente con este email.");
        }

        // mapeamos a cliente los datos a actualizar
        dbContext.Entry(existingClient).CurrentValues.SetValues(client);

        await dbContext.SaveChangesAsync();
        return true;
    }



}
