using AutoMapper;
using Givt.Donations.API.Models.Donations;
using Givt.Donations.Business.QRS.Donations.Create;
using Givt.Platform.Common.Utils;
using Givt.Platform.JWT.Handlers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Givt.Donations.API.Controllers;

[Route("api/[controller]")]
public class DonationsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    private readonly JwtTokenHandler _jwtTokenHandler;
    public DonationsController(IMediator mediator, IMapper mapper, JwtTokenHandler jwtTokenHandler)
    {
        _mediator = mediator;
        _mapper = mapper;
        _jwtTokenHandler = jwtTokenHandler;
    }

    /// <summary>
    /// Create a Donation and Payment Intent (backwards compatibility)
    /// </summary>
    /// <param name="request">Request json</param>
    /// <param name="language">x</param>
    /// <param name="cancellationToken">x</param>
    /// <returns>Information about the newly created payment intent</returns>
    [HttpPost("/api/donation/intent")]
    [ProducesResponseType(typeof(CreateDonationIntentResponse), StatusCodes.Status200OK, "application/json")]
    public async Task<IActionResult> CreatePaymentIntent(
        [FromBody] CreateDonationIntentRequest request,
        [FromHeader(Name = "Accept-Language")] string language,
        CancellationToken cancellationToken)
    {
        var languages = LanguageUtils.GetLanguages(language, request.Language);
        var command = _mapper.Map<DonationCreateCommand>(request);
        command.Languages = languages;
        var model = await _mediator.Send(command);
        var response = _mapper.Map<CreateDonationIntentResponse>(model/*, opt => { opt.Items[Keys.TOKEN_HANDLER] = _jwtTokenHandler; }*/);
        return Ok(response);
    }

}