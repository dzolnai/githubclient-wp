using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GithubClient.Utils
{
    public class NavigationHelper
    {
        private static object _navigationData;
        private static DataType _navigationDataType;

        public enum DataType
        {
            REPOSITORY_LIST, REPOSITORY, FILE, DOWNLOADED_FILE, NONE
        }

        public static void setNavigationData(Object data, DataType type){
            _navigationDataType = type;
            _navigationData = data;
        }

        public static void resetData()
        {
            _navigationData = null;
            _navigationDataType = DataType.NONE;
        }

        public static bool hasData()
        {
            if (_navigationData == null || _navigationDataType == DataType.NONE)
            {
                return false;
            }
            return true;
        }

        public static object getData()
        {
            return _navigationData;
        }

        public static DataType getDataType()
        {
            return _navigationDataType;
        }
    }
}
