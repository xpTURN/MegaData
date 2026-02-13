using System;

namespace xpTURN.Converter.AssignValue
{
    public class AssignFieldFactory
    {
        public static IAssignField GetAssignValue(Type fieldType)
        {
            if (fieldType.IsList())
            {
                return ListAssignField.Default;
            }
            else if (fieldType.IsDictionary())
            {
                return MapFieldAssignField.Default;
            }
            else
            {
                return ValueAssignField.Default;
            }
        }
    }
}
