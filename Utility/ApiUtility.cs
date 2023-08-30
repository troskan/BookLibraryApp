namespace BookLibraryApp.Utility
{
    public static class ApiUtility
    {
        public enum ApiType
        {
            GET,
            POST,
            PUT,
            DELETE
        }
        public static string SessionToken = "JWTToken";
    }
}
