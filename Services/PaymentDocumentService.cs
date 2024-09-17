using AutoMapper;
using FluentValidation;
using GajinoAgencies.Data;
using GajinoAgencies.Dtos;
using GajinoAgencies.Models;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Linq;

namespace GajinoAgencies.Services;


public interface IPaymentDocumentService
{
    ValueTask<bool> Add(AddPaymentDocumentDto dto, CancellationToken stopToken = default);
    ValueTask<ImportExcelResponseDto> ImportExcel(IFormFile file, CancellationToken stopToken = default);
    ValueTask<IEnumerable<PaymentDocumentsResponseDto>> GetAgencyPaymentsById(int AgentId, CancellationToken stopToken = default);
}
public class PaymentDocumentService : IPaymentDocumentService
{
    private readonly AgencyDbContext _ctx;
    private readonly IMapper _mapper;
    private readonly IValidator<PaymentDocumentFromExcelDto> _validator;
    public PaymentDocumentService(
        AgencyDbContext ctx,
        IMapper mapper,
        IValidator<PaymentDocumentFromExcelDto> validator)
    {
        _mapper = mapper;
        _validator = validator;
        _ctx = ctx;
    }
    public async ValueTask<bool> Add(AddPaymentDocumentDto dto, CancellationToken stopToken = default)
    {
        var payment = _mapper.Map<Payment>(dto);
        _ctx.Payments.Add(payment);
        return await _ctx.SaveChangesAsync(stopToken) > 0;
    }

    public async ValueTask<ImportExcelResponseDto> ImportExcel(IFormFile file, CancellationToken stopToken = default)
    {

        //var inValidDocuments = new List<(PaymentDocumentFromExcelDto Record, List<string>? Errors)>();
        var inValidDocuments = new List<PaymentDocumentFromExcelDto>();
        var validDocuments = new List<PaymentDocumentFromExcelDto>();

        using var stream = new MemoryStream();
        await file.CopyToAsync(stream, stopToken);
        using var package = new ExcelPackage(stream);
        var worksheet = package.Workbook.Worksheets[0];
        var rowCount = worksheet.Dimension.Rows;

        for (var row = 2; row <= rowCount; row++) // Assuming the first row is the header
        {
            if (IsRowBlank(worksheet, row)) continue;

            var document = new PaymentDocumentFromExcelDto(
                worksheet.Cells[row, 1].Text ?? "",
                worksheet.Cells[row, 2].Text ?? "",
                worksheet.Cells[row, 3].Text ?? "",
                worksheet.Cells[row, 4].Text ?? "",
                worksheet.Cells[row, 5].Text ?? "",
                worksheet.Cells[row, 6].Text ?? "",
                worksheet.Cells[row, 7].Text ?? ""
            );

            var validationResult = await _validator.ValidateAsync(document, stopToken);

            if (!validationResult.IsValid)
            {
                inValidDocuments.Add((document));
                //inValidDocuments.Add((document, validationResult.Errors.Select(e=>e.ErrorMessage).ToList()));
            }
            else
            {
                validDocuments.Add(document);
            }
        }

        if (validDocuments.Count > 0)
        {
            await AddExcelRowsToDb(validDocuments, _ctx, stopToken);
        }

        return new ImportExcelResponseDto(inValidDocuments);
        //return new ImportExcelResponseDto(inValidDocuments, validDocuments);
    }

    public async ValueTask<IEnumerable<PaymentDocumentsResponseDto>> GetAgencyPaymentsById(int AgentId, CancellationToken stopToken = default)
    {
        var payments = await _ctx.Payments.Where(x => x.AgencyId == AgentId).AsNoTracking().ToListAsync(stopToken);
        var paymentsResponse = _mapper.Map<List<PaymentDocumentsResponseDto>>(payments);
        return paymentsResponse;
    }

    private static bool IsRowBlank(ExcelWorksheet worksheet, int row)
    {
        for (var col = 1; col <= 6; col++)
        {
            if (!string.IsNullOrWhiteSpace(worksheet.Cells[row, col].Text))
            {
                return false;
            }
        }
        return true;
    }

    private async Task AddExcelRowsToDb(IEnumerable<PaymentDocumentFromExcelDto> documentsDto, AgencyDbContext ctx, CancellationToken stopToken = default)
    {
        var mappedDocuments = _mapper.Map<IEnumerable<Payment>>(documentsDto);

        var uniqueAgencies = mappedDocuments.DistinctBy(x => x.AgencyId).Select(s => s.AgencyId);

        var existPayments = await ctx.Payments
            .Where(p => uniqueAgencies.Contains(p.AgencyId))
            .Select(s => new OldPaymentDto(s.AgencyId, s.TraceNo))
            .Distinct()
            .ToListAsync(stopToken);

        var newPayments = mappedDocuments
            //.Where(p => !oldPayments.Any(a => a.AgencyId == p.AgencyId && a.TraceNo == p.TraceNo))
            .Where(p => !existPayments.Contains(new OldPaymentDto(p.AgencyId, p.TraceNo)))
            .ToList();

        if (newPayments.Any())
        {
            ctx.Payments.AttachRange(newPayments);
            await ctx.SaveChangesAsync(stopToken);
        }
    }

    private record OldPaymentDto(int AgencyId, string TraceNo);
}
