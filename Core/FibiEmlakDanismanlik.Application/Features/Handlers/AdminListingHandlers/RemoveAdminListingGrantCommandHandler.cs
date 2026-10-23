using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FibiEmlakDanismanlik.Application.Features.Commands.AdminListingCommands;
using FibiEmlakDanismanlik.Application.Interfaces.AdminListingInterfaces;
using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.AdminListingHandlers
{
    /// <summary>
    /// Bir ilana tanımlanmış özel erişim yetkisini kaldıran komut işleyicisi (Yalnızca Sahip/Yönetici).
    /// </summary>
    public class RemoveAdminListingGrantCommandHandler : IRequestHandler<RemoveAdminListingGrantCommand, bool>
    {
        private readonly IAdminListingRepository _adminListingRepository;

        public RemoveAdminListingGrantCommandHandler(IAdminListingRepository adminListingRepository)
        {
            _adminListingRepository = adminListingRepository;
        }

        public async Task<bool> Handle(RemoveAdminListingGrantCommand request, CancellationToken cancellationToken)
        {
            var removed = await _adminListingRepository.RemoveGrantAsync(request.ListingAccessGrantId);
            if (!removed)
            {
                throw new KeyNotFoundException("Silinmek istenen yetki kaydı bulunamadı.");
            }

            return true;
        }
    }
}
