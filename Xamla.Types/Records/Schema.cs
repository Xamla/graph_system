using System;
using System.Collections.Generic;
using System.Linq;

namespace Xamla.Types.Records
{
    public sealed class Field
    {
        public int Index;
        public string Name;
        public string Caption;
        public string Description;
        public int Offset;
        public Schema Schema;
        public bool Nullable;
        public int? MaxLength;

        public override string ToString()
        {
            return string.Format("{0} : {1}", this.Name, this.Schema);
        }

        public IEditableObject ToEditable(IEditableFactory factory)
        {
            var editableField = (IEditableObject)factory.Create(BuiltInSchema.Field, false);
            editableField.GetField("SchemaId").Set(this.Schema.Id);
            editableField.GetField("Nullable").Set(this.Nullable);
            editableField.GetField("Name").Set(this.Name);
            editableField.GetField("Caption").Set(this.Caption);
            editableField.GetField("Description").Set(this.Description);
            editableField.GetField("MaxLength").Set(this.MaxLength);
            return editableField;
        }

        public static Field FromCursor(ICursor cursor, ISchemaProvider schemaProvider)
        {
            var field = new Field();
            field.Schema = schemaProvider.GetSchemaById(cursor.GoTo("SchemaId").Get<int>());
            field.Nullable = cursor.GoTo("Nullable").Get<bool>();
            field.Name = cursor.GoTo("Name").Get<string>();
            field.Caption = cursor.GoTo("Caption").Get<string>();
            field.Description = cursor.GoTo("Description").Get<string>();
            var maxLengthField = cursor.GoTo("MaxLength");
            if (!maxLengthField.IsNull)
                field.MaxLength = maxLengthField.Get<int>();
            return field;
        }

        public Field Clone()
        {
            return (Field)MemberwiseClone();
        }
    }

    public class MemberNotFoundException
        : Exception
    {
        string missingMember;
        Schema schema;

        public MemberNotFoundException(string missingMember, Schema schema)
            : base(string.Format("Member not found: '{0}'", missingMember))
        {
            this.missingMember = missingMember;
            this.schema = schema;
        }
    }

    public static class SchemaExtensions
    {
        public static Field Lookup(this Schema source, string name)
        {
            Field result;
            if (!source.TryLookup(name, out result))
                throw new MemberNotFoundException(name, source);
            return result;
        }
    }

    public sealed class Schema
    {
        public static readonly BuiltInSchemas BuiltIn = new BuiltInSchemas();

        public int Id = -1;
        public string Name;
        public string Description;
        public DataType DataType;
        public int FixedSize;
        public int NullBitmapOffset = -1;
        public int VariableSizeOffset = -1;

        internal Field[] fields;
        internal Dictionary<string, Field> fieldLookup;
        internal bool isBuiltIn;
        internal int variableFieldCount;
        internal ICursor declarationItem;

        public Schema()
        {
        }

        public Schema(int id, string name, string description, DataType dataType, Field[] fields, ICursor declarationItem)
        {
            this.Id = id;
            this.Name = name;
            this.Description = description;
            this.DataType = dataType;
            this.fields = fields;
            this.declarationItem = declarationItem;
            this.UpdateLayout();
        }

        public int FieldCount
        {
            get { return fields != null ? fields.Length : 0; }
        }

        public bool IsBuiltIn
        {
            get { return isBuiltIn; }
        }

        public bool NullBitmap
        {
            get { return (NullBitmapOffset >= 0); }
        }

        public bool VariableSize
        {
            get { return (VariableSizeOffset >= 0); }
        }

        public ICursor DeclarationItem
        {
            get { return declarationItem; }
            set
            {
                if (value != null && value.Schema.Id != (int)BuiltInSchema.Item)
                    throw new Exception("Only objects of type Item can be set as schema declaration");
                declarationItem = value;
            }
        }

        public bool TryLookup(string name, out Field result)
        {
            if (fields == null)
            {
                result = null;
                return false;
            }

            lock (fields)
            {
                if (fieldLookup == null)
                {
                    if (this.FieldCount < 10)
                    {
                        result = fields.FirstOrDefault(x => x.Name == name);
                        return (result != null);
                    }
                    else
                    {
                        fieldLookup = new Dictionary<string, Field>();
                        foreach (var x in fields)
                            fieldLookup[x.Name] = x;
                    }
                }
                return fieldLookup.TryGetValue(name, out result);
            }
        }

        public Field this[int index]
        {
            get { return fields[index]; }
        }

        public IEnumerable<Field> Fields
        {
            get { return (fields != null) ? fields : Enumerable.Empty<Field>(); }
            set
            {
                fields = (value != null) ? value.ToArray() : null;
                fieldLookup = null;
            }
        }

        public override bool Equals(object obj)
        {
            Schema other = obj as Schema;
            if (other == null || other.Id != this.Id)
                return false;
            return (this.Id >= 0 || this.Name == other.Name);	// allow comparison by name for unregistered schemas (with Id == -1)
        }

        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }

        public override string ToString()
        {
            if (this.DataType == DataType.List)
            {
                return string.Format("List<{0}>", this[0].Schema.Name);
            }
            else
            {
                return this.Name;
            }
        }

        public IEditableObject ToEditable(IEditableFactory factory)
        {
            var editableSchema = (IEditableObject)factory.Create(BuiltInSchema.Schema, false);
            editableSchema.GetField("Id").Set(this.Id);
            editableSchema.GetField("DataType").Set((int)this.DataType);
            editableSchema.GetField("Name").Set(this.Name);
            editableSchema.GetField("Description").Set(this.Description);
            editableSchema.GetField("DeclarationItem").Set(this.declarationItem);

            if (this.fields != null)
            {
                var fieldList = (IEditableList)editableSchema.GetField("Fields");
                foreach (var f in fields)
                {
                    fieldList.Add().Set(f.ToEditable(factory));
                }
            }

            return editableSchema;
        }

        public static Schema FromCursor(ICursor cursor, ISchemaProvider schemaProvider)
        {
            if (cursor == null || cursor.IsNull)
                return null;

            var schema = new Schema();
            schema.Id = cursor.GoTo("Id").Get<int>();

            schema.DataType = (DataType)cursor.GoTo("DataType").Get();
            schema.Name = cursor.GoTo("Name").Get<string>();
            schema.Description = cursor.GoTo("Description").Get<string>();

            schema.DeclarationItem = cursor.GoTo("DeclarationItem");
            if (schema.DeclarationItem.IsNull)
                schema.DeclarationItem = null;

            var fieldList = cursor.GoTo("Fields");
            if (!fieldList.IsNull)
                schema.fields = fieldList.Children.Select(x => Field.FromCursor(x, schemaProvider)).ToArray();

            schema.UpdateLayout();
            return schema;
        }

        public void UpdateLayout()
        {
            if (this.DataType == DataType.Class)
            {
                if (fields == null)
                    return;

                bool nullBitmap = fields.Any(x => x.Nullable);
                variableFieldCount = fields.Count(x => x.Schema.VariableSize);

                // ensure all fixed size fields come first 
                fields = fields.Where(x => !x.Schema.VariableSize).Concat(fields.Where(x => x.Schema.VariableSize)).ToArray();

                int fixedSize = 0;
                int variableSlotIndex = -1;
                for (int i = 0; i < fields.Length; ++i)
                {
                    var f = fields[i];
                    f.Index = i;

                    if (!f.Schema.VariableSize)
                    {
                        f.Offset = fixedSize;
                        fixedSize += f.Schema.FixedSize;
                    }
                    else
                    {
                        f.Offset = variableSlotIndex--;
                    }
                }

                if (nullBitmap)
                {
                    NullBitmapOffset = fixedSize;
                    fixedSize += (this.fields.Length + 7) / 8;	 // space for null bitmap bytes
                }

                if (variableFieldCount > 0)
                {
                    VariableSizeOffset = fixedSize;
                }

                this.FixedSize = fixedSize;
            }
            else if (this.DataType == DataType.List || this.DataType == DataType.MultiChoice)
            {
                if (this.DataType != DataType.MultiChoice)
                {
                    if (this.FieldCount == 0)
                        fields = new Field[] { new Field() { Nullable = false, Schema = BuiltIn[BuiltInSchema.Int32] } };
                    if (fields.Length != 1)
                        throw new Exception("Invalid field count for list schema");
                }

                this.VariableSizeOffset = 0;
                this.FixedSize = 0;
            }
            else
            {
                if (this.FieldCount != 0)
                    throw new Exception("Primitive types must not have fields specified");

                this.FixedSize = EditablePrimitive.FixedSizeOf(this.DataType);
            }
        }

        public Schema Clone()
        {
            var c = (Schema)this.MemberwiseClone();
            if (this.fields != null)
                c.fields = this.fields.Select(x => x.Clone()).ToArray();
            return c;
        }
    }
}
