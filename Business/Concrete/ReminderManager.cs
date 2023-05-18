using Business.Abstract;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using Entities.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;
using System.Security.Cryptography.X509Certificates;
using Telegram.Bot.Polling;
using Microsoft.Extensions.Primitives;
using Telegram.Bot.Exceptions;

namespace Business.Concrete
{
    public class ReminderManager : IReminderService
    {

        IReminderDal _reminderDal;
        private readonly MailSettings _mailSettings;
        TelegramBotClient botClient;


        public ReminderManager(IReminderDal reminderDal, IOptions<MailSettings> mailSettingsOptions) 
        {
            _mailSettings = mailSettingsOptions.Value;
            _reminderDal = reminderDal;
            botClient = new TelegramBotClient("bot token");
        }
        
        public IResult Add(Reminder reminder)
        {

                _reminderDal.Add(reminder);
                return new SuccessResult("Reminder added succesfully");
            
        }

        public IResult Delete(Reminder reminder)
        {
            var result = _reminderDal.Get(r=>r.Id == reminder.Id);
            if(result == null)
            {
                return new ErrorResult($"Cannot find any reminder with this Id : {reminder.Id} ");
            }
            else
            {
                _reminderDal.Delete(reminder);
                return new SuccessResult($"Reminder deleted with Id : {reminder.Id}");
            }
            
        }



        public IDataResult<List<Reminder>>  GetList()
        {
            return new SuccessDataResult<List<Reminder>>(_reminderDal.GetAll());
        }

        public  IDataResult<Reminder> GetReminder(int id)
        {
            
            var  reminder = _reminderDal.Get(r=>r.Id == id);

            return reminder == null
                ? new ErrorDataResult<Reminder>(new Reminder(), $"Cannot find any reminder with this Id : {id} ")
                : new SuccessDataResult<Reminder>(reminder);
            

        }

        public  IResult SendViaEmail(object state)
        {
            var reminder = (Reminder)state;

            bool isValidEmail;
            try
            {
                MailAddress mailAddress = new MailAddress(reminder.SendTo);
                isValidEmail = true;
            }
            catch (Exception)
            {

                isValidEmail = false;
            }

            if (isValidEmail)
            {
                try
                {

                    MailMessage mail = new MailMessage();
                    mail.From = new MailAddress(_mailSettings.SenderEmail);
                    mail.To.Add(reminder.SendTo);
                    mail.Subject = "Schedule Reminder";
                    mail.Body = reminder.Content;


                    SmtpClient smtpClient = new SmtpClient(_mailSettings.Server, _mailSettings.Port);
                    smtpClient.EnableSsl = true;
                    smtpClient.Credentials = new NetworkCredential(_mailSettings.SenderEmail, _mailSettings.Password);

                    smtpClient.Send(mail);

                    return new SuccessResult();

                }
                catch (Exception ex)
                {
                    return new ErrorResult(ex.Message.ToString());
                }
            }
            else
            {
                return new ErrorResult("Email is not valid");
            }


        }



        public IResult Update(Reminder reminder)
        {
            var result = _reminderDal.Get(r=>r.Id == reminder.Id);
            
            if(result == null)
            {
                return new ErrorResult($"We cannot find any reminder with id : {reminder.Id}");
            }
            else
            {
                _reminderDal.Update(reminder);
                return new SuccessResult($"Reminder updated successfully with id : {reminder.Id}");
            }

        }

        public async void SendViaTelegram(object state)
        {
            var reminder = (Reminder)state;

            bool isValid = false;
            try
            {
                Chat chat = await botClient.GetChatAsync(reminder.SendTo);
                isValid = true;
            }
            catch (Exception)
            {
                isValid = false;
                
            }
            
            if(isValid)
            {
                await botClient.SendTextMessageAsync(reminder.SendTo, reminder.Content);
            }

            
        }
    }
}
