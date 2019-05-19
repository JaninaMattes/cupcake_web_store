﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Web;
using WebsiteLaitBrasseur.DAL;
using System.Security.Cryptography;
using System.Diagnostics;

namespace WebsiteLaitBrasseur.BL
{
    public class AccountBL
    {
        private AccountDAL db = new AccountDAL();
        private static int count = 0;

        /// <summary>
        /// create new login and simultaneously C:\Users\Janina\Documents\DEV\LaitBrasseur-WebStore\WebsiteC:\Users\Janina\Documents\DEV\LaitBrasseur-WebStore\WebsiteLaitBrasseur\BL\BO\Account.csLaitBrasseur\BL\BO\Account.cs
        /// the correct account that belongs to a login
        /// 
        /// Check correctness:
        /// a) Email is not correct in terms of not valid -> return 1
        /// b) Password string is not correct in terms of business rules and security -> return 2
        /// c) Everything is correct return 0 and store isConfirmed value with 1 in database
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        public int CreateAccount(string email, string password, string firstName, string lastName, 
            string birthDate, string phoneNo, string imgPath, int status, int isAdmin)
        {
            int isCorrect = 0;
            try
            {
                int isConfirmed = 0;
                //check if format of email is correct else return = 1
                if (IsValidEmail(email)) {
                //check if the email address is an existing mail
                if (IsEmailValid(email))
                    {
                        isConfirmed = 1; //is correct mail
                    }
                    else
                    {
                        return isCorrect = 1; //is not correct
                    }
                }
                //check if format of password is correct else return = 2
                if (IsValidPassword(password)) {
                    isCorrect = 0;
                } else {
                    return isCorrect = 2; //is not correct
                }
                //if both are correct return 0
                if (isCorrect == 0)
                {
                    //encrypt the password before it is sent to the DB
                    password = this.HashPassword(password);
                    //returns the created Account ID as integer value
                    db.Insert(email, password, isConfirmed, firstName, lastName, birthDate, phoneNo, imgPath, status, isAdmin);
                }                
            }
            catch (Exception e)
            {
                e.GetBaseException();
            }
            return isCorrect;
        }

        /// <summary>
        /// Function to check if password and email address are
        /// correctly entered in a textbox.
        /// Business Rules:
        /// a) Password and Username are not correct return = 0
        /// b) Password and Username are correct and exist return = 1
        /// c) Password is incorrect return = 2
        /// d) Username is incorrect return = 3        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        private int IsCorrect(string email, string password)
        {           
            int isCorrect = 0;
            string hashPW = HashPassword(password);
            try
            {
                if (db.FindLoginEmail(email) != 1)
                {
                    return isCorrect = 3;
                }
                if (db.FindLoginPW(hashPW) != 1)
                {
                    return isCorrect = 2;
                }
                else
                {
                    //check if login is correct = user already exists in database
                    isCorrect = db.FindLoginCred(email, hashPW);
                    Debug.Print("AccountBL / value returned " + isCorrect.ToString());
                }
                count++;
                //TODO if count is > 3 within certain amount of time, set counter back to 0 after 10min
                //then block the user account for 1 hour
            }
            catch (Exception e)
            {
                e.GetBaseException();
            }
            return isCorrect;
        }


        /// <summary>
        /// Check if the added email address is valid
        /// </summary>
        /// <param name="emailaddress"></param>
        /// <returns></returns>
        private bool IsEmailValid(string emailaddress)
        {
            try
            {
                MailAddress m = new MailAddress(emailaddress);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        /// <summary>
        /// source https://docs.microsoft.com/en-us/dotnet/standard/base-types/how-to-verify-that-strings-are-in-valid-email-format
        /// Check if email format is correct and return a boolean value for true/false
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        private static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Normalize the domain
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                                      RegexOptions.None, TimeSpan.FromMilliseconds(200));

                // Examines the domain part of the email and normalizes it.
                string DomainMapper(Match match)
                {
                    // Use IdnMapping class to convert Unicode domain names.
                    var idn = new IdnMapping();

                    // Pull out and process domain name (throws ArgumentException on invalid)
                    var domainName = idn.GetAscii(match.Groups[2].Value);

                    return match.Groups[1].Value + domainName;
                }
            }
            catch (RegexMatchTimeoutException e)
            {
                e.GetBaseException();
                return false;
            }
            catch (ArgumentException e)
            {
                e.GetBaseException();
                return false;
            }

            try
            {
                return Regex.IsMatch(email,
                    @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                    @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        /// <summary>
        /// function to check if a password is correct entered in the textfield
        /// this returns a true or false value if the business rules are kept correctly
        /// Rules for the Password are:
        /// * avoid spaces in the string
        /// * Length: Maximum 10 characters
        /// * UpperCase: at least one uppercase letter
        /// * LowerCase: at least one lowercase letter
        /// * Number: at least one number
        /// </summary>
        /// <param name="password"></param>
        /// <returns>boolean value</returns>
        private bool IsValidPassword(string password)
        {
            string MatchPasswordRules = "/^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{10,}$/";
            if (password != null) return Regex.IsMatch(password, MatchPasswordRules);
            else return false;
        }

        /// <summary>
        /// Additional enum to store values from 
        /// PasswordAdvisor class
        /// </summary>

        private enum PasswordScore
        {
            Blank = 0,
            VeryWeak = 1,
            Weak = 2,
            Medium = 3,
            Strong = 4,
            VeryStrong = 5
        }

        /// <summary>
        /// Check quality of password
        /// source: https://stackoverflow.com/questions/12899876/checking-strings-for-a-strong-enough-password
        /// </summary>
        private class PasswordAdvisor
        {
            public static PasswordScore CheckStrength(string password)
            {
                int score = 0;

                if (password.Length < 1)
                    return PasswordScore.Blank;
                if (password.Length < 4)
                    return PasswordScore.VeryWeak;
                if (password.Length >= 8)
                    score++;
                if (password.Length >= 12)
                    score++;
                if (Regex.Match(password, @"/\d+/", RegexOptions.ECMAScript).Success)
                    score++;
                if (Regex.Match(password, @"/[a-z]/", RegexOptions.ECMAScript).Success &&
                  Regex.Match(password, @"/[A-Z]/", RegexOptions.ECMAScript).Success)
                    score++;
                if (Regex.Match(password, @"/.[!,@,#,$,%,^,&,*,?,_,~,-,£,(,)]/", RegexOptions.ECMAScript).Success)
                    score++;

                return (PasswordScore)score;
            }
        }

        /// <summary>
        /// Source Stackoverflow: https://stackoverflow.com/questions/4181198/how-to-hash-a-password/10402129#10402129
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        private string HashPassword(string password)
        {
            byte[] salt;
            string savedPasswordHash;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);
            //get hash value
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);
            //combine both
            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);
            //return the created hash password
            return savedPasswordHash = Convert.ToBase64String(hashBytes); 
        }
    }
}