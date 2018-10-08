using System.Collections.Generic;

namespace Xamla.Graph.Models
{
    public enum ThumbnailSize
    {
        None,
        Small,
        Medium,
        Original
    }

    public class ImageStorageModel
    {
        public ThumbnailSize ThumbnailSize { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int ByteCount { get; set; }
        public string ImageFormat { get; set; }
    }

    public class ResultStorageModel
    {
        public string Id { get; set; }
        public string Path { get; set; }
        public string ContentType { get; set; }
        public ImageStorageModel ImageModel { get; set; }
    }

    public abstract class InspectionModelBase
    {
        public IEnumerable<ResultStorageModel> ResultStorageModels { get; set; }

        public string ModuleObjectId { get; set; }
        public string PinObjectId { get; set; }
        public string PinDisplayName { get; set; }
        public string ModuleDisplayName { get; set; }
    }

    public class InspectionModel
        : InspectionModelBase
    {
    }

    public class OutputModel
        : InspectionModelBase
    {
    }
}
