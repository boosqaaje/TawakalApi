using TawakalApi.app.Models.Dtos.RequestDtos;
using TawakalApi.app.Models.Dtos.ResponseDtos;
using TawakalApi.app.Repositories.Transaction;
using TawakalApi.app.Extensions;
using TawakalApi.app.Services.Util;
using TawakalApi.app.Data;
using Microsoft.EntityFrameworkCore;
using TawakalApi.app.Utils.Constants;

namespace TawakalApi.app.Services.Transaction;

public class TransactionService(
    ITransactionRepository tranRepo,
    IHttpContextAccessor contextAccessor,
    AppDbContext dbContext
) : ITransactionService
{

    private readonly ITransactionRepository _tranRepo = tranRepo;
    private readonly IHttpContextAccessor _contextAccessor = contextAccessor;
    private readonly AppDbContext _dbContext = dbContext;

    // Get the partner username from the HTTP context claims
    private string? GetPartnerUserName()
    {
        return _contextAccessor.HttpContext?.User?.GetPartnerUsername();
    }

    public async Task<CommonRes> InsertTransactionAsync(TransactionRequestDto? dto)
    {
        // Run all validations through a dedicated helper method
        var validationResult = ValidateFields(GetPartnerUserName(), dto);

        if (!validationResult.Success)
        {
            return new CommonRes
            {
                Code = validationResult.Code,
                Message = validationResult.Message
            };
        }

        var res = await _tranRepo.InsertTransactionAsync(GetPartnerUserName()!, dto!);

        if (res is null)
        {
            return new CommonRes
            {
                Code = SystemCodes.Tran.TransactionFailed,
                Message = SystemMessages.Tran.TranFailed
            };
        }

        return res;
    }

    public async Task<CommonRes> GetTransactionStatusAsync(string reference)
    {


        var partnerUsername = GetPartnerUserName();
        if (partnerUsername is null || string.IsNullOrWhiteSpace(partnerUsername))
        {
            return new CommonRes
            {
                Code = SystemCodes.Partner.PartnerUsernameMissing,
                Message = SystemMessages.Partner.PartnerUsernameMissing
            };
        }

        if (string.IsNullOrWhiteSpace(reference))
        {
            return new CommonRes
            {
                Code = SystemCodes.RequiredField,
                Message = SystemMessages.Tran.TranRefNumberRequired
            };
        }

        // 2. Query the database for the transaction matching both the reference AND the partner
        var transaction = await _dbContext.TransactionEntity
        .FirstOrDefaultAsync(t => t.ReferenceId == reference && t.PartnerUsername == partnerUsername);

        if (transaction is null)
        {
            return new CommonRes
            {
                Code = SystemCodes.Tran.TrnNotFound,
                Message = SystemMessages.Tran.TranNotFound
            };
        }

        return new CommonRes
        {
            Code = SystemCodes.Success,
            Message = SystemMessages.Tran.TranStatusRetreived,
            Data = new TransactionStatusResDto
            {
                Reference = transaction.ReferenceId,
                Status = transaction.Status,
                Amount = transaction.Amount,
                CreatedAt = transaction.CreatedAt
            }
        };
    }
    public async Task<CommonRes> CancelTransactionAsync(string reference)
    {

        if (string.IsNullOrWhiteSpace(reference))
        {
            return new CommonRes
            {
                Code = SystemCodes.RequiredField,
                Message = SystemMessages.Tran.TranRefNumberRequired
            };
        }

        var partnerUsername = GetPartnerUserName();
        if (partnerUsername is null || string.IsNullOrWhiteSpace(partnerUsername))
        {
            return new CommonRes
            {
                Code = SystemCodes.Partner.PartnerUsernameMissing,
                Message = SystemMessages.Partner.PartnerUsernameMissing
            };
        }

        // 2. Query the transaction ensuring it belongs to the authenticated partner
        var transaction = await _dbContext.TransactionEntity
            .FirstOrDefaultAsync(t => t.ReferenceId == reference
                                   && t.PartnerUsername == partnerUsername);

        if (transaction is null)
        {
            return new CommonRes
            {
                Code = SystemCodes.Tran.TrnNotFound,
                Message = SystemMessages.Tran.TranNotFound
            };
        }


        // 3. Optional: Check if the transaction can be cancelled (e.g., if it's already completed)
        if (transaction.Status.Equals("SUCCESS", StringComparison.OrdinalIgnoreCase))
        {
            return new CommonRes
            {
                Code = SystemCodes.Tran.NonCancellable,
                Message = SystemMessages.Tran.NonCancellable
            };
        }

        if (transaction.Status.Equals("Cancelled", StringComparison.OrdinalIgnoreCase))
        {
            return new CommonRes
            {
                Code = SystemCodes.Tran.NonCancellable,
                Message = SystemMessages.Tran.TranAlreadyCancelled
            };
        }


        // 4. Update the status and save changes
        transaction.Status = SystemMessages.Tran.Status.Cancelled;

        // If you have an updated timestamp column, update it here too:
        // transaction.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();

        return new CommonRes
        {
            Code = SystemCodes.Success,
            Message = SystemMessages.Tran.TranCancelled
        };
    }
    private static DataResult<bool> ValidateFields(string? partnerUsername, TransactionRequestDto? dto)
    {
        if (dto is null)
        {
            return DataResult<bool>.Error(
                SystemCodes.BodyNull,
                SystemMessages.BodyNull
            );
        }

        if (string.IsNullOrWhiteSpace(partnerUsername))
        {
            return DataResult<bool>.Error(
                SystemCodes.Partner.PartnerUsernameMissing,
                SystemMessages.Partner.PartnerUsernameMissing
                );
        }

        if (string.IsNullOrWhiteSpace(dto.ReferenceId))
        {
            return DataResult<bool>.Error(
                SystemCodes.RequiredField,
                SystemMessages.Tran.TranRefNumberRequired
            );
        }
        if (string.IsNullOrWhiteSpace(dto.Currency))
        {
            return DataResult<bool>.Error(
                SystemCodes.RequiredField,
                SystemMessages.Tran.CurrencyRequired
            );
        }
        if (string.IsNullOrWhiteSpace(dto.ServiceCode))
        {
            return DataResult<bool>.Error(
                SystemCodes.RequiredField,
                SystemMessages.Tran.ServiceRequired
            );
        }
        if (dto.Amount <= 0)
        {
            return DataResult<bool>.Error(
                SystemCodes.RequiredField,
                SystemMessages.Tran.AmountRequired
            );
        }


        return DataResult<bool>.Ok(true);
    }

}