using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using xpTURN.Common;

namespace xpTURN.MegaData
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class TableSetPostProcessAttribute : Attribute
    {
        public Type TargetType { get; set; } = null;
        public int Order { get; set; } = 1000;
        public string FullName => TargetType?.FullName ?? string.Empty;

        public TableSetPostProcessAttribute(Type type = null, int order = 1000)
        {
            TargetType = type;
            Order = order;
        }
    }

    public interface IPostProcess
    {
        public class Context
        {
            public bool IsMainData { get; set; } = false;
            public bool IsEnumerable { get; set; } = false;
            public bool IsList { get; set; } = false;
            public bool IsMap { get; set; } = false;
        }

        int Order { get; set; }
        TableSet TableSet { get; set; }
        Table Table { get; set; }
    }

    public abstract class TableSetPostProcess : IPostProcess
    {
        public int Order { get; set; } = 1000;
        public TableSet TableSet { get; set; }
        public Table Table { get; set; }

        // PostProcess : TableSet > Table > Data
        virtual public void Begin(IPostProcess.Context context, TableSet tableSet) { }
        virtual public void Begin(IPostProcess.Context context, Table table) { }
        virtual public void Begin(IPostProcess.Context context, Data data) { }

        virtual public void PostProcess(IPostProcess.Context context, TableSet tableSet) { }
        virtual public void PostProcess(IPostProcess.Context context, Table table) { }
        virtual public void PostProcess(IPostProcess.Context context, Data data) { }

        virtual public void End(IPostProcess.Context context, TableSet tableSet) { }
        virtual public void End(IPostProcess.Context context, Table table) { }
        virtual public void End(IPostProcess.Context context, Data data) { }
    }

    public abstract class TableSetCheckPostProcess : IPostProcess
    {
        public int Order { get; set; } = 1000;
        public TableSet TableSet { get; set; }
        public Table Table { get; set; }

        // CheckData : TableSet > Table > Data
        virtual public void CheckData(IPostProcess.Context context, TableSet tableSet) { }
        virtual public void CheckData(IPostProcess.Context context, Table table) { }
        virtual public void CheckData(IPostProcess.Context context, Data data) { }
    }

    public partial class TableSet
    {
        protected List<TableSetPostProcess> GetAllPostProcessors()
        {
            //
            List<TableSetPostProcess> postProcessors = new List<TableSetPostProcess>();

            // Load all dependencies
            AssemblyUtils.LoadAllDependencies();

            // Retrieve all types that inherit from TableSetPostProcess
            var postProcessorTypes = AssemblyUtils.GetTypesByBaseName(typeof(TableSetPostProcess).FullName);
            foreach (var type in postProcessorTypes)
            {
                if (Activator.CreateInstance(type) is TableSetPostProcess postProcessor)
                {
                    var attr = (TableSetPostProcessAttribute)Attribute.GetCustomAttribute(type, typeof(TableSetPostProcessAttribute));
                    if (attr != null && attr.TargetType != typeof(TableSet))
                    {
                        if (attr.TargetType != GetType())
                            continue;
                    }

                    if (attr != null)
                        postProcessor.Order = attr.Order;

                    postProcessors.Add(postProcessor);
                }
                else
                {
                    Logger.Log.Tool.Error(DebugInfo.Empty, $"Failed to create instance of {type.FullName}");
                }
            }

            postProcessors.Sort((x, y) => x.Order.CompareTo(y.Order));
            return postProcessors;
        }

        protected List<TableSetCheckPostProcess> GetAllCheckPostProcessors()
        {
            // 
            List<TableSetCheckPostProcess> checkPostProcessors = new List<TableSetCheckPostProcess>();

            // Load all dependencies
            AssemblyUtils.LoadAllDependencies();

            // Retrieve all types that inherit from TableSetPostProcess
            var postProcessorTypes = AssemblyUtils.GetTypesByBaseName(typeof(TableSetCheckPostProcess).FullName);
            foreach (var type in postProcessorTypes)
            {
                if (Activator.CreateInstance(type) is TableSetCheckPostProcess postProcessor)
                {
                    var attr = (TableSetPostProcessAttribute)Attribute.GetCustomAttribute(type, typeof(TableSetPostProcessAttribute));
                    if (attr != null && attr.TargetType != typeof(TableSet))
                    {
                        if (attr.TargetType != GetType())
                            continue;
                    }

                    if (attr != null)
                        postProcessor.Order = attr.Order;

                    checkPostProcessors.Add(postProcessor);
                }
                else
                {
                    Logger.Log.Tool.Error(DebugInfo.Empty, $"Failed to create instance of {type.FullName}");
                }
            }

            checkPostProcessors.Sort((x, y) => x.Order.CompareTo(y.Order));
            return checkPostProcessors;
        }

        protected void CallFunctionData(IPostProcess postProcessor, IPostProcess.Context context, Data data, string functionName)
        {
            Logger.Log.Tool.File(data.DebugInfo);
            InvokeUtils.InvokeFunc(postProcessor, functionName, new Type[] { typeof(IPostProcess.Context), typeof(Data) }, new object[] { context, data });
            Logger.Log.Tool.File(string.Empty);

            // Call Nested Data
            CallFunctionNested(postProcessor, context, data, functionName);
        }

        protected void CallFunctionNested(IPostProcess postProcessor, IPostProcess.Context context, Data data, string functionName)
        {
            var type = data.GetType();
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            // NestedData : Dictionary<key?, Message>
            var mapFields = fields.ToList().FindAll(field => field.IsDictionary() &&
                                                    typeof(Data).IsAssignableFrom(field.FieldType.GetCollectionElementType()));
            foreach (var field in mapFields)
            {
                var mapObj = field.GetValue(data) as System.Collections.IDictionary;
                var collection = mapObj.Values;

                foreach (var obj in collection)
                {
                    context.IsMainData = typeof(Table).IsAssignableFrom(data.GetType());
                    context.IsEnumerable = true;
                    context.IsMap = true;
                    context.IsList = false;
                    CallFunctionData(postProcessor, context, obj as Data, functionName);
                }
            }

            // NestedData : Message field
            var dataFields = fields.ToList().FindAll(field => typeof(Data).IsAssignableFrom(field.FieldType));

            foreach (var field in dataFields)
            {
                var nestedData = field.GetValue(data) as Data;
                if (nestedData == null) continue;

                context.IsMainData = false;
                context.IsEnumerable = false;
                context.IsMap = false;
                context.IsList = false;
                CallFunctionData(postProcessor, context, nestedData, functionName);
            }

            // NestedData : List<Message> field
            var listFields = fields.ToList().FindAll(field => field.IsListArg<Data>());

            foreach (var field in listFields)
            {
                var list = InvokeUtils.GetFieldListEnumerable<Data>(field, data);
                if (list == null) continue;

                foreach (var item in list)
                {
                    context.IsMainData = false;
                    context.IsEnumerable = true;
                    context.IsMap = false;
                    context.IsList = true;
                    CallFunctionData(postProcessor, context, item as Data, functionName);
                }
            }
        }

        protected void CallFunction(IPostProcess postProcessor, IPostProcess.Context context, string functionName)
        {
            //
            Logger.Log.Info($"{functionName}");

            //
            InvokeUtils.SetPropValue(postProcessor, "TableSet", this);
            InvokeUtils.InvokeFunc(postProcessor, functionName, new Type[] { typeof(IPostProcess.Context), typeof(TableSet) }, new object[] { context, this });

            foreach (var table in Tables.Values)
            {
                Logger.Log.Tool.File(table.DebugInfo);
                InvokeUtils.SetPropValue(postProcessor, "Table", table);
                InvokeUtils.InvokeFunc(postProcessor, functionName, new Type[] { typeof(IPostProcess.Context), typeof(Table) }, new object[] { context, table });
                Logger.Log.Tool.File(string.Empty);

                // Call Begin/PostProcess/End for each Data in the Table
                CallFunctionNested(postProcessor, context, table, functionName);
            }
        }

        public void PostProcess()
        {
            //
            IPostProcess.Context context = new IPostProcess.Context();
            List<TableSetPostProcess> postProcessors = GetAllPostProcessors();

            Logger.Log.Info($"PostProcess Start");
            Logger.Log.Info($"");

            // Begin PostProcess
            foreach (var postProcessor in postProcessors)
            {
                Logger.Log.Info($"{postProcessor.GetType().FullName} (Order: {postProcessor.Order})");
                Logger.Log.Indent();

                // Begin
                //
                CallFunction(postProcessor, context, "Begin");

                // PostProcess
                //
                CallFunction(postProcessor, context, "PostProcess");

                // End
                // 
                CallFunction(postProcessor, context, "End");

                Logger.Log.Outdent();
                Logger.Log.Info($"");
            }

            Logger.Log.Info($"PostProcess End");
            Logger.Log.Info($"");
            Logger.Log.Info($"------------------------------------------------------");
        }

        public void CheckData()
        {
            //
            IPostProcess.Context context = new IPostProcess.Context();
            List<TableSetCheckPostProcess> checkPostProcessors = GetAllCheckPostProcessors();

            Logger.Log.Info($"CheckData Start");
            Logger.Log.Info($"");

            //
            foreach (var postProcessor in checkPostProcessors)
            {
                Logger.Log.Info($"{postProcessor.GetType().FullName}.CheckData (Order: {postProcessor.Order})");

                // CheckData
                //
                CallFunction(postProcessor, context, "CheckData");
            }
        }
    }
}