// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Strata.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ResendEmailConfirmationModel : PageModel
    {
        public IActionResult OnGet()
        {
            return RedirectToPage("/Account/Login");
        }

        public IActionResult OnPost()
        {
            return RedirectToPage("/Account/Login");
        }
    }
}
