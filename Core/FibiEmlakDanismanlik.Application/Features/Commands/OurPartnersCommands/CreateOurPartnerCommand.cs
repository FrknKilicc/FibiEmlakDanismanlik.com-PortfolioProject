using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Commands.OurPartnersCommands
{
    public class CreateOurPartnerCommand : IRequest
    {
        public string? PartnerName { get; set; }
        public string? PartnerLogoUrl { get; set; }
    }
}
