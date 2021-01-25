namespace Todo.Shared
{
    public abstract record AbstractConnectionString(string Value)
    {
        public static implicit operator string(
            AbstractConnectionString connectionString)
        {
            return connectionString.Value;
        }
    }
}