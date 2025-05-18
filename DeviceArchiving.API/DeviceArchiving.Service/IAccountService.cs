using DeviceArchiving.Data;
using DeviceArchiving.Data.Dto;

namespace DeviceArchiving.Service
{
    public interface IAccountService
    {
        Task<BaseResponse<string>> AuthenticateAsync(AuthenticationRequest request);
        Task<BaseResponse<string>> AddUserAsync(AuthenticationRequest request);
    }
}
