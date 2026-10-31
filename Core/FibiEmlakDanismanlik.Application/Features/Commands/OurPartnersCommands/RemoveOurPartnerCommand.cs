using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Commands.OurPartnersCommands
{
    public class RemoveOurPartnerCommand : IRequest
    {
        public int Id { get; set; }

        public RemoveOurPartnerCommand(int id)
        {
            Id = id;
        }
    }
}
