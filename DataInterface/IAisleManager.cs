using System;
using System.Collections.Generic;
using System.Text;

namespace DataInterface
{
    public interface IAisleManager
    {
        void AddAisle(int aisleNumber);
        void RemoveAisle(int aisleID);
        Aisle GetAisleByAisleNumber(int aisleNumber);        
    }
}
