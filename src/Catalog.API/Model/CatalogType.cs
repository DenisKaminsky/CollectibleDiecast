﻿using System.ComponentModel.DataAnnotations;

namespace CollectibleDiecast.Catalog.API.Model;

public class CatalogType
{
    public int Id { get; set; }

    [Required]
    public string Type { get; set; }
}
