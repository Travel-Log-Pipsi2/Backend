﻿using Core.Response;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace Core.Interfaces.Authentication
{
    interface IAdditionalAuthMetods
    {
        public string BuildUrl(string token, string email, string path);

        public string CreateValidationErrorMessage(IdentityResult result);

        public Task<ServiceResponse> GetUserTokenResponse(string provider, string providerKey);
    }
}
