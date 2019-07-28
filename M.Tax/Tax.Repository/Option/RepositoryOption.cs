using System;
using System.Collections.Generic;
using System.Text;

namespace Tax.Repository
{
    public class RepositoryOption
    {
        string _connStr;
        public RepositoryOption(string connStr)
        {
            _connStr = connStr;
        }

        public string ConnectionString
        {
            get
            {
                return _connStr;
            }
        }

    }
}
