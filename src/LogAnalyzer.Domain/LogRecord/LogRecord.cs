using LogAnalyzer.Domain.Interfaces;
using System.Net;

namespace LogAnalyzer.Domain.LogRecord
{
    public class LogRecord : IEntity
    {
        private Guid _id;
        private DateTime _requestTime;
        private string _applicationName;
        private string _stage;
        private IPAddress _сlientIpAddress;
        private string _clientName;
        private string _clientVersion;
        private string _path;
        private string _method;
        private string _statusCode;
        private string _statusMessage;
        private string _contentType;
        private int _contentLength;
        private TimeSpan _executionTime;
        private int _memoryUsage;

        protected LogRecord() { }
        public LogRecord(Guid id, DateTime requestTime, string applicationName, string stage,
            IPAddress сlientIpAddress, string clientName, string clientVersion, string path,
            string method, string statusCode, string statusMessage, string contentType,
            int contentLength, TimeSpan executionTime, int memoryUsage)
        {
            _id = id;
            _applicationName = applicationName ?? throw new ArgumentNullException(nameof(applicationName));
            _stage = stage ?? throw new ArgumentNullException(nameof(stage));
            _clientName = clientName ?? throw new ArgumentNullException(nameof(clientName));
            _clientVersion = clientVersion ?? throw new ArgumentNullException(nameof(clientVersion));
            _path = path ?? throw new ArgumentNullException(nameof(path));
            _method = method ?? throw new ArgumentNullException(nameof(method));
            _statusCode = statusCode ?? throw new ArgumentNullException(nameof(statusCode));
            _statusMessage = statusMessage ?? throw new ArgumentNullException(nameof(statusMessage));
            _contentType = contentType ?? throw new ArgumentNullException(nameof(contentType));

            if (contentLength < 0)
            {
                throw new ArgumentNullException(nameof(contentLength));
            }
            if (memoryUsage < 0)
            {
                throw new ArgumentNullException(nameof(memoryUsage));
            }
            _contentLength = contentLength;
            _memoryUsage = memoryUsage;
            _requestTime = requestTime;
            _сlientIpAddress = сlientIpAddress;
            _executionTime = executionTime;
        }

        public Guid Id
        {
            get
            {
                return _id;
            }
            init
            {
                _id = value;
            }
        }
        public DateTime RequestTime
        {
            get
            {
                return _requestTime;
            }
            set
            {
                _requestTime = value;
            }
        }
        public string ApplicationName
        {
            get
            {
                return _applicationName;
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentNullException(nameof(value));
                }
                _applicationName = value;
            }
        }
        public string Stage
        {
            get
            {
                return _stage;
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentNullException(nameof(value));
                }
                _stage = value;
            }
        }
        public IPAddress ClientIpAddress
        {
            get
            {
                return _сlientIpAddress;
            }
            init
            {
                ArgumentException.ThrowIfNullOrEmpty(nameof(value));
                _сlientIpAddress = value;
            }
        }
        public string ClientName
        {
            get
            {
                return _clientName;
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentNullException(nameof(value));
                }
                _clientName = value;
            }
        }
        public string ClientVersion
        {
            get
            {
                return _clientVersion;
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentNullException(nameof(value));
                }
                _clientVersion = value;
            }
        }
        public string Path
        {
            get
            {
                return _path;
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentNullException(nameof(value));
                }
                _path = value;
            }
        }
        public string Method
        {
            get
            {
                return _method;
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentNullException(nameof(value));
                }
                _method = value;
            }
        }
        public string StatusCode
        {
            get
            {
                return _statusCode;
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentNullException(nameof(value));
                }
                _statusCode = value;
            }
        }
        public string StatusMessage
        {
            get
            {
                return _statusMessage;
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentNullException(nameof(value));
                }
                _statusMessage = value;
            }
        }
        public string ContentType
        {
            get
            {
                return _contentType;
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentNullException(nameof(value));
                }
                _contentType = value;
            }
        }
        public int ContentLength
        {
            get
            {
                return _contentLength;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentNullException(nameof(value));
                }
                _contentLength = value;
            }
        }
        public TimeSpan ExecutionTime
        {
            get
            {
                return _executionTime;
            }
            set
            {
                _executionTime = value;
            }
        }
        public int MemoryUsage
        {
            get
            {
                return _memoryUsage;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentNullException(nameof(value));
                }
                _memoryUsage = value;
            }
        }
    }
}



