# Bitsion-technical-test-WN
This project is developed for a Bitsion technical test - Author: WN

## BACKEND - ASP.NET CORE 8

# Decisiones Técnicas

A continuación se exponen aquellas decisiones técnicas implementadas a lo largo del desarrollo y el presente análisis, junto con las observaciones que llevaron a considerarlas.

### Paquetes

- AutoMapper: para mapear objetos y DTOs de manera estandarizada y reduciendo código repetitivo.
- Identity: para gestionar usuarios, autenticaciones, tokens, roles; de manera robusta y flexible.

### Patterns

A continuación se presentan los patterns o patrones utilizados de manera consciente (los que soy capaz de reconocer e implementar por decisión consciente):

- Patrón de Diseño Dependency Injection (o Dependency Inversion) para eliminar dependencias hard-codeadas o instanciadas en los objetos por una inyección desde una locación central a pedido cumpliendo de esta manera con la "D" de los principios SOLID.

### SQLServer

La base de datos será SQLServer ya que es un motor de base de datos relacional robusto e integrado perfectamente con .NET y Entity Framework Core y en especial Visual Studio, lo que permite una fácil configuración y usabilidad.

### DbContext

Se utilizará **DBContext** del paquete **Microsoft.EntityFrameworkCore** en la capa **Repository** para mantener una conexión con la DB, mapear entidades, trackear cambios y acceder a operaciones CRUD. Al estar trabajando con SQLServer el proveedor utilizado será **Microsoft.EntityFrameworkCore.SqlServer**. Esta herramienta estará representada en la carpeta Data en la clase ApplicationDbContext quien hereda del DbContext.

### CodeFirst

Para el desarrollo los modelos serán definidos mediante clases en C#.

### isDeleted

Este atributo en un cliente indica si el cliente ha sido eliminado del sistema en caso de true. Un cliente se registra con este atributo false por defecto y al momento de la eliminación lógica se cambia a true, esto conlleva que dicho cliente será ignorado por las consultas realizadas contra el backend (consultar cliente, listar clientes) resultando en un 404. El front puede abstraerse totalmente de este atributo.

### Entidades auxiliares o secundarias

Respecto a entidades tales como Roles, Phone, Nationality, State y Email.
Plantié tres posibilidades para su implementación, quedándome con la primera:

**
- Usar tablas en la base de datos (Con FK y normalización)**
Creo que es la forma más escalable y flexible, en especial para Roles. Si éstos luego deben ser modificados o agregados nuevos, considero es el mejor enfoque, por más que en un proyecto pequeño incremente el tamaño del mismo y la cantidad de código utilizado.

- Usar enum
Aunque es muy rápido y sencillo de implementar adhiere rigidez y en caso de querer escalarlo habría que tocar uno por uno en cada capa. Además puede dar problemas de consistencia.

- Usar una clase de roles (sin tabla)
No es ideal si se quiere una base de datos normalizada o más avanzada con relaciones que deberían ser representadas.
