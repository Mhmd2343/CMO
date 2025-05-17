using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeliveryAppSystem.Models
{
    public class DeliveryRequest
    {
        [Key]
        public int RequestId { get; set; }

        [Required]
        public int ClientId { get; set; }

        [ForeignKey(nameof(ClientId))]
        public virtual Client Client { get; set; }

        [Required, MaxLength(200)]
        public string PickupLocation { get; set; }

        [Required, MaxLength(200)]
        public string DropoffLocation { get; set; }

        [Required]
        public DeliveryType DeliveryType { get; set; }

        [Required]
        public RequestStatus Status { get; set; } = RequestStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public enum DeliveryType
    {
        PersonDelivery = 1,
        ShoppingDelivery = 2,
        ItemDelivery = 3
    }

    public enum RequestStatus
    {
        Pending = 1,
        Assigned = 2,
        InProgress = 3,
        Delivered = 4,
        Canceled = 5
    }
}
