using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Portal.Services
{
    public class BaseService
    {

        protected IdFilter Map(IdFilter filter, FilterPermissionDefinition filterDefinition)
        {
            if (filter == null)
                filter = new IdFilter();
            if (filter.In == null)
                filter.In = filterDefinition.Ids;
            else
                filter.In.AddRange(filterDefinition.Ids);
            filter.In = filter.In.Distinct().ToList();
            return filter;
        }

        protected StringFilter Map(StringFilter filter, FilterPermissionDefinition filterDefinition)
        {
            if (filter == null)
                filter = new StringFilter();
            if (string.IsNullOrEmpty(filter.StartWith))
                filter.StartWith = filterDefinition.PrefixString;
            else
                filter.StartWith = filter.StartWith.StartsWith(filterDefinition.PrefixString)
                    ? filter.StartWith : filterDefinition.PrefixString;

            if (string.IsNullOrEmpty(filter.EndWith))
                filter.EndWith = filterDefinition.SuffixString;
            else
                filter.EndWith = filter.EndWith.EndsWith(filterDefinition.PrefixString)
                    ? filter.EndWith : filterDefinition.SuffixString;
            return filter;
        }

        protected DateFilter Map(DateFilter filter, FilterPermissionDefinition filterDefinition)
        {
            if (filter == null)
                filter = new DateFilter();
            if (filter.GreaterEqual.HasValue)
                filter.GreaterEqual = filter.GreaterEqual.Value > filterDefinition.StartDate
                        ? filter.GreaterEqual.Value : filterDefinition.StartDate;
            else
                filter.GreaterEqual = filterDefinition.StartDate;

            if (filter.LessEqual.HasValue)
                filter.LessEqual = filter.LessEqual.Value < filterDefinition.EndDate
                        ? filter.LessEqual : filterDefinition.EndDate;
            else
                filter.GreaterEqual = filterDefinition.EndDate;
            return filter;
        }

        protected LongFilter Map(LongFilter filter, FilterPermissionDefinition filterDefinition)
        {
            if (filter == null)
                filter = new LongFilter();
            if (filter.GreaterEqual.HasValue)
                filter.GreaterEqual = filter.GreaterEqual.Value > filterDefinition.StartLong
                        ? filter.GreaterEqual : filterDefinition.StartLong;
            else
                filter.GreaterEqual = filterDefinition.StartLong;

            if (filter.LessEqual.HasValue)
                filter.LessEqual = filter.LessEqual.Value < filterDefinition.EndLong
                        ? filter.LessEqual : filterDefinition.EndLong;
            else
                filter.LessEqual = filterDefinition.EndLong;
            return filter;
        }

        protected DecimalFilter Map(DecimalFilter filter, FilterPermissionDefinition filterDefinition)
        {
            if (filter == null)
                filter = new DecimalFilter();

            if (filter.GreaterEqual.HasValue)
                filter.GreaterEqual = filter.GreaterEqual.Value > filterDefinition.StartDecimal
                        ? filter.GreaterEqual : filterDefinition.StartDecimal;
            else
                filter.GreaterEqual = filterDefinition.StartLong;


            if (filter.LessEqual.HasValue)
                filter.LessEqual = filter.LessEqual.Value < filterDefinition.EndDecimal
                        ? filter.LessEqual : filterDefinition.EndDecimal;
            else
                filter.LessEqual = filterDefinition.EndLong;
            return filter;
        }
    }
}
