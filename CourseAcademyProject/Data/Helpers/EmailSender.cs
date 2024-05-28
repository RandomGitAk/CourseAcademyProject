using CourseAcademyProject.Models.Email;
using MailKit.Net.Smtp;
using MimeKit;

namespace CourseAcademyProject.Data.Helpers
{
    public class EmailSender
    {
        private readonly EmailConfiguration _emailConfig;
        public EmailSender(EmailConfiguration emailConfig)
        {
            _emailConfig = emailConfig;
        }

        public bool SendResetPassword(string email, string link, string name) 
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Course Academy", _emailConfig.UserName));
            message.To.Add(new MailboxAddress(name, email));
            message.Subject = "Your Course Academy password reset link";
            message.Body = new TextPart("html")
            {
                Text = $@"<!DOCTYPE html>
                <html>
                <head>
                    <title>Course Academy - Change Password</title>
                    <style>
                        body {{
                            font-family: Arial, sans-serif;
                            background-color: #f5f5f5;
                            margin: 0;
                            padding: 0;
                        }}

                        .container {{
                            max-width: 600px;
                            margin: 0 auto;
                            padding: 20px;
                            background-color: #fff;
                            box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
                            border-radius: 5px;
                            text-align: center;
                        }}

                        .header {{
                            background-color: #3d84a8;
                            color: white;
                            padding: 10px;
                            border-radius: 5px 5px 0 0;
                        }}

                        .content {{
                            padding: 20px;
                            text-align: left;
                        }}

                        .footer {{
                            background-color: #f5f5f5;
                            padding: 10px;
                            border-radius: 0 0 5px 5px;
                        }}

                        .button {{
                            display: inline-block;
                            padding: 10px 20px;
                            background-color: #3d84a8;
                            color: white;
                            text-decoration: none;
                            border-radius: 5px;
                        }}
                    </style>
                </head>
                <body>
                    <div class=""container"">
                        <div class=""header"">
                            <h1>Course Academy</h1>
                        </div>
                        <div class=""content"">
                            <h2>Congratulations!</h2>
                            <p>You received this email because your password has been changed on your Course Academy account.</p>
                              <p>If that was you, click the button below to complete the password reset process:</p>
                                <a class=""button"" href=""{link}"">Change password</a>
                                <p>If you have not made this change, please ignore this Email.</p>
                                <p>Best regards,<br>Course Academy team</p>
                         </div>
                        <div class=""footer"">
                             <p>© 2023 Course Academy</p>
                        </div>
                    </div>
                </body>
                </html>
                "
            };
            return Send(message);
        }

        private bool Send(MimeMessage message)
        {
            using (var client = new SmtpClient())
            {
                client.Connect(_emailConfig.SmtpServer, _emailConfig.Port, false);
                client.Authenticate(_emailConfig.UserName, _emailConfig.Password);
                try
                {
                    client.Send(message);
                    return true;
                }
                catch (Exception)
                {
                    //Logging information
                }
                finally 
                {
                    client.Disconnect(true);
                }
            } 
            return false;
        }
    }
}
