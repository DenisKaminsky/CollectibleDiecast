﻿using CollectibleDiecast.WebAppComponents.Services;

namespace CollectibleDiecast.WebApp.Services;

public class ProductImageUrlProvider : IProductImageUrlProvider
{
    public string GetProductImageUrl(int productId)
        => $"product-images/{productId}?api-version=1.0";
}