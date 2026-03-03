using MediatR;
using SwiftOrder.Application.DTOs.Products;

namespace SwiftOrder.Application.UseCases.Products.GetProducts;

public sealed record GetProductsQuery() : IRequest<List<ProductDto>>;