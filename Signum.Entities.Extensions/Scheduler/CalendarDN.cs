﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Signum.Entities;
using Signum.Utilities;
using System.Reflection;
using System.Linq.Expressions;

namespace Signum.Entities.Scheduler
{
    [Serializable]
    public class CalendarDN : Entity
    {
        [NotNullable, SqlDbType(Size = 100), UniqueIndex]
        string name;
        [StringLengthValidator(AllowNulls = false, Min = 3, Max = 100)]
        public string Name
        {
            get { return name; }
            set { SetToStr(ref name, value, () => Name); }
        }
        
        [NotNullable]
        MList<HolidayDN> holidays = new MList<HolidayDN>();
        [NotNullValidator]
        public MList<HolidayDN> Holidays
        {
            get { return holidays; }
            set { Set(ref holidays, value, () => Holidays); }
        }

        public void CleanOldHolidays()
        {
            holidays.RemoveAll(h => h.Date < TimeZoneManager.Now);
        }

        public bool IsHoliday(DateTime date)
        {
            return holidays.Any(h => h.Date == date);
        }

        protected override string PropertyValidation(PropertyInfo pi)
        {
            if (pi.Is(()=>Holidays) && holidays != null)
            {
                string rep = (from h in holidays
                              group 1 by h.Date into g
                              where g.Count() > 2
                              select new { Date = g.Key, Num = g.Count() }).ToString(g => "{0} ({1})".Formato(g.Date, g.Num), ", ");

                if (rep.HasText())
                    return "Some dates have been repeated: " + rep;
            }

            return base.PropertyValidation(pi);
        }

        static readonly Expression<Func<CalendarDN, string>> ToStringExpression = e => e.name;
        public override string ToString()
        {
            return ToStringExpression.Evaluate(this);
        }
    }

    [Serializable]
    public class HolidayDN : EmbeddedEntity
    {
        DateTime date;
        [DaysPrecissionValidator]
        public DateTime Date
        {
            get { return date; }
            set { SetToStr(ref date, value, () => Date); }
        }

        [NotNullable, SqlDbType(Size = 100), UniqueIndex]
        string name;
        [StringLengthValidator(AllowNulls = false, Min = 3, Max = 100)]
        public string Name
        {
            get { return name; }
            set { SetToStr(ref name, value, () => Name); }
        }

        public override string ToString()
        {
            return "{0:d} {1}".Formato(date, name);
        }
    }
}
