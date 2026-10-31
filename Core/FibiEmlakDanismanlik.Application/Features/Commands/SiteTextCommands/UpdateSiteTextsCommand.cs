using System.Collections.Generic;
using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Commands.SiteTextCommands
{
    /// <summary>
    /// Site metinlerini anahtar → değer olarak güncelleme komutu (yalnızca gönderilen anahtarlar değişir).
    /// </summary>
    public class UpdateSiteTextsCommand : IRequest
    {
        public Dictionary<string, string?> Values { get; set; } = new();
    }
}
