namespace Domain.Enums
{
    public enum AuditActionType
    {
        Created = 1,
        Updated,
        Deleted,
        Accessed,
        Warning,
        Security,
        SoftDeleted,
        Restored,
        Login,
        Logout,
        Other
    }
}