using Capital.GSG.FX.Data.Core.ContractData;
using Capital.GSG.FX.Data.Core.ExecutionData;
using Capital.GSG.FX.Data.Core.OrderData;

namespace StratedgemeMonitor.Utils
{
    public class FormatUtils
    {
        public static string FormatRate(Cross cross, double? rate)
        {
            if (!rate.HasValue)
                return null;
            else
            {
                if (cross.IsJpyCross())
                    return rate.Value.ToString("N3");
                else
                    return rate.Value.ToString("N5");
            }
        }

        public static string ShortenOrigin(OrderOrigin origin)
        {
            switch (origin)
            {
                case OrderOrigin.PositionOpen:
                    return "PO";
                case OrderOrigin.PositionClose:
                    return "PC";
                case OrderOrigin.PositionClose_ContStop:
                    return "PCS";
                case OrderOrigin.PositionClose_ContLimit:
                    return "PCL";
                case OrderOrigin.PositionClose_TE:
                    return "PCT";
                case OrderOrigin.PositionClose_CircuitBreaker:
                    return "PCB";
                case OrderOrigin.PositionReverse_Close:
                    return "PRC";
                case OrderOrigin.PositionReverse_Open:
                    return "PRO";
                case OrderOrigin.PositionDouble:
                    return "PD";
                default:
                    return null;
            }
        }

        public static string ShortenPair(Cross pair)
        {
            return pair.ToString().Replace("USD", "");
        }

        public static string ShortenSide(ExecutionSide side)
        {
            switch (side)
            {
                case ExecutionSide.BOUGHT:
                    return "B";
                case ExecutionSide.SOLD:
                    return "S";
                default:
                    return null;
            }
        }

        public static string ShortenSide(OrderSide side)
        {
            switch (side)
            {
                case OrderSide.BUY:
                    return "B";
                case OrderSide.SELL:
                    return "S";
                default:
                    return null;
            }
        }

        public static string ShortenStatus(OrderStatusCode status)
        {
            switch (status)
            {
                case OrderStatusCode.PendingSubmit:
                    return "PS";
                case OrderStatusCode.PendingCancel:
                    return "PCxl";
                case OrderStatusCode.PreSubmitted:
                    return "PreSub";
                case OrderStatusCode.Submitted:
                    return "Sub";
                case OrderStatusCode.ApiCanceled:
                    return "ApiCxld";
                case OrderStatusCode.Cancelled:
                    return "Cxld";
                case OrderStatusCode.Filled:
                    return "Fld";
                case OrderStatusCode.Inactive:
                    return "Inac";
                default:
                    return null;
            }
        }

        public static string ShortenType(OrderType type)
        {
            switch (type)
            {
                case OrderType.LIMIT:
                    return "LMT";
                case OrderType.MARKET:
                    return "MKT";
                case OrderType.STOP:
                    return "STP";
                case OrderType.TRAILING_STOP:
                    return "T_STP";
                case OrderType.TRAILING_MARKET_IF_TOUCHED:
                    return "T_MKT_TOUCH";
                default:
                    return null;
            }
        }
    }
}
