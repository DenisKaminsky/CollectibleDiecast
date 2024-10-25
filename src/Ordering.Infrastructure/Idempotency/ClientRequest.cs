﻿using System.ComponentModel.DataAnnotations;

namespace CollectibleDiecast.Ordering.Infrastructure.Idempotency;

public class ClientRequest
{
    public Guid Id { get; set; }
    [Required]
    public string Name { get; set; }
    public DateTime Time { get; set; }
}
