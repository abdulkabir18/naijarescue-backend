namespace Domain.Enums
{
    public enum AuditActionType
    {
        Created = 1,
        Updated,
        Deleted,
        Accessed,
        Warning,
        SoftDeleted,
        Restored,
        Login,
        Logout,
        Other
    }
}