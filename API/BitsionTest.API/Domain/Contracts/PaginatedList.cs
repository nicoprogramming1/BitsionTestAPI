namespace BitsionTest.API.Domain.Contracts
{
    /**
     * Esta clase podría ser reutilizable a cualquier entidad de la que se necesitase
     * recuperar enlistada y paginada en un futuro dando así escalabilidad
     * por lo cual la separé del repositorio de clientes donde es actualmente utilizada
     */

    public class PaginatedList<T>
    {
        public List<T> Items { get; set; }
        public int TotalCount { get; set; } // total de clientes
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        public PaginatedList(List<T> items, int totalCount, int pageNumber, int pageSize)
        {
            Items = items;
            TotalCount = totalCount;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }
}
