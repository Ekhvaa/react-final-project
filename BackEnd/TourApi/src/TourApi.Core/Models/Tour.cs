using System.ComponentModel.DataAnnotations;

namespace TourApi.Models;

public class Tour : BaseEntity
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    
    [StringLength(1000)]
    public string? Description { get; set; }

    // Kept from the original script: no enforced FK (see README) - typically
    // used to point at the "main" / cover TourDetail row for a tour.
    public int TourDetailId { get; set; }

    public decimal CurrentPrice { get; set; }

    public ICollection<TourDetail> TourDetails { get; set; } = new List<TourDetail>();
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    public int? CreatedByEmployeeId { get; set; }
    public Employee? CreatedByEmployee { get; set; }

    public int? AssignedTourGuideId { get; set; }
    public TourGuide? AssignedTourGuide { get; set; }

    public ICollection<TourImage> Images { get; set; } = new List<TourImage>();
    public ICollection<TouringHistory> TouringHistories { get; set; } = new List<TouringHistory>();
}
