using MediatR;
using SwiftOrder.Application.DTOs.Products;

namespace SwiftOrder.Application.UseCases.Products.CreateProduct;

public sealed record CreateProductCommand(CreateProductRequest Request) : IRequest<ProductDto>;