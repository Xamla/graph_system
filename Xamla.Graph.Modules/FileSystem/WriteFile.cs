using System;
using System.IO;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;
using Xamla.Types;
using Xamla.Types.Sequence;
using Xamla.Utilities;

namespace Xamla.Graph.Modules.FileSystem
{
    public enum WriteFileMode
    {
        CreateNew,
        OverwriteOrCreate,
        Append
    }

    [Module(ModuleType = "Xamla.IO.WriteFile")]
    public class WriteFile
        : ModuleBase
    {
        GenericInputPin pathPin;
        GenericInputPin modePin;
        GenericInputPin writablePin;
        GenericInputPin appendGuidPin;

        GenericInputPin createDirectoryPin;

        public WriteFile(IGraphRuntime runtime)
            : base(runtime)
        {
            pathPin = this.AddInputPin("fileName", PinDataTypeFactory.Create<string>(string.Empty, WellKnownEditors.FileSelector), PropertyMode.Default, flags: PinFlags.ResolvePath);
            modePin = this.AddInputPin("mode", PinDataTypeFactory.CreateEnum(WriteFileMode.OverwriteOrCreate, WellKnownEditors.DropDown), PropertyMode.Default);
            writablePin = this.AddInputPin("writable", PinDataTypeFactory.Create<IWritable>(), PropertyMode.Never);
            appendGuidPin = this.AddInputPin("appendGuid", PinDataTypeFactory.CreateBoolean(false), PropertyMode.Default);
            createDirectoryPin = this.AddInputPin("createDirectory", PinDataTypeFactory.CreateBoolean(false), PropertyMode.Default);
            EnableVirtualOutputPin();
        }

        protected async override Task<object[]> EvaluateInternal(object[] inputs, CancellationToken cancel)
        {
            var path = (string)inputs[0];
            var mode = (WriteFileMode)inputs[1];
            var writable = (IWritable)inputs[2];
            var appendGuid = (bool)inputs[3];
            var createDirectory = (bool)inputs[4];

            await Write(path, mode, writable, appendGuid, createDirectory, cancel).ConfigureAwait(false);

            return new object[0];
        }

        public async Task Write(string path, WriteFileMode mode, IWritable writable, bool appendGuid, bool createDirectory, CancellationToken cancel)
        {
            if (appendGuid)
            {
                var p = new PosixPath(path);
                if (p.IsEmpty)
                {
                    path = Guid.NewGuid().ToString("N");
                }
                else
                {
                    path = p.FileNameWithoutExtension + Guid.NewGuid().ToString("N") + p.Extension;
                }
            }

            path = this.Graph.ResolvePath(path);

            if (createDirectory)
            {
                var directoryPath = Path.GetDirectoryName(path);
                Directory.CreateDirectory(directoryPath);
            }

            using (var file = Open(path, mode))
            {
                using (writable)
                {
                    await writable.WriteTo(file, cancel).ConfigureAwait(false);
                }
            }
        }

        public IInputPin FileNamePin
        {
            get { return pathPin; }
        }

        public string FileName
        {
            get { return this.Properties.Get<string>(pathPin.Id); }
            set { this.Properties.Set(pathPin.Id, value); }
        }

        public IInputPin ModePin
        {
            get { return modePin; }
        }

        public WriteFileMode Mode
        {
            get { return this.Properties.Get<WriteFileMode>(modePin.Id); }
            set { this.Properties.SetEnum(modePin.Id, value); }
        }

        public IInputPin WritablePin
        {
            get { return writablePin; }
        }

        Stream Open(string path, WriteFileMode mode)
        {
            var fs = this.Runtime.FileSystem;

            switch (mode)
            {
                case WriteFileMode.Append:
                    return fs.Open(path, FileMode.Append, FileAccess.Write);
                case WriteFileMode.OverwriteOrCreate:
                    return fs.Open(path, FileMode.Create, FileAccess.Write);
                default:
                    return fs.Open(path, FileMode.CreateNew, FileAccess.Write);
            }
        }
    }
}
