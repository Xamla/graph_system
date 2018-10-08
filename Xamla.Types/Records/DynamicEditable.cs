using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace Xamla.Types.Records
{
    public class DynamicEditable
        : DynamicObject
    {
        IEditable obj;

        public DynamicEditable(IEditable obj)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");
            this.obj = obj;
        }

        public override bool Equals(object obj)
        {
            return object.Equals(this.obj, Unwrap(obj));
        }

        public override int GetHashCode()
        {
            return obj.GetHashCode();
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return obj.Schema.Fields.Select(x => x.Name);
        }

        object Wrap(object value)
        {
            if (value is IEditable)
                return new DynamicEditable(value as IEditable);
            return value;
        }

        object Unwrap(object value)
        {
            if (value is DynamicEditable)
                return ((DynamicEditable)value).obj;
            return value;
        }

        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            if (binder.Type == typeof(IEditable) || (obj != null && binder.Type.GetTypeInfo().IsAssignableFrom(obj.GetType())))
            {
                result = obj;
                return true;
            }

            var list = obj as IEditableList;
            if (list != null)
            {
                if (binder.Type == typeof(System.Collections.IEnumerable))
                {
                    result = (obj as IEditableList).Select(x => Wrap(x.Get()));
                    return true;
                }
            }

            return base.TryConvert(binder, out result);
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            var list = obj as IEditableList;
            if (list != null)
            {
                result = null;

                switch (binder.Name)
                {
                    case "Add":
                        if (args.Length == 1)
                        {
                            list.Add().Set(Unwrap(args[0]));
                            return true;
                        }
                        break;
                    case "InsertAt":
                        if (args.Length == 2 && args[0] is int)
                        {
                            list.InsertAt((int)args[0]).Set(Unwrap(args[1]));
                            return true;
                        }
                        break;
                    case "RemoveAt":
                        if (args.Length == 1 && args[0] is int)
                        {
                            list.RemoveAt((int)args[0]);
                            return true;
                        }
                        break;
                    case "Clear":
                        if (args.Length == 0)
                        {
                            list.Clear();
                            return true;
                        }
                        break;
                }
            }

            return base.TryInvokeMember(binder, args, out result);
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var obj = this.obj;
            if (obj is IEditableVariable)
            {
                obj = obj.Get() as IEditable;
            }

            Field f;
            if (obj.Schema.TryLookup(binder.Name, out f) && obj is IEditableObject)
            {
                result = Wrap((obj as IEditableObject).GetField(f.Index).Get());
                return true;
            }
            else if (obj is IEditableList)
            {
                if (binder.Name == "Count")
                {
                    result = (obj as IEditableList).Count;
                    return true;
                }
            }

            result = null;
            return false;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            Field f;
            if (obj.Schema.TryLookup(binder.Name, out f) && obj is IEditableObject)
            {
                (obj as IEditableObject).GetField(f.Index).Set(Unwrap(value));
                return true;
            }

            return false;
        }

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            if (indexes.Length == 1 && indexes[0] is int && obj is IEditableList)
            {
                result = Wrap((obj as IEditableList).GetItem((int)indexes[0]).Get());
                return true;
            }

            result = null;
            return false;
        }

        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            if (indexes.Length == 1 && indexes[0] is int && obj is IEditableList)
            {
                (obj as IEditableList).GetItem((int)indexes[0]).Set(value);
                return true;
            }

            return false;
        }
    }
}
