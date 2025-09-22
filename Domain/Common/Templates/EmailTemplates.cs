using System.Drawing;
using static System.Net.Mime.MediaTypeNames;

namespace Domain.Common.Templates
{
    public static class EmailTemplates
    {
        public static string GetVerificationSubject() =>
            "Verify your NaijaRescue account";

        public static string GetVerificationBody(string code) => $$"""
<!DOCTYPE html>
<html>
<head>
    <meta charset="UTF-8">
    <style>
        body { {font-family: Arial, sans-serif; background-color: #f4f4f4; padding: 20px;} }
        .container { {background-color: #ffffff; max-width: 500px; margin: auto; border-radius: 8px; padding: 20px; box-shadow: 0 2px 5px rgba(0,0,0,0.1);} }
        .header { {font-size: 20px; font-weight: bold; margin-bottom: 15px; color: #d62828;} }
        .code { {font-size: 24px; font-weight: bold; color: #2b2d42; background: #edf2f4; padding: 10px; border-radius: 6px; text-align: center; margin: 20px 0;} }
        .footer { {font-size: 12px; color: #6c757d; margin-top: 20px; text-align: center;} }
    </style>
</head>
<body>
    <div class="container">
        <div class="header">Welcome to NaijaRescue 🚨</div>
        <p>Thank you for joining NaijaRescue! Please verify your email address using the code below:</p>
        <div class="code">{{code}}</div>
        <p>If you didn’t request this, you can safely ignore this email.</p>
        <div class="footer">&copy; {{DateTime.UtcNow.Year}} NaijaRescue. All rights reserved.</div>
    </div>
</body>
</html>
""";

        public static string GetAccountCreatedSubject()
    => "Your NaijaRescue account has been created";

        public static string GetAccountCreatedBody(string role) => $$"""
<!DOCTYPE html>
<html>
<head>
  <meta charset="UTF-8">
  <style>
    body { {font - family: Arial, sans-serif;
      background-color: #f9f9f9;
      padding: 20px;
    } }
    .container { {background - color: #ffffff;
      max-width: 500px;
      margin: auto;
      border-radius: 8px;
      padding: 20px;
      box-shadow: 0 2px 5px rgba(0,0,0,0.1);
    } }
    .header { {font - size: 20px;
      font-weight: bold;
      margin-bottom: 15px;
      color: #0077b6;
    } }
    .role { {font - size: 18px;
      font-weight: bold;
      color: #023e8a;
      margin: 10px 0;
    } }
    .footer { {font - size: 12px;
      color: #6c757d;
      margin-top: 20px;
      text-align: center;
    } }
  </style>
</head>
<body>
  <div class="container">
    <div class="header">Your NaijaRescue Account is Ready 🎉</div>
    <p>Hello,</p>
    <p>A NaijaRescue account has been created for you with the role:</p>
    
    <div class="role">{{role}}</div>

    <p>Please log in with your registered email. If you need help, contact your administrator.</p>
    
    <div class="footer">
      Stay safe,<br/>NaijaRescue Team
    </div>
  </div>
</body>
</html>
""";
    }
}
