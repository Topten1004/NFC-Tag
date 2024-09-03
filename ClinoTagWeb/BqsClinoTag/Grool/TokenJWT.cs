﻿using BqsClinoTag.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using BqsClinoTag.ViewModel;

namespace BqsClinoTag.Grool
{
    public static class TokenJWT
    {
        public enum TOKEN_EXPIRATION : int
        {
            IMMEDIAT = 15,
            JOUR = 3600 * 24,
            SEMAINE = 3600 * 24 * 7,
            MOIS = 3600 * 24 * 30,
            ANNEE = 3600 * 24 * 365
        }

        public static JwtSecurityToken creerTokenJWT(string userId, string login, string role, CLINOTAGBQSContext db, TOKEN_EXPIRATION tokenExp = TOKEN_EXPIRATION.MOIS)
        {
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, login),
                new Claim(ClaimTypes.Role, role)
            };

            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT:Secret"]));

            DateTime expiration = DateTime.Now.AddSeconds(Convert.ToDouble(tokenExp));

            var token = new JwtSecurityTokenHandler().CreateJwtSecurityToken(
                issuer: config["JWT:ValidIssuer"],
                audience: config["JWT:ValidAudience"],
                subject: new ClaimsIdentity(authClaims),
                notBefore: DateTime.Now,
                expires: expiration,
                issuedAt: DateTime.Now,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256),
                encryptingCredentials: new EncryptingCredentials(authSigningKey, JwtConstants.DirectKeyUseAlg, SecurityAlgorithms.Aes256CbcHmacSha512));

            return token;
        }
    }
}