using System;
using System.Collections.Generic;

namespace xpTURN.TableGen
{
    interface ICodeGenerator
    {
        void WriteFileHeader(GeneratorContext ctx);

        void WriteFileFooter(GeneratorContext ctx);

        void WriteEnumHeader(GeneratorContext ctx, TableDesc tableDef);

        void WriteEnumValue(GeneratorContext ctx, FieldDesc fieldDef);

        void WriteEnumFooter(GeneratorContext ctx, TableDesc tableDef);

        void WriteEnum(GeneratorContext ctx, TableDesc tableDef);

        void WriteMessageHeader(GeneratorContext ctx, TableDesc tableDef);

        void WriteProtobufFunction(GeneratorContext ctx, TableDesc tableDef);

        void WriteField(GeneratorContext ctx, TableDesc tableDef, FieldDesc fieldDef);

        void WriteMessageFooter(GeneratorContext ctx, TableDesc tableDef);

        void WriteMessage(GeneratorContext ctx, TableDesc tableDef);

        string ExportFile(GeneratorContext ctx, string fileName, List<TableDesc> listTable);
    }
}
