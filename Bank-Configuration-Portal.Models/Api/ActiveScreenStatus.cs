using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank_Configuration_Portal.Models.Api
{
    public enum ActiveScreenStatus : int
    {
        Ok = 0,
        InvalidBranch = -1,
        NoActiveScreen = -2
    }

}
