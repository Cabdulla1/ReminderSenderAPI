using Business.Abstract;

namespace ReminderSenderAPI.Services
{
    public class ReminderSenderBackgroundService:BackgroundService
    {
        private readonly IReminderService _reminderService;

        private readonly TimeSpan checkInterval = TimeSpan.FromSeconds(20);

        public ReminderSenderBackgroundService(IReminderService emailReminderService)
        {
            _reminderService = emailReminderService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
         {

            while (!stoppingToken.IsCancellationRequested)
            {
                var remindersViaEmail = _reminderService.GetList().Data.
                    Where(r => r.SendAt >= DateTime.Now && 
                    (r.Method.ToLower() == "email" || r.Method =="telegram")
                    && (r.SendAt - DateTime.Now).TotalMinutes < 2);

                foreach (var reminder in remindersViaEmail)
                {
                    TimeSpan delay = reminder.SendAt - DateTime.Now;
                    await Task.Delay(delay);
                    if(reminder.Method.ToLower() == "email")
                    {
                        _reminderService.SendViaEmail(reminder);
                    }
                    else
                    {
                        _reminderService.SendViaTelegram(reminder);
                    }
                    

                }
                await Task.Delay(checkInterval, stoppingToken);
            }


        }
    }
}
