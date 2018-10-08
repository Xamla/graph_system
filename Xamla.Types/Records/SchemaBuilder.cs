using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xamla.Types.Records
{
    public class SchemaBuilder
    {
        List<Field> fieldList;
        DataType dataType;

        public SchemaBuilder(string name = null, DataType dataType = DataType.Class, ICursor declarationItem = null)
        {
            this.Name = name;
            this.dataType = dataType;
            this.DeclarationItem = declarationItem;
            this.fieldList = new List<Field>();
        }

        public string Name
        {
            get;
            set;
        }

        public DataType DataType
        {
            get { return dataType; }
            set
            {
                if (value != DataType.Class && value != DataType.List)
                    throw new ArgumentException("DataType can only be Class or List", "value");
                dataType = value;
            }
        }

        public ICursor DeclarationItem
        {
            get;
            set;
        }

        public void AddField(string name, Schema schema, bool nullable = false, int? maxLength = null, string caption = null, string description = null)
        {
            if (schema == null)
                throw new ArgumentNullException("schema");
            fieldList.Add(new Field { Name = name, Schema = schema, Nullable = nullable, MaxLength = maxLength, Caption = caption, Description = description });
        }

        public void AddFields(IEnumerable<Field> range)
        {
            fieldList.AddRange(range);
        }

        public void AddFieldBoolean(string name, bool nullable = false)
        {
            this.AddField(name, Schema.BuiltIn[BuiltInSchema.Boolean], nullable);
        }

        public void AddFieldInt32(string name, bool nullable = false)
        {
            this.AddField(name, Schema.BuiltIn[BuiltInSchema.Int32], nullable);
        }

        public void AddFieldString(string name, bool nullable = false, int? maxLength = null)
        {
            this.AddField(name, Schema.BuiltIn[BuiltInSchema.String], nullable, maxLength);
        }

        public void AddFieldGuid(string name, bool nullable = false)
        {
            this.AddField(name, Schema.BuiltIn[BuiltInSchema.Guid], nullable);
        }

        public Schema ToSchema()
        {
            var s = new Schema()
            {
                Name = this.Name,
                DataType = this.DataType,
                declarationItem = this.DeclarationItem,
                fields = this.fieldList.Count > 0 ? this.fieldList.ToArray() : null
            };
            s.UpdateLayout();
            return s;
        }

        public static Schema BuildChoice(Type enumType, int? defaultValue)
        {
            var choiceSet = ChoiceSet.FromEnum(enumType);
            if (defaultValue.HasValue)
                choiceSet.Defaults = new List<int> { defaultValue.Value };
            return new SchemaBuilder(enumType.FullName, DataType.Choice, choiceSet.ToEditable(new EditableFactory(null))).ToSchema();
        }

        public static Schema BuildMultiChoice(Type enumType, params int[] defaultValues)
        {
            var choiceSet = ChoiceSet.FromEnum(enumType);
            choiceSet.Defaults = defaultValues.ToList();
            return new SchemaBuilder(enumType.FullName, DataType.MultiChoice, choiceSet.ToEditable(new EditableFactory(null))).ToSchema();
        }
    }
}
