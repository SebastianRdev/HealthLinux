namespace ShiftManager.Web.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Person
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string LastName { get; set; }
    
    [Required]
    public string Identification { get; set; }
    [Required]
    public DateOnly BirthDate { get; set; }
    public string Email { get; set; }
    [Required]
    public string Phone { get; set; }
    [Required]
    public string? Address { get; set; }
}