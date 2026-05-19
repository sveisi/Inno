namespace Inno.Types
{
    public static class UserRoleName
    {
        public const string Admin = nameof(UserRoleType.Admin);
        public const string Storekeeper = nameof(UserRoleType.Storekeeper);
        public const string Customer = nameof(UserRoleType.Customer);

        public const string Admin_Storekeeper = nameof(UserRoleType.Admin)+","+ nameof(UserRoleType.Storekeeper);
        public const string Admin_Storekeeper_Customer = nameof(UserRoleType.Admin) + "," + nameof(UserRoleType.Storekeeper) + "," + nameof(UserRoleType.Customer);
    }
}