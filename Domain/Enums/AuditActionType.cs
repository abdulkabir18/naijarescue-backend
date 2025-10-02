namespace Domain.Enums
{
    public enum AuditActionType
    {
        Created = 1,
        Updated,
        Deleted,
        SoftDeleted,
        Restored,
        Login,
        Logout,
        Other
    }
}