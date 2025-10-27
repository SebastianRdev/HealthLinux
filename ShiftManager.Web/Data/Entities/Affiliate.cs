namespace ShiftManager.Web.Data.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ShiftManager.Web.Models.Enums;

using ShiftManager.Web.Models;

public class Affiliate : Person
{
    public Affiliate() {}
    
    public Affiliate(string? password, DocumentType documenttype, string? photoUrl, string? membershipCardUrl, bool active)
    {
        Password = password;
        Role = AffiliateRole.Member;
        DocumentType = documenttype;
        MembershipNumber = Guid.NewGuid().ToString();
        PhotoUrl = photoUrl;
        MembershipCardUrl = membershipCardUrl;
        UniqueCode = Guid.NewGuid().ToString();
        RegistrationDate = DateTime.Now;
        Active = active;
    }
    
    public string? Password { get; set; }
    public AffiliateRole Role { get; set; } = AffiliateRole.Member;
    public DocumentType DocumentType { get; set; }
    public string MembershipNumber { get; set; } = Guid.NewGuid().ToString();
    
    public string? PhotoUrl { get; set; }
    public DateTime RegistrationDate { get; set; } = DateTime.Now;
    public string? MembershipCardUrl { get; set; }
    public string? UniqueCode { get; set; } = Guid.NewGuid().ToString();
    public bool Active { get; set; } = true;
    public bool ProfileComplete { get; set; } = false;
}