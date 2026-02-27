namespace Inno.ViewModels
{
    public class LookupView<T>
    {
        public T Id { get; }
        public string Name { get; }
        public string? EnName { get; }

        public LookupView(T id, string name, string? enName)
        {
            Id = id;
            Name = name;
            EnName = enName;
        }

        public LookupView(T id, string name) : this(id, name, null)
        {
        }
    }
}