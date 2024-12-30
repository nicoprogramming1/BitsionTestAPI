# Bitsion-technical-test-WN
This project is developed for a Bitsion SA technical test - WN

# Decisiones Técnicas

A continuación se exponen aquellas decisiones técnicas implementadas a lo largo del desarrollo y el presente análisis, junto con las observaciones que llevaron a considerarlas.

### Estado (state active / inactive)

El estado de un cliente será implementado en la BD como un atributo que indica si el cliente está activo o ha sufrido una eliminación lógica del sistema.
Esto conlleva que dicho cliente será ignorado por las consultas realizadas contra el backend (consultar cliente, listar clientes) resultando en un 404.
De esta manera no es de interés explícito para el front (no está asociado a formularios) y será predeterminado true al momento del registro de un nuevo cliente.

### Entidades auxiliares o secundarias

Respecto a entidades tales como Roles, Phone, Nationality y Email.
Plantié tres posibilidades para su implementación, quedándome con la primera:

**- Usar tablas en la base de datos (Con FK y normalización)**
Motivación: es la forma más escalable y flexible, en especial para Roles. Si éstos luego deben ser modificados o agregados nuevos es el mejor enfoque.

- Usar enum
Motivación: muy rápido y sencillo de implementar aunque adhiere rigidez y en caso de querer escalarlo habría que tocar uno por uno en cada capa. Además puede dar problemas de consistencia.

- Usar una clase de roles (sin tabla)
Motivación: es sencillo de implementar pero pero no es ideal si se quiere una base de datos normalizada o más avanzada con relaciones que deberían ser representadas.
