using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Commands.OurPartnersCommands
{
    public class UpdateOurPartnerCommand : IRequest
    {
        public int Id { get; set; }
        public string? PartnerName { get; set; }
        public string? PartnerLogoUrl { get; set; }
        public bool IsActive { get; set; } = true;
        public int DisplayOrder { get; set; }
    }
}
