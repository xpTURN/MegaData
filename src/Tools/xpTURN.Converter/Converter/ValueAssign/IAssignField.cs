using System;
using System.Reflection;

namespace xpTURN.Converter.AssignValue
{
    public interface IAssignField
    {
        bool AssignValue(FieldInfo fieldInfo, object target, object value);
    }
}
