namespace Domain.Enums
{
    public enum AuditActionType
    {
        Created = 1,
        Updated,
        Deleted,
        Accessed,
        SoftDeleted,
        Restored,
        Login,
        Logout,
        Other
    }
}