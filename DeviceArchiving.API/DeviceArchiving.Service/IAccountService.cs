using DeviceArchiving.Data;
using DeviceArchiving.Data.Dto;
using DeviceArchiving.Data.Dto.Users;

namespace DeviceArchiving.Service
{
    public interface IAccountService
    {
        Task<BaseResponse<AuthenticationResponse>> AuthenticateAsync( AuthenticationRequest request);
        Task<BaseResponse<string>> AddUserAsync(AuthenticationRequest request);
    }
}
