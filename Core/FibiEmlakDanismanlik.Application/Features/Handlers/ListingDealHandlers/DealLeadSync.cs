using System;
using System.Threading.Tasks;
using FibiEmlakDanismanlik.Application.Interfaces.LeadInterfaces;
using FibiEmlakDanismanlik.Domain.Entities;
using FibiEmlakDanismanlik.Domain.Enums;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.ListingDealHandlers
{
    /// <summary>
    /// İşlem adımlarında (kapora, tamamlama, iptal) bağlı müşteri talebinin durumunu ve hareket geçmişini güncelleyen yardımcı sınıf.
    /// </summary>
    internal static class DealLeadSync
    {
        public static async Task AdvanceAsync(
            ILeadRepository repo,
            LeadSource source,
            int leadId,
            LeadStatus? targetStatus,
            string text,
            int? agentId)
        {
            var detail = await repo.GetDetailAsync(source, leadId);
            if (detail == null)
            {
                return;
            }

            // targetStatus null ise veya talebin mevcut durumu hedeften büyük/eşitse (Lost=7 dahil: Lost ise ilerletme) yalnızca Note ekler
            bool shouldUpdateStatus = targetStatus.HasValue
                && (int)detail.Status < (int)targetStatus.Value
                && detail.Status != LeadStatus.Lost;

            if (shouldUpdateStatus)
            {
                var oldStatus = detail.Status;
                await repo.UpdateStatusAsync(source, leadId, targetStatus!.Value);

                var activity = new LeadActivity
                {
                    Source = source,
                    LeadId = leadId,
                    AgentId = agentId,
                    Type = LeadActivityType.StatusChange,
                    Text = text,
                    OldStatus = oldStatus,
                    NewStatus = targetStatus.Value,
                    AssignedAgentId = null,
                    CreatedAtUtc = DateTime.UtcNow
                };

                await repo.AddActivityAsync(activity);
            }
            else
            {
                var activity = new LeadActivity
                {
                    Source = source,
                    LeadId = leadId,
                    AgentId = agentId,
                    Type = LeadActivityType.Note,
                    Text = text,
                    OldStatus = null,
                    NewStatus = null,
                    AssignedAgentId = null,
                    CreatedAtUtc = DateTime.UtcNow
                };

                await repo.AddActivityAsync(activity);
            }
        }
    }
}
