using System;
using System.Collections.Generic;

namespace WpfDemoExam02.Models;

public partial class Order
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public int Amount { get; set; }

    public DateTime OrderDate { get; set; }

    public DateTime DeliveryDate { get; set; }

    public int PickupPointAddressId { get; set; }

    public int UserId { get; set; }

    public int ObtainingCode { get; set; }

    public int OrderStatusId { get; set; }

    public virtual OrderStatus OrderStatus { get; set; } = null!;

    public virtual House PickupPointAddress { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
