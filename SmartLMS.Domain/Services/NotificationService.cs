﻿using System.Linq;
using System.Net;
using System.Net.Mail;
using Carubbi.Extensions;
using Carubbi.Mailer.Interfaces;
using SmartLMS.Domain.Entities;
using SmartLMS.Domain.Entities.Content;
using SmartLMS.Domain.Entities.UserAccess;
using SmartLMS.Domain.Repositories;
using SmartLMS.Domain.Resources;

namespace SmartLMS.Domain.Services
{
    public class NotificationService
    {
        private readonly IContext _context;
        private readonly IMailSender _sender;

        public NotificationService(IContext context, IMailSender sender)
        {
            _sender = sender;
            _context = context;
            ConfigureSender();
        }


        public void SendRecoverPasswordNotification(string email, string recoveredPassword)
        {
            var userRepository = new UserRepository(_context);
            var user = userRepository.GetByEmail(email);

            var parameterRepository = new ParameterRepository(_context);

            var body = Resource.PasswordRecoveryEmailBody
                .Replace("{username}", user.Name)
                .Replace("{login}", email)
                .Replace("{password}", recoveredPassword)
                .Replace("{link}", Parameter.BASE_URL);


            var message = new MailMessage();
            message.To.Add(email);
            message.From = new MailAddress(parameterRepository.GetValueByKey(Parameter.EMAIL_FROM_KEY),
                Parameter.APP_NAME);
            message.Subject = Resource.PasswordRecoveryEmailSubject;
            message.IsBodyHtml = true;
            message.Body = body;

            _sender.Send(message);
        }


        private void ConfigureSender()
        {
            _sender.PortNumber =
                _context.GetList<Parameter>().Single(x => x.Key == Parameter.SMTP_PORT_KEY).Value.To(0);
            _sender.Host = _context.GetList<Parameter>().Single(x => x.Key == Parameter.SMTP_SERVER_KEY).Value;
            _sender.UseDefaultCredentials = _context.GetList<Parameter>()
                .Single(x => x.Key == Parameter.SMTP_USE_DEFAULT_CREDENTIALS_KEY).Value.To(false);
            _sender.UseSsl = _context.GetList<Parameter>().Single(x => x.Key == Parameter.SMTP_USE_SSL_KEY).Value
                .To(false);
            if (_sender.UseDefaultCredentials) return;


            _sender.Username = _context.GetList<Parameter>().Single(x => x.Key == Parameter.SMTP_USERNAME_KEY).Value;
            _sender.Password = _context.GetList<Parameter>().Single(x => x.Key == Parameter.SMTP_PASSWORD_KEY).Value;
        }

        public void SendTalkToUsMessage(string name, string email, string message)
        {
            var talkToUsReceiverEmail = _context.GetList<Parameter>()
                .Single(x => x.Key == Parameter.TALK_TO_US_RECEIVER_EMAIL_KEY).Value;
            var talkToUsReceiverName = _context.GetList<Parameter>()
                .Single(x => x.Key == Parameter.TALK_TO_US_RECEIVER_NAME_KEY).Value;
            var senderMail = _context.GetList<Parameter>().Single(x => x.Key == Parameter.EMAIL_FROM_KEY).Value;

            var mailMessage = new MailMessage();
            var receiverMailAddress = new MailAddress(talkToUsReceiverEmail, talkToUsReceiverName);
            mailMessage.To.Add(receiverMailAddress);
            mailMessage.From = new MailAddress(senderMail, Parameter.APP_NAME);
            mailMessage.IsBodyHtml = true;
            mailMessage.Body = Resource.TalkToUsEmailBody
                .Replace("{name}", name)
                .Replace("{email}", email)
                .Replace("{message}", message);

            mailMessage.Subject = Resource.TalkToUsEmailSubject;
            _sender.Send(mailMessage);
        }

        public void SendDeliveryClassEmail(Class klass, Student student)
        {
            var senderEmail = _context.GetList<Parameter>().Single(x => x.Key == Parameter.EMAIL_FROM_KEY).Value;

            var email = new MailMessage();
            var receievrMailAddress = new MailAddress(student.Email, student.Name);
            email.To.Add(receievrMailAddress);
            email.From = new MailAddress(senderEmail, Parameter.APP_NAME);
            email.IsBodyHtml = true;
            email.Body = Resource.DeliveredClassNotificationEmailBody
                .Replace("{username}", student.Name)
                .Replace("{classname}", klass.Name)
                .Replace("{classId}", klass.Id.ToString())
                .Replace("{coursename}", klass.Course.Name)
                .Replace("{courseId}", klass.Course.Id.ToString())
                .Replace("{link}", Parameter.BASE_URL);

            email.Subject = Resource.DeliveredClassNotificationEmailSubject;
            _sender.Send(email);
        }

        internal void SendCreatingUserNotiication(User user, string password)
        {
            var parameterRepository = new ParameterRepository(_context);

            var body = Resource.CreatingUserNotificationEmailBody
                .Replace("{username}", user.Name)
                .Replace("{login}", user.Login)
                .Replace("{password}", password)
                .Replace("{link}", Parameter.BASE_URL);


            var message = new MailMessage();
            message.To.Add(user.Email);
            message.From = new MailAddress(parameterRepository.GetValueByKey(Parameter.EMAIL_FROM_KEY),
                Parameter.APP_NAME);
            
            message.Subject = Resource.CreatingUserNotificationEmailSubject;
            message.IsBodyHtml = true;
            message.Body = body;

            //var client = new SmtpClient(parameterRepository.GetValueByKey(Parameter.SMTP_SERVER_KEY), System.Convert.ToInt32(parameterRepository.GetValueByKey(Parameter.SMTP_PORT_KEY)))
            //{
            //    Credentials = new NetworkCredential(parameterRepository.GetValueByKey(Parameter.EMAIL_FROM_KEY), parameterRepository.GetValueByKey(Parameter.SMTP_PASSWORD_KEY)),
            //    EnableSsl = true
            //};
            //client.Send(parameterRepository.GetValueByKey(Parameter.EMAIL_FROM_KEY), user.Email, Resource.CreatingUserNotificationEmailSubject, body);

            _sender.Send(message);
        }
    }
}