namespace Our.Umbraco.Impersonator
{
    public class ImpersonatedUserId
    {
        public int UserId { get; }
        public int ImpersonatingUserId { get; }

        public string SessionId { get; }

        public ImpersonatedUserId(int userId, int impersonatingUserId, string sessionId)
        {
            UserId = userId;
            SessionId = sessionId;
            ImpersonatingUserId = impersonatingUserId;
        }
    }
}
