namespace Dominio
{
    public class Usuario : Microsoft.AspNetCore.Identity.IdentityUser
    {
        public string NombreCompleto { get; set; }
    }
}