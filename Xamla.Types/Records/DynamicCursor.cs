using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace Xamla.Types.Records
{
    public class DynamicCursor
        : DynamicObject
    {
        ICursor cursor;

        public DynamicCursor(ICursor cursor)
        {
            if (cursor == null)
                throw new ArgumentNullException("obj");
            this.cursor = cursor;
        }

        public override bool Equals(object obj)
        {
            return object.Equals(this.cursor, Unwrap(obj));
        }

        public override int GetHashCode()
        {
            return cursor.GetHashCode();
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return cursor.Schema.Fields.Select(x => x.Name);
        }

        object Wrap(object value)
        {
            if (value is ICursor)
                return new DynamicCursor(value as ICursor);
            return value;
        }

        object Unwrap(object value)
        {
            if (value is DynamicCursor)
                return ((DynamicCursor)value).cursor;
            return value;
        }

        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            if (cursor == null || cursor.IsNull)
            {
                result = null;
                return (!binder.Type.GetTypeInfo().IsValueType);
            }

            if (binder.Type == typeof(System.Collections.IEnumerable))
            {
                result = cursor.Children.Select(Wrap).ToList();
                return true;
            }

            if (binder.Type == typeof(ICursor) || binder.Type.GetTypeInfo().IsAssignableFrom(cursor.GetType()))
            {
                result = cursor;
                return true;
            }

            var v = cursor.Get();
            if (v != null && binder.Type.GetTypeInfo().IsAssignableFrom(v.GetType()))
            {
                result = v;
                return true;
            }

            return base.TryConvert(binder, out result);
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            return base.TryInvokeMember(binder, args, out result);
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            Field f;
            if (cursor.Schema.TryLookup(binder.Name, out f))
            {
                result = Wrap(cursor.GoTo(f).Get());
                return true;
            }

            if (cursor.Schema.DataType == DataType.List || cursor.Schema.DataType == DataType.MultiChoice)
            {
                if (binder.Name == "Count")
                {
                    result = cursor.Count;
                    return true;
                }
            }

            result = null;
            return false;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            return false;
        }

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            if (indexes.Length == 1 && indexes[0] is int
                && (cursor.Schema.DataType == DataType.List || cursor.Schema.DataType == DataType.MultiChoice))
            {
                result = Wrap(cursor.GoTo((int)indexes[0]).Get());
                return true;
            }

            result = null;
            return false;
        }

        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            return false;
        }
    }
}
