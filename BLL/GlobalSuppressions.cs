// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Naming", "CA1704:Identifiers should be spelled correctly", Justification = "<>", Scope = "type", Target = "~T:BLL.Services.JwtService")]
[assembly: SuppressMessage("Naming", "CA1704:Identifiers should be spelled correctly", Justification = "<>", Scope = "member", Target = "~M:BLL.Services.EmailService.SendEmail(System.String,System.String,System.String)")]
[assembly: SuppressMessage("Naming", "CA1704:Identifiers should be spelled correctly", Justification = "<>", Scope = "member", Target = "~M:BLL.Services.UserService.Update(Core.Models.UserUpdateModel)")]
[assembly: SuppressMessage("Naming", "CA1704:Identifiers should be spelled correctly", Justification = "<>", Scope = "member", Target = "~M:BLL.Services.UserService.CreateNonActiveUser(Core.Models.UserCreateModel)")]
[assembly: SuppressMessage("Naming", "CA1704:Identifiers should be spelled correctly", Justification = "<>", Scope = "member", Target = "~F:BLL.Services.LoginService.jwtService")]
[assembly: SuppressMessage("Naming", "CA1704:Identifiers should be spelled correctly", Justification = "<>", Scope = "member", Target = "~M:BLL.Services.LoginService.#ctor(BLL.Abstractions.Interfaces.IUserService,BLL.Abstractions.Interfaces.IJwtService)")]
[assembly: SuppressMessage("Naming", "CA1704:Identifiers should be spelled correctly", Justification = "<>", Scope = "member", Target = "~M:BLL.Services.SHA256HashingService.Hash(System.String)~System.String")]
[assembly: SuppressMessage("Naming", "CA1704:Identifiers should be spelled correctly", Justification = "<>", Scope = "member", Target = "~M:BLL.Services.UserService.MapUserCreateModel(Core.Models.UserCreateModel)~Core.Models.UserModel")]
[assembly: SuppressMessage("Naming", "CA1704:Identifiers should be spelled correctly", Justification = "<>", Scope = "member", Target = "~M:BLL.Services.UserService.MapUserUpdateModel(Core.Models.UserUpdateModel)~Core.Models.UserModel")]
[assembly: SuppressMessage("Naming", "CA1704:Identifiers should be spelled correctly", Justification = "<>", Scope = "member", Target = "~M:BLL.Services.RegistrationService.MapUserRegistrationModel(Core.Models.UserRegistrationModel)~Core.Models.UserCreateModel")]
[assembly: SuppressMessage("Naming", "CA1704:Identifiers should be spelled correctly", Justification = "<>", Scope = "member", Target = "~M:BLL.Services.TokenGeneratorService.GetUidb64(Core.Models.UserModel)~System.String")]
[assembly: SuppressMessage("Naming", "CA1704:Identifiers should be spelled correctly", Justification = "<>", Scope = "member", Target = "~M:BLL.Services.TokenGeneratorService.GetIdFromUidb64(System.String)~System.Int32")]
[assembly: SuppressMessage("Naming", "CA1704:Identifiers should be spelled correctly", Justification = "<>", Scope = "member", Target = "~M:BLL.Services.RegistrationService.SendActivationEmail(Core.Models.UserModel)")]
[assembly: SuppressMessage("Naming", "CA1704:Identifiers should be spelled correctly", Justification = "<>", Scope = "member", Target = "~M:BLL.Services.AccountActivationService.Activate(System.String,System.String)")]
