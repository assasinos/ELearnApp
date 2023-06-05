using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using WebApplication3.Validators;

namespace WebApplication3.Models;

public class RegisterModel
{
    [Required(ErrorMessage = "You must specify Username")]
    public string username { get; set; }
    [Required(ErrorMessage = "You must specify Password")]
    public string password { get; set; }
    [Required(ErrorMessage = "You must specify Email")]
    [EmailAddress(ErrorMessage = "Invalid Email Address")]
    public string email { get; set; }
    [Required(ErrorMessage = "You must specify First Name")]
    public string firstname { get; set; }
    [Required(ErrorMessage = "You must specify Surname")]
    public string surname { get; set; }
    
    public string full_name => $"{firstname} {surname}";
    
}