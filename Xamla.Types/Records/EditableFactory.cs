using Newtonsoft.Json;
using System;

namespace Xamla.Types.Records
{
    public class EditableFactory
        : IEditableFactory
    {
        ISchemaProvider schemaProvider;

        public EditableFactory(ISchemaProvider schemaProvider)
        {
            this.schemaProvider = schemaProvider;
        }

        #region IEditableFactory Members

        public ISchemaProvider SchemaProvider
        {
            get { return schemaProvider; }
        }

        public IEditable Create(Schema schema, bool nullable)
        {
            if (schema == null)
                throw new ArgumentNullException("schema");

            if (EditablePrimitive.IsSupportedType(schema.DataType))
            {
                var x = new EditablePrimitive(this, schema, nullable);
                if (!nullable && schema.DataType == DataType.Choice && schema.DeclarationItem != null)
                {
                    var defaults = schema.DeclarationItem.NavigateTo(FieldPath.Create("Data", "Defaults"));
                    if (!defaults.IsNull && defaults.Count > 0)
                    {
                        x.Set(defaults.GoTo(0).Get<int>());
                    }
                }
                return x;
            }

            if (schema.DataType == DataType.Class)
            {
                if (schema == Schema.BuiltIn[BuiltInSchema.Variable])
                    return new EditableVariable(this, nullable);
                var x = new EditableObject(this, schema, nullable);
                if (!nullable && schema.DeclarationItem != null)
                {

                }
                return x;
            }
            else if (schema.DataType == DataType.List || schema.DataType == DataType.MultiChoice)
            {
                var x = new EditableList(this, schema, nullable);
                if (!nullable && schema.DataType == DataType.MultiChoice && schema.DeclarationItem != null)
                {
                    var defaults = schema.DeclarationItem.NavigateTo(FieldPath.Create("Data", "Defaults"));
                    foreach (var d in defaults.Children)
                    {
                        x.Add().Set(d.Get<int>());
                    }
                }
                return x;
            }
            else
            {
                throw new ArgumentException("Unsupported schema specified", "schema");
            }
        }

        public IEditable Create(BuiltInSchema schemaId, bool nullable)
        {
            return Create(Schema.BuiltIn[schemaId], nullable);
        }

        void LoadFromCursor(Cursor source, IEditable destination)
        {
            if (source.IsNull)
            {
                destination.Set(null);
                return;
            }

            var obj = destination as IEditableObject;
            if (obj != null)
            {
                var schema = source.Schema;
                for (int i = 0; i < schema.FieldCount; ++i)
                {
                    LoadFromCursor(source.GoTo(i, false), obj.GetField(i));
                }
            }
            else
            {
                var v = destination as IEditableVariable;
                if (v != null)
                {
                    System.Diagnostics.Debug.Assert(source.Schema.Id == (int)BuiltInSchema.Variable);
                    v.DataCursor = source.AccessVariable();
                }
                else
                {
                    var list = destination as IEditableList;
                    if (list != null)
                    {
                        list.Clear();   // ensure list has been created (is not null)
                        if (source.Count > 0)
                        {
                            using (var itemEnum = source.GetEnumerator(false))
                            {
                                while (itemEnum.MoveNext())
                                {
                                    LoadFromCursor(itemEnum.Current, list.Add());
                                }
                            }
                        }
                    }
                    else
                    {
                        destination.Set(source.Get());
                    }
                }
            }
        }

        public IEditable Create(Cursor cursor)
        {
            if (cursor.IsNull)
                return this.Create(cursor.Schema, true);

            var e = this.Create(cursor.Schema, false);
            LoadFromCursor(cursor, e);
            return e;
        }

        void LoadFromCursor2(ICursor source, IEditable destination)
        {
            if (source.IsNull)
            {
                destination.Set(null);
                return;
            }

            var obj = destination as IEditableObject;
            if (obj != null)
            {
                var schema = source.Schema;
                for (int i = 0; i < schema.FieldCount; ++i)
                {
                    LoadFromCursor2(source.GoTo(i, false), obj.GetField(i));
                }
            }
            else
            {
                var v = destination as IEditableVariable;
                if (v != null && source.Schema.Id == (int)BuiltInSchema.Variable)
                {
                    ICursor innerSource = source.Get<ICursor>();
                    v.DataSchema = innerSource.Schema;
                    LoadFromCursor2(innerSource, v.Get<IEditable>());
                }
                else
                {
                    var list = destination as IEditableList;
                    if (list != null)
                    {
                        list.Clear();	// ensure list has been created (is not null)
                        if (source.Count > 0)
                        {
                            foreach (var c in source.Children)
                            {
                                LoadFromCursor2(c, list.Add());
                            }
                        }
                    }
                    else
                    {
                        destination.Set(source.Get());
                    }
                }
            }
        }

        public IEditable Create(ICursor cursor)
        {
            if (cursor == null)
                return null;

            IEditable editable = cursor as IEditable;
            if (editable != null)
            {
                return (IEditable)editable.CloneAsCursor();
            }

            if (cursor.IsNull)
                return this.Create(cursor.Schema, true);

            var e = this.Create(cursor.Schema, false);
            LoadFromCursor2(cursor, e);
            return e;
        }

        public IEditable CreateFromMemory(ArraySegment<byte> segment, Schema schema)
        {
            return this.Create(new Cursor(schemaProvider, schema, segment));
        }

        void ReadJsonArray(JsonReader reader, IEditableList destination)
        {
            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.Comment)
                    continue;

                if (reader.TokenType == JsonToken.Null)
                {
                    destination.Set(null);
                    return;
                }

                if (reader.TokenType == JsonToken.StartArray)
                    break;

                throw new Exception("StartArray token expected");
            }

            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonToken.StartObject:
                        if (destination.ItemSchema.DataType != DataType.Class)
                            throw new Exception("StartObject token unexpected (list item is not an object)");

                        ReadJson(reader, destination.Add());
                        break;

                    case JsonToken.StartArray:
                        if (destination.ItemSchema.DataType != DataType.List || destination.ItemSchema.DataType != DataType.MultiChoice)
                            throw new Exception("StartArray token unexpected (list item is not a list)");
                        ReadJson(reader, destination.Add());
                        break;

                    case JsonToken.EndArray:
                        return;

                    case JsonToken.EndObject:
                        throw new Exception("EndObject token was unexpected");

                    case JsonToken.Boolean:
                    case JsonToken.Date:
                    case JsonToken.Integer:
                    case JsonToken.Float:
                    case JsonToken.String:
                    case JsonToken.Null:
                        destination.Add().Set(reader.Value);
                        break;

                    case JsonToken.Bytes:
                        destination.Add().Set(reader.ReadAsBytes());
                        break;
                }
            }
        }

        void ReadJsonVariable(JsonReader reader, IEditableVariable destination)
        {
            bool isObjectVariable = false;

            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.PropertyName)
                {
                    var name = (string)reader.Value;
                    if (name == "@schema")
                    {
                        reader.Read();
                        if (destination.Schema.Id == (int)BuiltInSchema.Variable)
                        {
                            var schemaName = (string)reader.Value;
                            if (schemaName == Schema.BuiltIn[BuiltInSchema.Variable].Name)
                            {
                                isObjectVariable = true;
                            }
                            else
                            {
                                ((IEditableVariable)destination).DataSchema = schemaProvider.GetSchemaByName(schemaName);
                            }
                        }
                    }
                    else if (!isObjectVariable)
                    {
                        IEditable destinationField;
                        if (destination.Get<IEditableObject>().TryGetField((string)reader.Value, out destinationField))
                        {
                            ReadJson(reader, destinationField);
                        }
                        else
                        {
                            reader.Skip();
                        }
                    }
                    else
                    {
                        if (name == "DataSchemaId")
                        {
                            reader.Read();
                            destination.DataSchema = schemaProvider.GetSchemaById(Convert.ToInt32(reader.Value));
                        }
                        else if (name == "DataSchema")
                        {
                            reader.Read();
                            destination.DataSchema = schemaProvider.GetSchemaByName(reader.Value.ToString());
                        }
                        else if (name == "Data")
                        {
                            ReadJson(reader, destination.Get<IEditable>());
                        }
                    }
                }
                else
                {
                    if (ReadJsonValue(reader, destination))
                        return;
                }
            }
        }

        public void ReadJson(JsonReader reader, IEditable destination)
        {
            if (destination.Schema.DataType == DataType.List || destination.Schema.DataType == DataType.MultiChoice)
            {
                ReadJsonArray(reader, (IEditableList)destination);
            }
            else if (destination.Schema.Id == (int)BuiltInSchema.Variable)
            {
                ReadJsonVariable(reader, (IEditableVariable)destination);
            }
            else
            {
                while (reader.Read())
                {
                    if (ReadJsonValue(reader, destination))
                        return;
                }
            }
        }

        bool ReadJsonValue(JsonReader reader, IEditable destination)
        {
            switch (reader.TokenType)
            {
                case JsonToken.PropertyName:
                    if (reader.TokenType == JsonToken.PropertyName)
                    {
                        var name = (string)reader.Value;
                        if (name == "@schema")
                        {
                            reader.Read();
                        }
                        else
                        {
                            IEditable destinationField;
                            if (destination.Get<IEditableObject>().TryGetField(name, out destinationField))
                            {
                                ReadJson(reader, destinationField);
                            }
                            else
                            {
                                reader.Skip();
                            }
                        }
                    }
                    break;

                case JsonToken.StartArray:
                    throw new Exception("StartArray token was unexpected");
                case JsonToken.EndArray:
                    throw new Exception("EndArray token was unexpected");

                case JsonToken.EndObject:
                    return true;

                case JsonToken.Boolean:
                case JsonToken.Date:
                case JsonToken.Integer:
                case JsonToken.Float:
                case JsonToken.Null:
                    destination.Set(reader.Value);
                    return true;

                case JsonToken.String:
                    if (destination.Schema.DataType == DataType.Binary)
                        destination.Set(Convert.FromBase64String((string)reader.Value));
                    //else if (destination.Schema.DataType == DataType.ItemPath)
                    //destination.Set(HierarchyIdExtensions.Read(Convert.FromBase64String((string)reader.Value)));
                    else
                        destination.Set(reader.Value);
                    return true;
            }
            return false;
        }

        public IEditable CreateFromJson(JsonReader reader, Schema schema)
        {
            var result = this.Create(schema, true);
            ReadJson(reader, result);
            return result;
        }

        public IEditableObject CreateItem(bool nullable = false)
        {
            var item = (IEditableObject)this.Create(Schema.BuiltIn[BuiltInSchema.Item], nullable);
            item.GetField((int)ItemLayout.Flags).Set((int)(ItemFlags.CountAsChild | ItemFlags.Propagate));
            return item;
        }

        public IEditableObject CreateItem(Schema dataSchema, bool nullable = false)
        {
            var item = CreateItem(nullable);
            item.GetField((int)ItemLayout.Data).Set(this.Create(dataSchema, false));
            return item;
        }

        public IEditableObject CreateItemFromJson(JsonReader reader, Schema dataSchema)
        {
            var item = (IEditableObject)this.Create(Schema.BuiltIn[BuiltInSchema.Item], true);
            item.GetField((int)ItemLayout.Data).Set(this.Create(dataSchema, false));
            this.ReadJson(reader, item);
            return item;
        }

        public IEditable CreatePrimitive(object value)
        {
            IEditable e = value as IEditable;
            if (e != null)
                return e;

            return new EditablePrimitive(this, value);
        }

        public IEditableVariable CreateVariable(IEditable content = null, bool nullable = false)
        {
            IEditableVariable v = new EditableVariable(this, nullable);
            if (content != null)
                v.Set(content);
            return v;
        }

        #endregion
    }
}
