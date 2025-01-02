using System.ComponentModel.DataAnnotations;

namespace BitsionTest.API.Domain.Contracts
{

    // ENUMS
    public enum Genero
    {
        Masculino,
        Femenino,
        Otro
    }

    public enum State
    {
        Activo,
        Inactivo
    }

    public enum Nationality
    {
        Argentina,
        Brasil,
        Chile,
        Uruguay,
        Paraguay
    }


    // CUSTOM VALIDATORS

    // esta clase valida que si se escribe en el campo otras enfermedades no sean sólo espacios en blanco
    public class DiseaseWithoutBlankSpacesAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value is string str && !string.IsNullOrWhiteSpace(str))
            {
                return true;
            }

            return false; // Si es solo espacios en blanco o null
        }
    }


    // DTOS de Client

    public class ClientRegisterRequest
    {

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string LongName { get; set; }


        [Required(ErrorMessage = "La edad es obligatoria.")]
        [Range(18, int.MaxValue, ErrorMessage = "La edad debe ser un número entero mayor o igual a 18 años.")]
        public int Age { get; set; }


        [Required(ErrorMessage = "El género es obligatorio.")]
        [EnumDataType(typeof(Genero), ErrorMessage = "El género seleccionado no es válido.")]
        public string Gender { get; set; }


        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "El correo electrónico ingresado no es válido.")]
        public string Email { get; set; }


        [EnumDataType(typeof(Nationality), ErrorMessage = "La nacionalidad seleccionada no es válida.")]
        public string? Nationality { get; set; }


        [Required(ErrorMessage = "El estado es obligatorio.")]
        [EnumDataType(typeof(State), ErrorMessage = "El estado debe ser Activo o Inactivo.")]
        public string State { get; set; }


        [Required(ErrorMessage = "El teléfono es obligatorio.")]
        [RegularExpression(@"^\+?(?:\d{1,3})?[-\s]?\(?\d{1,4}\)?[-\s]?\d{6,10}$",
                       ErrorMessage = "El número de teléfono no es válido. Ejemplo válido: +54 911 1234 5678")]
        public string Phone { get; set; }


        [Required(ErrorMessage = "Debes especificar si sabe conducir o no de forma obligatoria.")]
        public bool CanDrive { get; set; }


        [Required(ErrorMessage = "Debes especificar si usa lentes o no de forma obligatoria.")]
        public bool WearGlasses { get; set; }


        [Required(ErrorMessage = "Debes especificar si es diabético o no de forma obligatoria.")]
        public bool IsDiabetic { get; set; }

        [DiseaseWithoutBlankSpaces]   // usamos una custom validate attribute
        [StringLength(500, ErrorMessage = "No debe superar los 500 caracteres.")]
        public string? OtherDiseases { get; set; }

    }

    /**
     * Los timestamps sólo serán para uso interno y auditorías, no los expondremos al front,
     * no hay funcionalidades implementadas que hagan uso de ellos
     * Como la aplicación es manejada por empleados autorizados por el admin
     * no hay otros atributos sensibles que ocultar
     */
    public class ClientResponse
    {
        public Guid Id { get; set; }
        public string LongName { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public string? Nationality { get; set; }
        public string State { get; set; }
        public string Phone { get; set; }
        public bool CanDrive { get; set; }
        public bool WearGlasses { get; set; }
        public bool IsDiabetic { get; set; }
        public string? OtherDiseases { get; set; }
    }

    public class ClientsListResponse
    {
        public ClientResponse[] Clients { get; set; }
        public int TotalCount { get; set; }  // Total de clientes disponibles
        public int PageNumber { get; set; }  // Página actual
        public int PageSize { get; set; }    // Cantidad de elementos por página
    }



    // igual al ClientRegisterRequest debido al método PUT
    public class ClientUpdateRequest
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string LongName { get; set; }


        [Required(ErrorMessage = "La edad es obligatoria.")]
        [Range(18, int.MaxValue, ErrorMessage = "La edad debe ser un número entero mayor o igual a 18 años.")]
        public int Age { get; set; }


        [Required(ErrorMessage = "El género es obligatorio.")]
        [EnumDataType(typeof(Genero), ErrorMessage = "El género seleccionado no es válido.")]
        public string Gender { get; set; }


        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "El correo electrónico ingresado no es válido.")]
        public string Email { get; set; }


        [EnumDataType(typeof(Nationality), ErrorMessage = "La nacionalidad seleccionada no es válida.")]
        public string? Nationality { get; set; }


        [Required(ErrorMessage = "El estado es obligatorio.")]
        [EnumDataType(typeof(State), ErrorMessage = "El estado debe ser Activo o Inactivo.")]
        public string State { get; set; }


        [Required(ErrorMessage = "El teléfono es obligatorio.")]
        [RegularExpression(@"^\+?(?:\d{1,3})?[-\s]?\(?\d{1,4}\)?[-\s]?\d{6,10}$",
                       ErrorMessage = "El número de teléfono no es válido. Ejemplo válido: +54 911 1234 5678")]
        public string Phone { get; set; }


        [Required(ErrorMessage = "Debes especificar si sabe conducir o no de forma obligatoria.")]
        public bool CanDrive { get; set; }


        [Required(ErrorMessage = "Debes especificar si usa lentes o no de forma obligatoria.")]
        public bool WearGlasses { get; set; }


        [Required(ErrorMessage = "Debes especificar si es diabético o no de forma obligatoria.")]
        public bool IsDiabetic { get; set; }

        [DiseaseWithoutBlankSpaces]   // usamos una custom validate attribute
        [StringLength(500, ErrorMessage = "No debe superar los 500 caracteres.")]
        public string? OtherDiseases { get; set; }
    }


    public class ClientDeleteRequest
    {
        [Required(ErrorMessage = "El ID debe ser proporcionado de forma obligatoria.")]
        public Guid Id { get; set; }

    }
}
