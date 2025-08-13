using System;
using System.Reflection;

using xpTURN.Common;

namespace xpTURN.Converter.AssginValue
{
    public class ListAssignField : IAssignField
    {
        public static ListAssignField Default = new ();

        public bool AssignValue(FieldInfo fieldInfo, object target, object value)
        {
            // Call the Add method of List<T>
            try
            {
                Type valType = fieldInfo.FieldType.GetGenericArguments()[0];
                object fieldObject = fieldInfo.GetValue(target);
                InvokeUtils.InvokeFunc(fieldObject, "Add", new Type[] { valType }, new object[] { value });
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
