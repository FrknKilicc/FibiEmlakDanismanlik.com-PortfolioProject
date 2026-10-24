using FibiEmlakDanismanlik.Application.Features.Commands.MeCommands;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.MeHandlers
{
    /// <summary>
    /// Giriş yapmış çalışanın profil bilgilerini güncelleyen işleyici.
    /// Rol, e-posta ve aktiflik gibi güvenlik alanları bu işlemle değiştirilemez.
    /// </summary>
    public class UpdateMyProfileCommandHandler : IRequestHandler<UpdateMyProfileCommand>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IRepository<Agent> _agentRepository;

        public UpdateMyProfileCommandHandler(
            ICurrentUserService currentUserService,
            IRepository<Agent> agentRepository)
        {
            _currentUserService = currentUserService;
            _agentRepository = agentRepository;
        }

        public async Task Handle(UpdateMyProfileCommand request, CancellationToken cancellationToken)
        {
            var agentId = _currentUserService.AgentId;
            if (agentId == null)
            {
                throw new UnauthorizedAccessException("Oturum açmış kullanıcı bulunamadı.");
            }

            var agent = await _agentRepository.GetByIdAsync(agentId.Value);
            if (agent == null)
            {
                throw new KeyNotFoundException("Kullanıcı hesabı bulunamadı.");
            }

            // Yalnızca izin verilen 5 alan güncellenir; metinler kırpılır, boşluklar null yapılır
            agent.AgentName = request.AgentName.Trim();
            agent.AgentPhoneNumber = request.AgentPhoneNumber.Trim();
            agent.AgentTitle = string.IsNullOrWhiteSpace(request.AgentTitle) ? null : request.AgentTitle.Trim();
            agent.AgentDescription = string.IsNullOrWhiteSpace(request.AgentDescription) ? null : request.AgentDescription.Trim();
            agent.AgentImgUrl = string.IsNullOrWhiteSpace(request.AgentImgUrl) ? null : request.AgentImgUrl.Trim();

            await _agentRepository.UpdateAsync(agent);
        }
    }
}
