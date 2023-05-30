using Application;
using Application.Commands;
using Application.DTO;
using Application.Queries;
using Application.Validators;
using FluentValidation.Results;
using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace VerticalSliceShop.Controllers;

[ApiController]
[Route("[controller]")]
public class BasketsController : ControllerBase
{
    private readonly ILogger<BasketsController> _logger;
    private readonly IMediator _mediator;
    
    public BasketsController(ILogger<BasketsController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [HttpGet]
    [Route("{id:guid}")]
    public async Task<Results<NotFound, Ok<GetBasket>>> Get([FromRoute] Guid id)
    {
        var request = new GetBasketQuery(id);

        var result = await _mediator.Send(request);

        return result.IsSuccessful ? TypedResults.Ok(result.Value) : TypedResults.NotFound();
    }

    [HttpPost]
    public async Task<Results<ProblemHttpResult, ValidationProblem,  Ok<BasketAdded>>> Add([FromBody] AddBasketCommand command)
    {
        var validator = new AddBasketValidator();
        var validationResult = await validator.ValidateAsync(command);
        
        if (!validationResult.IsValid) 
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }
        
        var result = await _mediator.Send(command);
        
        return result.IsSuccessful ? TypedResults.Ok(result.Value) : TypedResults.Problem("", "", (int)ErrorCodes.InternalServerError);
    }
}