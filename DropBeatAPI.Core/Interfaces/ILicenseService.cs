using DropBeatAPI.Core.DTOs.Documents;

namespace DropBeatAPI.Core.Interfaces;

public interface ILicenseService
{
    Task<Stream> GenerateLicensePdfAsync(LicenseDocumentDto licenseDto);
}