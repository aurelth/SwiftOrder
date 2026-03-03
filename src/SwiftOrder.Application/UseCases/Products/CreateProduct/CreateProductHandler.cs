using FluentValidation;
using MediatR;
using SwiftOrder.Application.Abstractions.Persistence;
using SwiftOrder.Application.DTOs.Products;
using SwiftOrder.Domain.Entities;

namespace SwiftOrder.Application.UseCases.Products.CreateProduct;

public sealed class CreateProductHandler : IRequestHandler<CreateProductCommand, ProductDto>
{
    private readonly IProductRepository _products;
    private readonly IUnitOfWork _uow;
    private readonly IValidator<CreateProductRequest> _validator;

    public CreateProductHandler(
        IProductRepository products,
        IUnitOfWork uow,
        IValidator<CreateProductRequest> validator)
    {
        _products = products;
        _uow = uow;
        _validator = validator;
    }

    public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken ct)
    {
        await _validator.ValidateAndThrowAsync(request.Request, ct);

        // Prevent duplicate SKU
        var existing = await _products.GetBySkuAsync(request.Request.Sku, ct);
        if (existing is not null)
            throw new InvalidOperationException("A product with the same SKU already exists.");

        var product = new Product(
            request.Request.Name,
            request.Request.Sku,
            request.Request.Price,
            request.Request.StockQuantity
        );

        await _products.AddAsync(product, ct);
        await _uow.SaveChangesAsync(ct);

        return new ProductDto(
            product.Id,
            product.Name,
            product.Sku,
            product.Price,
            product.StockQuantity,
            product.IsActive,
            product.CreatedAt
        );
    }
}