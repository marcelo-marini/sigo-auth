// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.ComponentModel.DataAnnotations;

namespace Sigo.Auth.Api.Quickstart.Account
{
    public class LoginInputModel
    {
        [Required(ErrorMessage = "Usuário é obrigatório")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Senha é obrigatória")]
        public string Password { get; set; }
        public bool RememberLogin { get; set; }
        public string ReturnUrl { get; set; }
    }
}