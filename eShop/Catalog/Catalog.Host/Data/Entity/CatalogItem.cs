﻿namespace Catalog.Host.Data.Entity;

public class CatalogItem
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public decimal Price { get; set; }

    public string PictureFileName { get; set; }

    public Guid? CatalogTypeId { get; set; }

    public CatalogType? CatalogType { get; set; }

    public Guid? CatalogBrandId { get; set; }

    public CatalogBrand? CatalogBrand { get; set; }

    public int AvailableStock { get; set; }
}