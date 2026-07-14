using System.ComponentModel.DataAnnotations;

namespace TourApi.DTOs.Tours;

public class AssignTravelAgentRequest
{
    [Range(1, int.MaxValue)]
    public int TravelAgentId { get; set; }
}