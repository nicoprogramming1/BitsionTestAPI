# Bitsion-Test-WN
This project is developed for **Bitsion** Test - Author: **WN**

# ANÁLISIS FUNCIONAL - **FICTICIA SRL**

- Los usuarios son registrado predeterminadamente con el rol de "User", limitando sus funcionalidades sólo a la Gestión de Clientes y a la autenticación
- El usuario Administrador "Admin" puede implementar todas las funcionalidades tanto de la Gestión de Clientes como de Usuarios (es quien puede registrar un nuevo usuario)
- Los clientes poseen un atributo isDeleted para gestionar su eliminación lógica
- 



# FRONTEND - TYPESCRIPT | ANGULAR 18 - Visual Studio Code

## Decisiones Técnicas




# BACKEND - C# | ASP.NET CORE 8 - Visual Studio Community 2022

## Decisiones Técnicas

A continuación se exponen aquellas decisiones técnicas implementadas a lo largo del desarrollo y el presente análisis, junto con las observaciones que llevaron a considerarlas.

- Se utiliza DataAnnotations para las validaciones de los DTOs en las request.
- Se implementa un Exceptions/GlobalExceptionHandler para gestionar excepciones de manera global.
- La aplicación es desarrollada en 3 capas (controlador, servicio, repositorio).
- Sólo el Administrador es capaz de crear nuevos usuarios (por defecto se crean con rol "User").
- Respecto al CRUD de users, sólo se implementa el registro (en los servicios está también el delete, lo hice sin querer pero ya que está lo dejo aunque no será implementado).
- Al iniciar la aplicación se crea por defecto un usuario con rol "Admin" (email: "admin@bitsion.com", password: "Admin123") el cual puede ser utilizado para acceder y registrar nuevos usuarios con rol "User".
- Se implementarán Timestamps para las entidades ApplicationUser y Client de dos formas distintas para mostrar flexibilidad, cada una tiene sus pro y sus contra:
  1. **ApplicationUser:** como no se implementará el update de un usuario, sólo existirá el CreatedAt el cual será especificado de forma manual -> "createdUser.CreatedAt = DateTime.Now;"   en el RegisterAsync() del "../Services/Implementation/UserServiceImpl.cs"
  De esta manera se tiene un control de cómo y cuando se gestiona con poca complejidad técnica, pero también resulta en código repetitivo propenso a errores.
  2. **Client:** al tener un CRUD completo goza de Timestamp de creación y actualización y es implementado de forma automática en el "../Infrastructure/Context/ApplicationDbContext.cs"     haciendo overrides de los métodos SaveChangesAsync() y SaveChanges() y configurando un método HandleTimestamps().
  De esta manera se logra consistencia, reutilización y mantenimiento aunque es más complejo y menos flexible.
- Se desarrollará la aplicación en inglés (excepto comentarios pero sí commits messages) y en notación PascalCase según buenas prácticas en C#, pero se convertirá a camelCase al momento de la Response al front. Esto está configurado en el Program.cs, en el chain de builder.Services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);
- El método de actualización de clientes será **PUT** por tratarse de un proyecto pequeño. En un sistema complejo y con muchas peticiones se vuelve ineficiente ya que el objeto cliente es reemplazado en su totalidad en cada update, resultando en una transferencia mayor de datos. Por cuestiones de tiempo y simplicidad para esta tarea se ha decidido proceder de la manera indicada, generando la siguiente situación: Los **DTO**s "ClientRegisterRequest" y "ClientUpdateRequest" son iguales, pero por razones de escalabilidad y buenas prácticas configuro ambos ya que si en un futuro quisiese agregarse o modificar sus atributos ya está el DTO listo.

### Paquetes

- AutoMapper: para mapear objetos y DTOs de manera estandarizada y reduciendo código repetitivo
- Identity: para gestionar usuarios, autenticaciones, tokens, roles; de manera robusta y flexible
- JwtBearer: para implementar tokens de usuarios
- EntityFrameworkCore: ORM para gestionar y mapear entidades a la base de datos, además de las consultas para actualizar/consultar información en persistencia

### Patterns

A continuación se presentan los patterns o patrones utilizados de manera consciente (los que soy capaz de reconocer e implementar por decisión consciente):

S - 
O - 
L - 
I - 
D - Patrón de Diseño Dependency Injection (o Dependency Inversion) para eliminar dependencias hard-codeadas o instanciadas en los objetos por una inyección desde una locación central a pedido cumpliendo de esta manera con la "D" de los principios SOLID

### SQLServer

La base de datos será SQLServer ya que es un motor de base de datos relacional robusto e integrado perfectamente con .NET y Entity Framework Core y en especial Visual Studio, lo que permite una fácil configuración y usabilidad.

### DbContext

Se utilizará **DBContext** del paquete **Microsoft.EntityFrameworkCore** en la capa **Repository** para mantener una conexión con la DB, mapear entidades, trackear cambios y acceder a operaciones CRUD. Al estar trabajando con SQLServer el proveedor utilizado será **Microsoft.EntityFrameworkCore.SqlServer**. Esta herramienta estará representada en la carpeta Infrastructure/Context en la clase ApplicationDbContext quien hereda del DbContext.

### CodeFirst

Los modelos serán definidos mediante clases en C# usando el enfoque de CodeFirst y luego migrando las entidades a DB.

### isDeleted

Este atributo en un cliente indica si el cliente ha sido eliminado del sistema en caso de true. Un cliente se registra con este atributo false por defecto y al momento de la eliminación lógica se cambia a true, esto conlleva que dicho cliente será ignorado por las consultas realizadas contra el backend (consultar cliente, listar clientes) resultando en un 404. El front puede abstraerse totalmente de este atributo.
Esto se ve reflejado en el ApplicationDbContext.cs -> Método OnModelCreating -> builder.Entity<Client>().HasQueryFilter(c => !c.isDeleted);
