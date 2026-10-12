using System;

namespace FibiEmlakDanismanlik.Application.Features.Results.AuthResults
{
    public class GetLoginResult
    {
        public string Token { get; set; }
        public DateTime ExpireDate { get; set; }
    }
}
