using FibiEmlakDanismanlik.Application.Exceptions;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Domain.Entities;
using FibiEmlakDanismanlik.Domain.Enums;

namespace FibiEmlakDanismanlik.Application.Services.Auth
{
    /// <summary>
    /// Personel yönetiminde rol ve yetki kurallarını denetleyen statik yardımcı sınıf.
    /// </summary>
    public static class AgentManagementRules
    {
        /// <summary>
        /// Geçerli kullanıcının oturum açmış olup olmadığını denetler. Oturum yoksa istisna fırlatır.
        /// </summary>
        public static (int ActorId, AgentRole ActorRole) EnsureAuthenticated(ICurrentUserService currentUser)
        {
            if (currentUser == null || !currentUser.AgentId.HasValue || !currentUser.Role.HasValue)
            {
                throw new AgentManagementForbiddenException("Oturum bilgisi bulunamadı.");
            }

            return (currentUser.AgentId.Value, currentUser.Role.Value);
        }

        /// <summary>
        /// İşlemi gerçekleştiren kullanıcının hedef personeli yönetme yetkisini denetler.
        /// Sahip (Owner) herkesi yönetebilir.
        /// Yönetici (Manager) yalnızca Danışman (Consultant) hesaplarını yönetebilir; Sahip ve Yönetici hesaplarını değiştiremez.
        /// </summary>
        public static void EnsureCanManage(AgentRole actorRole, int actorId, Agent target)
        {
            if (target == null)
            {
                throw new AgentNotFoundException();
            }

            if (actorRole == AgentRole.Owner)
            {
                return;
            }

            if (actorRole == AgentRole.Manager)
            {
                if (target.Role != AgentRole.Consultant)
                {
                    throw new AgentManagementForbiddenException("Bu hesabı yönetme yetkiniz yok.");
                }
                return;
            }

            throw new AgentManagementForbiddenException("Bu hesabı yönetme yetkiniz yok.");
        }

        /// <summary>
        /// İşlemi gerçekleştiren kullanıcının belirtilen rolü atama yetkisini denetler.
        /// Sahip her rolü atayabilir.
        /// Yönetici yalnızca Danışman rolü atayabilir; Danışman hiçbir rol atayamaz.
        /// </summary>
        public static void EnsureCanAssignRole(AgentRole actorRole, AgentRole newRole)
        {
            if (actorRole == AgentRole.Owner)
            {
                return;
            }

            if (actorRole == AgentRole.Manager)
            {
                if (newRole != AgentRole.Consultant)
                {
                    throw new AgentManagementForbiddenException("Yöneticiler yalnızca Danışman rolü atayabilir.");
                }
                return;
            }

            throw new AgentManagementForbiddenException("Rol atama yetkiniz yok.");
        }
    }
}
