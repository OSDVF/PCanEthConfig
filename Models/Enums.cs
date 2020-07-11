using System;
using System.Collections.Generic;
using System.Text;

namespace EthCanConfig.Models
{
    enum LogLevel
    {
        no, normal, all
    }
    enum RouteType
    {
        canin, canout
    }
    enum Protocol
    {
        tcp,udp
    }
    enum ByteOrder
    {
        bigEndian,lowEndian
    }
    enum BitOrder
    {
        MSB,LSB
    }
    enum MaskOperations
    {
        AND,OR
    }
    enum DataTypes
    {
        uchar, uint8, int8, uint16, int16, uint32, int32, str
    }
}
