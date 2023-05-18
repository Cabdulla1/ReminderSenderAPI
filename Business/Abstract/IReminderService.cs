using Core.Utilities.Results;
using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Abstract
{
    public interface IReminderService
    {
        IDataResult<List<Reminder>> GetList();
        IDataResult<Reminder>  GetReminder(int id);
        IResult Add(Reminder reminder);
        IResult Update(Reminder reminder);
        IResult Delete(Reminder reminder);
        IResult SendViaEmail(object state);
        void SendViaTelegram(object state);
    }
}
