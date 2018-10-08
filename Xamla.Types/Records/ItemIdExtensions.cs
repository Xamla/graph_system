using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Xamla.Utilities;

namespace Xamla.Types.Records
{
    public static class ItemIdExtensions
    {
        public static byte[] ToArray(this ItemId source)
        {
            if (source.IsNull)
                return null;

            var buffer = new MemoryStream();
            source.Write(new BinaryWriter(buffer));
            return buffer.ToArray();
        }

        public static ItemId FromBase64(string s)
        {
            return Read(Convert.FromBase64String(s));
        }

        public static ItemId Read(byte[] data)
        {
            var id = new ItemId();
            id.Read(new BinaryReader(new MemoryStream(data)));
            return id;
        }

        public static ItemId Read(byte[] data, int index, int count)
        {
            var id = new ItemId();
            id.Read(new BinaryReader(new MemoryStream(data, index, count)));
            return id;
        }

        static readonly Regex hierarchyIdPattern = new Regex(@"^/(-?\d+(\.-?\d+)*/)*$");
        public static bool TryParse(string input, out ItemId result)
        {
            result = ItemId.Null;
            if (string.IsNullOrWhiteSpace(input) || !hierarchyIdPattern.IsMatch(input))
            {
                if (string.Compare(input, "NULL", true) == 0)
                    return true;
                return false;
            }

            result = ItemId.Parse(input);
            return true;
        }

        public static ItemId FromBase64Url(string token)
        {
            if (token == null)
                return ItemId.Null;

            return token != "~" ? Read(Base64Url.Parse(token)) : ItemId.GetRoot();
        }

        public static string ToBase64Url(this ItemId id)
        {
            if (id.IsNull)
                return null;

            var s = id.ToArray().ToBase64Url();
            return s.Length > 0 ? s : "~";
        }

        public static ItemId Append(this ItemId path, params int[] descendantIds)
        {
            return Append(path, (IEnumerable<int>)descendantIds);
        }

        /// <summary>
        /// Split a hierarchy into an integer array. The input hierarchy id value must not contain any sub elements
        /// (e.g. it's string repersentation must not contain '.' characters).
        /// </summary>
        /// <param name="path">A hierarchyid with simple integer elements without sub elements.</param>
        /// <returns>Array of ID values from root to descendants.</returns>
        public static int[] ToInt32Array(this ItemId path)
        {
            if (path.IsNull)
                return null;

            return path.ToString()
                .Split(new [] { '/' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse)
                .ToArray();
        }

        public static int First(this ItemId path)
        {
            if (path.IsNull)
                throw new ArgumentNullException("path");

            ItemId first;

            int level = (int)path.GetLevel();
            if (level > 1)
                first = path.GetAncestor(level - 1);
            else if (level == 1)
                first = path;
            else
                throw new ArgumentException("Path does not contain any segment.", "path");

            return int.Parse(first.ToString().Trim('/'));
        }

        public static int Last(this ItemId path)
        {
            if (path.IsNull)
                throw new ArgumentNullException("path");

            ItemId last;

            int level = (int)path.GetLevel();
            if (level > 1)
                last = path.MakeRelativeToAncestor(path.GetParent());
            else if (level == 1)
                last = path;
            else
                throw new ArgumentException("Path does not contain any segment.", "path");

            return int.Parse(last.ToString().Trim('/'));
        }

        public static ItemId Append(this ItemId path, IEnumerable<int> descendantIds)
        {
            var sb = new StringBuilder(path.ToString());
            foreach (var i in descendantIds)
                sb.AppendFormat(CultureInfo.InvariantCulture, "{0:D}/", i);
            return ItemId.Parse(sb.ToString());
        }

        public static ItemId GetNextSibling(this ItemId id)
        {
            if (id.IsNull)
                return ItemId.Null;
            var parent = id.GetAncestor(1);
            if (parent.IsNull)
                throw new ArgumentException("HierarchyId root value ('/') has no siblings.", "id");
            return parent.GetDescendant(id, ItemId.Null);
        }

        public static ItemId MakeRelativeToAncestor(this ItemId id, ItemId ancestor)
        {
            return id.GetReparentedValue(ancestor, ItemId.GetRoot());
        }

        public static ItemId Append(this ItemId path, ItemId relativePath)
        {
            return relativePath.GetReparentedValue(ItemId.GetRoot(), path);
        }

        //public static ItemId AddHierarchyId(this SqlParameterCollection collection, string parameterName, SqlHierarchyId value)
        //{
        //    return collection.Add(new SqlParameter(parameterName, System.Data.SqlDbType.Udt) { SqlValue = value, UdtTypeName = "HierarchyId" });
        //}

        //public static SqlParameter AddHierarchyIdList(this SqlParameterCollection collection, string parameterName, IEnumerable<SqlHierarchyId> itemIds)
        //{
        //    DataTable itemRecords = new DataTable();
        //    itemRecords.Columns.Add("Id");

        //    foreach (var x in itemIds)
        //        itemRecords.Rows.Add(x);

        //    var tableParam = collection.AddWithValue(parameterName, itemRecords);
        //    tableParam.SqlDbType = SqlDbType.Structured;
        //    tableParam.TypeName = "dbo.IdList";

        //    return tableParam;
        //}

        //public static SqlParameter AddUniqueHierarchyIdList(this SqlParameterCollection collection, string parameterName, IEnumerable<SqlHierarchyId> itemIds)
        //{
        //    DataTable itemRecords = new DataTable();
        //    itemRecords.Columns.Add("Id");

        //    foreach (var x in itemIds)
        //        itemRecords.Rows.Add(x);

        //    var tableParam = collection.AddWithValue(parameterName, itemRecords);
        //    tableParam.SqlDbType = SqlDbType.Structured;
        //    tableParam.TypeName = "dbo.UniqueIdList";
        //    return tableParam;
        //}

        //public static SqlParameter AddStringList(this SqlParameterCollection collection, string parameterName, IEnumerable<string> strings)
        //{
        //    DataTable itemRecords = new DataTable();
        //    itemRecords.Columns.Add("Value");

        //    foreach (var x in strings)
        //        itemRecords.Rows.Add(x);

        //    var listParam = collection.AddWithValue(parameterName, itemRecords);
        //    listParam.SqlDbType = SqlDbType.Structured;
        //    listParam.TypeName = "dbo.StringList";
        //    return listParam;
        //}

        //public static SqlParameter AddHierarchyId(this SqlParameterCollection collection, string parameterName, ParameterDirection direction)
        //{
        //    return collection.Add(new SqlParameter(parameterName, System.Data.SqlDbType.Udt) { Direction = direction, UdtTypeName = "HierarchyId" });
        //}

        public static ItemId FromDate(DateTime date, bool includeTime = true)
        {
            if (includeTime)
                return ItemId.Parse(string.Format(CultureInfo.InvariantCulture, "/{0:D}/{1:D}/{2:D}/{3:D}/{4:D}/{5:D}/", date.Year - 2000, date.Month, date.Day, date.Hour, date.Minute, date.Second));
            else
                return ItemId.Parse(string.Format(CultureInfo.InvariantCulture, "/{0:D}/{1:D}/{2:D}/", date.Year - 2000, date.Month, date.Day));
        }

        public static ItemId Parent(this ItemId id)
        {
            return id.GetAncestor(1);
        }

        public static IEnumerable<ItemId> Ancestors(this ItemId id)
        {
            if (id.IsNull)
                yield break;
            
            while (true)
            {
                id = id.GetAncestor(1);
                if (id.IsNull)
                    break;
                yield return id;
            }
        }

        public static ItemId NextSibling(this ItemId id)
        {
            return id.Parent().GetDescendant(id, ItemId.Null);
        }

        public static ItemId GetParent(this ItemId id)
        {
            return id.GetAncestor(1);
        }
    }
}
