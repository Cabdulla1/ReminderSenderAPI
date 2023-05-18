using Business.Abstract;
using Entities.Concrete;
using Entities.Configurations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ReminderSenderAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReminderController : ControllerBase
    {

        IReminderService _reminderService;
        public ReminderController(IReminderService reminderService) 
        {
            _reminderService = reminderService;
        }



        

        [HttpGet("Read")]
        public IActionResult Read()
        {

            return Ok(_reminderService.GetList());
        }

        [HttpDelete("delete")]
        public IActionResult Delete(int reminderId)
        {

            var result = _reminderService.Delete(new Reminder { Id = reminderId });

            if (result.Success)
            {
                return Ok(result.Message);
            }
            else
            {
                return BadRequest(result.Message);
            }
            
        }



        [HttpPost("create")] 
        public IActionResult Create(Reminder reminder)
        {
            
            if (reminder.Method.ToLower() == "email" || reminder.Method.ToLower() == "telegram")
            {

                if (reminder.SendAt < DateTime.Now)
                {
                    return BadRequest("You cannot schedule reminder for past time");
                }
                else
                {
                    if (ModelState.IsValid)
                    {
                        var result = _reminderService.Add(reminder);
                        return Ok(result);

                    }
                    return BadRequest(ModelState);
                }
            }

            return BadRequest("You can send reminders with either email or telegram");
            

        }


        [HttpPost("update")]

        public IActionResult Update(Reminder reminder)
        {
            if(reminder.Method.ToLower() == "email" || reminder.Method.ToLower() == "telegram")
            {
                if (reminder.SendAt < DateTime.Now)
                {
                    return BadRequest("You cannot update reminder for past time");
                }
                else
                {
                    var result = _reminderService.Update(reminder);
                    if (result.Success)
                    {
                        return Ok(result.Message);
                    }

                    return BadRequest(result.Message);
                }
            }

            return BadRequest("You can send reminders with either email or telegram");

        }
    }
}
