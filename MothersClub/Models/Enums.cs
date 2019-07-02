
namespace MothersClub.Models
{
    public class Enums
    {
        public enum InvitationStatus
        {
            delivered = 1,
            notDelivered = 0
        }
        public enum ReferenceStatus
        {
            pending = 0,
            active = 1,
            passive = -1
        }
        public enum RuleTypes
        {
            percentage = 0,
            amount = 1
        }
        public enum OrderState
        {
            paid = 1,
            canceled = 0
        }
    }
}