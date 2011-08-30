using System;
using System.Collections.Generic;
using System.Linq;

namespace Rejuicer.Model
{
    internal class RejuicedFileModel
    {
        public RejuicedFileModel(ResourceType resourceType, string requestedFile)
        {
            _requestFor = requestedFile;
            _resourceType = resourceType;
            File = new List<string>();
            FilesMatching = new List<FileMatchModel>();
            fileOrder = new Dictionary<int, object>();
            Cache = true;
            Mode = Mode.Compact;
            _containsPlaceHolder = requestedFile.Contains(FilenameUniquePlaceholder);

            if (_containsPlaceHolder && requestedFile.IndexOf(FilenameUniquePlaceholder) != requestedFile.LastIndexOf(FilenameUniquePlaceholder))
            {
                throw new ArgumentException("Combined filename cannot contain two unique-code placeholders.");
            }
        }

        public Context Context { get; set; }
        public bool Cache { get; set; }

        private bool _containsPlaceHolder;
        public bool ContainsPlaceHolder
        {
            get { return _containsPlaceHolder; }
        }

        private ResourceType _resourceType;
        public ResourceType ResourceType
        {
            get { return _resourceType; }
        }

        private string _requestFor;
        public string RequestFor
        {
            get { return _requestFor; }
        }

        public const string FilenameUniquePlaceholder = "{0}";

        public Mode Mode { get; set; }

        protected readonly Dictionary<int, object> fileOrder;
        protected int currentOrdinal = 0;

        protected List<string> File { get; set; }
        protected List<FileMatchModel> FilesMatching { get; set; }

        public void AddFile(string filename)
        {
            File.Add(filename);
            fileOrder[currentOrdinal++] = filename;
        }

        public void AddFilesMatching(FileMatchModel fileMatch)
        {
            FilesMatching.Add(fileMatch);
            fileOrder[currentOrdinal++] = fileMatch;
        }

        public List<object> OrderedFiles
        {
            get { return fileOrder.Keys.Select(x=> fileOrder[x]).ToList(); }
        }
    }
}
