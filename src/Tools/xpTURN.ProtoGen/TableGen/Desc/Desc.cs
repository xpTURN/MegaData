
namespace xpTURN.TableGen
{
    public enum TableType
    {
        None,
        Table,
        Data,
        Enum,
    }

    public enum FieldCollections
    {
        None,
        List,
        Map,
    }

    public enum FieldAccess
    {
        None,
        Client,
        Server,
        All,
    }

    public enum FieldObsolete
    {
        None,
        Warning,
        Error,
        Delete,
    }
}
