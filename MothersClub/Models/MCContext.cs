using System.Data.Entity;

namespace MothersClub.Models
{
    public class MCContext : DbContext
    {
        public MCContext() : base("MCContext")
        {

        }
        public virtual DbSet<Campaign> campaigns { get; set; }
        public virtual DbSet<CampaignRule> campaignRules { get; set; }
        public virtual DbSet<ExceptionLog> exceptionLogs { get; set; }
        public virtual DbSet<SystemLog> systemLogs { get; set; }
        public virtual DbSet<UserInvitation> userInvitations { get; set; }
        public virtual DbSet<UserReference> userReferences { get; set; }
        public virtual DbSet<UserReferenceOrder> userReferenceOrders { get; set; }

    }
}
