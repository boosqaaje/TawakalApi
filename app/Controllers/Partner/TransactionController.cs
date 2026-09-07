using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TawakalApi.app.Models.Dtos.RequestDtos;
using TawakalApi.app.Models.Dtos.ResponseDtos;
using TawakalApi.app.Services.Transaction;

namespace TawakalApi.app.Controllers.Partner;


[ApiController]
[Route("partner/transaction")]
public class TransactionController(
    ITransactionService tService
) : BaseApiController
{

    private readonly ITransactionService _tService = tService;

    [HttpPost("send")]
    public async Task<CommonRes> SendTransaction(
        [FromBody] PartnerTransactionRequestDto? dto
        )
    {
        return await _tService.InsertTransactionAsync(dto);
    }



    [HttpGet("status/{reference}")]
    public async Task<CommonRes> GetStatus(string reference)
    {
        return await _tService.GetTransactionStatusAsync(reference);
    }

    [HttpGet("cancel/{reference}")]
    public async Task<CommonRes> Cancel(string reference)
    {
        return await _tService.CancelTransactionAsync(reference);
    }

}