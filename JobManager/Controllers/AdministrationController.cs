using EasyQ.Models;
using JobManager.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Subscription.Models;
using System.Security.Claims;
using System.Text.RegularExpressions;
using ViewModels;

namespace EasyQ.Controllers
{
    public class AdministrationController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        public AdministrationController(SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.signInManager = signInManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [HttpPost]   // Or use [AcceptVerbs("Get","Post")] it will accept get and post both
        public async Task<IActionResult> IsEmailInUse(string email, string Id)
        {
            var user = await userManager.FindByEmailAsync(email);
            var user1 = await userManager.FindByIdAsync(Id);
            if (user1.Email == email)
            {
                return Json(true);
            }
            if (user == null)
            {
                return Json(true);
            }
            else
            {
                return Json($"Email {email} is already taken.");
            }
        }



        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
               // ViewBag.ErrorMessage = $"User with Id: {id} cannot be found";
                return View("Error", new ErrorViewModel
                {
                    ErrorCode = 10000,
                    ErrorMessage = $"User with Id: {id} cannot be found"
                });

               // return View("Error");
            }
            // GetClaimsAsync retunrs the list of user Claims
            var userClaims = await userManager.GetClaimsAsync(user);
            // GetRolesAsync returns the list of user Roles
            var userRoles = await userManager.GetRolesAsync(user);
            var model = new EditUserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                DisplayName = user.DisplayName,
                UserName = user.UserName,
                Claims = userClaims.Select(c => c.Type + " : " + c.Value).ToList(),
                Roles = userRoles,
                IsEmailConfirmed = user.EmailConfirmed
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            var user = await userManager.FindByIdAsync(model.Id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {model.Id} cannot be found";
                return View("NotFound");
            }
            else
            {
                user.Email = model.Email;
                user.UserName = model.UserName;
                user.DisplayName = model.DisplayName;
                user.EmailConfirmed = model.IsEmailConfirmed;
                var result = await userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(model.NewPassword))
                    {
                        var token = await userManager.GeneratePasswordResetTokenAsync(user);
                        await userManager.ResetPasswordAsync(user, token, model.NewPassword);
                    }
                    return View(model);
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(model);
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {id} cannot be found";
                return View("NotFound");
            }
            else
            {
                try
                {
                    var IsUserAdmin = await userManager.IsInRoleAsync(user, "Admin");
                    if (!IsUserAdmin)
                    {
                        var externallogin = await userManager.GetLoginsAsync(user);
                        if (externallogin.Count != 0)
                        {
                            foreach (var login in externallogin)
                            {
                                await userManager.RemoveLoginAsync(user, login.LoginProvider, login.ProviderKey);
                            }
                        }
                        var userClaims = await userManager.GetClaimsAsync(user);
                        if (userClaims.Count != 0)
                        {
                            foreach (var claim in userClaims)
                            {
                                await userManager.RemoveClaimAsync(user, claim);
                            }
                        }
                        var userRoles = await userManager.GetRolesAsync(user);
                        if (userRoles.Count != 0)
                        {
                            foreach (var role in userRoles)
                            {
                                await userManager.RemoveFromRoleAsync(user, role);
                            }
                        }

                        var result = await userManager.DeleteAsync(user);

                        if (result.Succeeded)
                        {
                            return RedirectToAction("ListUsers");
                        }

                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                        return View("ListUsers");
                    }
                    else
                    {
                        ViewBag.ErrorTitle = "Error: Admin user cannot be deleted";
                        ViewBag.ErrorMessage = "Error:" + user.Email + " is an Admin user, hence cannot be deleted";
                        return View("Error");
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorTitle = "Error occured in deleting the user";
                    ViewBag.ErrorMessage = "Error:" + ex.Message + "\n" + ex.InnerException;
                    return View("Error");
                }
            }
        }


        [HttpGet]
        public IActionResult ListUsers()
        {
            IQueryable<ApplicationUser> users = userManager.Users;
            return View(users);
        }
        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityRole identityRole = new IdentityRole { Name = model.RoleName };
                var result = await roleManager.CreateAsync(identityRole);
                if (result.Succeeded)
                {
                    TempData["IsRoleCreated"] = "true";
                    return RedirectToAction("ListRoles");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }

            return View(model);
        }
        [HttpGet]
        public IActionResult ListRoles()
        {
            var roles = roleManager.Roles;
            return View(roles);
        }
        [HttpGet]
       // [Authorize(Policy = "EditRolePolicy")]
        public async Task<IActionResult> EditRole(string Id)
        {
            var role = await roleManager.FindByIdAsync(Id);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role within Id = {Id} cannot be found";
                return View("NotFound");
            }
            var model = new EditRoleViewModel
            {
                Id = role.Id,
                RoleName = role.Name
            };
            var users = await userManager.Users.ToListAsync();
            foreach (var user in users)
            {
                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    //model.Users.Add(user.DisplayName + " - " + user.UserName);
                    model.Users.Add(user.UserName + " - " + user.UserName);
                }
            }
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
      //  [Authorize(Policy = "EditRolePolicy")]
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            var role = await roleManager.FindByIdAsync(model.Id);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role within Id = {model.Id} cannot be found";
                return View("NotFound");
            }
            else
            {
                role.Name = model.RoleName;
                var result = await roleManager.UpdateAsync(role);
                if (result.Succeeded) return RedirectToAction("ListRoles");
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(model);
            }
        }



        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await roleManager.FindByIdAsync(id);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {id} cannot be found";
                return View("NotFound");
            }
            else
            {
                try
                {
                    var result = await roleManager.DeleteAsync(role);

                    if (result.Succeeded)
                    {
                        return RedirectToAction("ListRoles");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }

                    return View("ListRoles");
                }
                catch//(DbUpdateException ex)
                {
                    ViewBag.ErrorTitle = $"{role.Name} role is in use";
                    ViewBag.ErrorMessage = $"{role.Name} role cannot be deleted as there are users in this role. If you want to delete this role, please remove the users from the role and then try to delete";
                    return View("Error");
                }

            }
        }
        [HttpGet]
        public async Task<IActionResult> EditUsersInRole(string roleId)
        {
            ViewBag.roleId = roleId;
            var role = await roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role within Id = {roleId} cannot be found";
                return View("NotFound");
            }
            ViewBag.roleName = role.Name;
            var model = new List<UserRoleViewModel>();
            foreach (var user in await userManager.Users.ToListAsync())
            {
                var userRoleViewModel = new UserRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                   // FullName = user.DisplayName,
                    IsSelected = await userManager.IsInRoleAsync(user, role.Name)
                };
                model.Add(userRoleViewModel);
            }
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUsersInRole(List<UserRoleViewModel> model, string roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role within Id = {roleId} cannot be found";
                return View("NotFound");
            }
            for (int i = 0; i < model.Count; i++)
            {
                var user = await userManager.FindByIdAsync(model[i].UserId);
                IdentityResult result;
                if (model[i].IsSelected && !(await userManager.IsInRoleAsync(user, role.Name)))
                {
                    result = await userManager.AddToRoleAsync(user, role.Name);
                }
                else if (!model[i].IsSelected && (await userManager.IsInRoleAsync(user, role.Name)))
                {
                    result = await userManager.RemoveFromRoleAsync(user, role.Name);
                }
                else
                {
                    continue;
                }
                if (result.Succeeded)
                {
                    if (i < (model.Count - 1)) continue;
                    else return RedirectToAction("EditRole", new { Id = roleId });
                }
            }
            return RedirectToAction("EditRole", new { Id = roleId });
        }
        [HttpGet]
       // [Authorize(Policy = "EditRolePolicy")]
        public async Task<IActionResult> ManageUserRoles(string userId)
        {
            ViewBag.userId = userId;

            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found";
                return View("NotFound");
            }

            var model = new List<UserRolesViewModel>();

            foreach (var role in await roleManager.Roles.ToListAsync())
            {
                var userRolesViewModel = new UserRolesViewModel
                {
                    RoleId = role.Id,
                    RoleName = role.Name
                };

                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    userRolesViewModel.IsSelected = true;
                }
                else
                {
                    userRolesViewModel.IsSelected = false;
                }

                model.Add(userRolesViewModel);
            }

            return View(model);
        }
        [HttpPost]
       // [Authorize(Policy = "EditRolePolicy")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageUserRoles(List<UserRolesViewModel> model, string userId)
        {
            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found";
                return View("NotFound");
            }

            var roles = await userManager.GetRolesAsync(user);
            var result = await userManager.RemoveFromRolesAsync(user, roles);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot remove user existing roles");
                return View(model);
            }

            result = await userManager.AddToRolesAsync(user,
                model.Where(x => x.IsSelected).Select(y => y.RoleName));

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot add selected roles to user");
                return View(model);
            }

            return RedirectToAction("EditUser", new { Id = userId });
        }


        //[Authorize(Policy = "AdminRolePolicy")]
        [HttpGet]
        public async Task<IActionResult> ManageUserClaims(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found";
                return View("NotFound");
            }

            // UserManager service GetClaimsAsync method gets all the current claims of the user
            var existingUserClaims = await userManager.GetClaimsAsync(user);

            var model = new UserClaimsViewModel
            {
                UserId = userId
            };

            // Loop through each claim we have in our application
            foreach (Claim claim in ClaimsStore.AllClaims)
            {
                UserClaim userClaim = new UserClaim
                {
                    ClaimType = claim.Type
                };

                // If the user has the claim, set IsSelected property to true, so the checkbox
                // next to the claim is checked on the UI
                if (existingUserClaims.Any(c => c.Type == claim.Type && c.Value == "true"))
                {
                    userClaim.IsSelected = true;
                }

                model.Cliams.Add(userClaim);
            }

            return View(model);

        }

        //[Authorize(Policy = "AdminRolePolicy")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageUserClaims(UserClaimsViewModel model)
        {
            var user = await userManager.FindByIdAsync(model.UserId);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {model.UserId} cannot be found";
                return View("NotFound");
            }

            // Get all the user existing claims and delete them
            var claims = await userManager.GetClaimsAsync(user);
            var result = await userManager.RemoveClaimsAsync(user, claims);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot remove user existing claims");
                return View(model);
            }

            // Add all the claims that are selected on the UI
            result = await userManager.AddClaimsAsync(user,
                model.Cliams.Select(c => new Claim(c.ClaimType, c.IsSelected ? "true" : "false")));

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot add selected claims to user");
                return View(model);
            }

            return RedirectToAction("EditUser", new { Id = model.UserId });

        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult SeedAdminUser()
        {
            ViewBag.Error = "";
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult SeedAdminUser(SeedAdminUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Email == "syedirfanali@outlook.com" && model.Password == "Abcd@1234")
                {
                    TempData["Authenticated"] = "true";
                    return RedirectToAction("Setup", "Administration");
                }
                ModelState.AddModelError("", "Invalid Login Attempt");
            }
            return View();
        }


        [HttpGet]
        public IActionResult SetUpDepartments()
        {
            if (TempData.Peek("Authenticated") == null)
            {
                return RedirectToAction("Index", "Home");
            }
            string IsAuthenticated = String.Empty;
            if (TempData.ContainsKey("Authenticated"))
            {
                IsAuthenticated = TempData.Peek("Authenticated")!.ToString();
            }
            if (IsAuthenticated != "true")
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SetUpDepartments(SetUpDepartmentsViewModel model)
        {
            if (ModelState.IsValid)
            {

                return RedirectToAction("Setup", "Administration");
            }
            return View(model);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Setup()
        {
            if (TempData.Peek("Authenticated") == null)
            {
                return RedirectToAction("Index", "Home");
            }

            string IsAuthenticated = String.Empty;
            if (TempData.ContainsKey("Authenticated"))
            {
                IsAuthenticated = TempData.Peek("Authenticated")!.ToString();
            }
            if (IsAuthenticated != "true")
            {
                return RedirectToAction("Index", "Home");
            }
            SetupViewModel model = new SetupViewModel();
          
            return View(model);
        }
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Setup(SetupViewModel model)
        {
            try
            {

                if (!(await roleManager.RoleExistsAsync("Admin")))
                {
                    IdentityRole identityRole = new IdentityRole { Name = "Admin" };
                    var result = await roleManager.CreateAsync(identityRole);
                    if (!result.Succeeded)
                    {
                        var errors = string.Empty;
                        foreach (var error in result.Errors)
                        {
                            //ModelState.AddModelError("", error.Description);
                            errors += "Error Code: " + error.Code + " | Description:" + error.Description + "\n";
                        }
                        return View("Error", new ErrorViewModel
                        {
                            ErrorCode = 1000,
                            ErrorMessage = errors
                        });
                    }
                }
                if (!(await roleManager.RoleExistsAsync("SuperAdmin")))
                {
                    var identityRole = new IdentityRole { Name = "SuperAdmin" };
                    var result = await roleManager.CreateAsync(identityRole);
                    if (!result.Succeeded)
                    {
                        var errors = string.Empty;
                        foreach (var error in result.Errors)
                        {
                            //ModelState.AddModelError("", error.Description);
                            errors += "Error Code: " + error.Code + " | Description:" + error.Description + "\n";
                        }
                        return View("Error", new ErrorViewModel
                        {
                            ErrorCode = 1001,
                            ErrorMessage = errors
                        });
                    }
                }

                var user = await userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    user = new ApplicationUser
                    {
                        UserName = model.Email,
                        Email = model.Email,
                       // DisplayName = model.DisplayName,
                        EmailConfirmed = true,
                        LockoutEnabled = false,
                        TwoFactorEnabled = false,
                        FirstName = "",
                        //Surname = "",
                        //FatherName = "",
                        //MotherName = "",
                        //MiddleName = "",
                        //Gender=(int)LookUp.LookUpClass.Genders.Male,
                        //LocalAddress = "",
                        //PermanentAddress = "",
                        // CasteCategory=(int)LookUp.LookUpClass.CasteCategories.OPEN,
                        //DateOfBirth= DateTime.Now.AddYears(-40) ,
                        //Religion= (int)LookUp.LookUpClass.Religions.MUSLIM,
                        //PhotoId = 0,
                       // SignatureId = 0,
                        //AadhaarNo = "",
                        //Pincode = "440001"
                    };
                    var result = await userManager.CreateAsync(user, model.Password);
                    if (!result.Succeeded)
                    {
                        var errors = string.Empty;
                        foreach (var error in result.Errors)
                        {
                            //ModelState.AddModelError("", error.Description);
                            errors += "Error Code: " + error.Code + " | Description:" + error.Description + "\n";
                        }
                        return View("Error", new ErrorViewModel
                        {
                            ErrorCode = 1001,
                            ErrorMessage = errors
                        });
                        //foreach (var error in result.Errors)
                        //{
                        //    ModelState.AddModelError("", error.Description);
                        //}
                    }

                  


                    List<string> userRoles = new List<string>() { "Admin", "SuperAdmin" };

                    result = await userManager.AddToRolesAsync(user, userRoles);
                    if (!result.Succeeded)
                    {
                        var errors = string.Empty;
                        foreach (var error in result.Errors)
                        {
                            //ModelState.AddModelError("", error.Description);
                            errors += "Error Code: " + error.Code + " | Description:" + error.Description + "\n";
                        }
                        throw new Exception(errors);
                        //ModelState.AddModelError("", "Cannot add selected roles to user");
                        //return View(model);
                    }
                }


                //Add organization name and add its id to this user
                var resultSignIn = await signInManager.PasswordSignInAsync(model.Email, model.Password, isPersistent: true, false);
                if (!resultSignIn.Succeeded)
                {
                    throw new Exception("Failed to sign in");
                }
                if (resultSignIn.IsLockedOut)
                {
                    throw new Exception("User is locked out");
                }
                if (resultSignIn.IsNotAllowed)
                {
                    throw new Exception("User is not allowed to login");
                }

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                return View("Error", new ErrorViewModel
                {
                    ErrorCode = 0001,
                    ErrorMessage = ex.Message + "/" + ex.InnerException
                });
            }
        }
        private static readonly Regex sWhitespace = new Regex(@"\s+");
        public static string GenerateUserName(string Name, string MobileNo)
        {
            var compactName = sWhitespace.Replace(Name, "");
            var fourCharName = compactName.Substring(0, 4);
            var fourDigitMobile = MobileNo.Substring(0, 4);
            var userName = fourCharName.ToLower() + fourDigitMobile;
            return userName;
        }
    }
}
