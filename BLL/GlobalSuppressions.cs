// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Naming", "CA1704:Identifiers should be spelled correctly", Justification = "<>", Scope = "type", Target = "~T:BLL.Services.UserServices.JwtService")]
[assembly: SuppressMessage("Naming", "CA1704:Identifiers should be spelled correctly", Justification = "<>", Scope = "member", Target = "~M:BLL.Services.UserServices.EmailService.SendEmail(System.String,System.String,System.String)")]
[assembly: SuppressMessage("Naming", "CA1704:Identifiers should be spelled correctly", Justification = "<>", Scope = "member", Target = "~M:BLL.Services.UserServices.UserService.Update(Core.Models.UserModels.UserUpdateModel)")]
[assembly: SuppressMessage("Naming", "CA1704:Identifiers should be spelled correctly", Justification = "<>", Scope = "member", Target = "~M:BLL.Services.UserServices.UserService.CreateNonActiveUser(Core.Models.UserModels.UserCreateModel)")]
[assembly: SuppressMessage("Naming", "CA1704:Identifiers should be spelled correctly", Justification = "<>", Scope = "member", Target = "~F:BLL.Services.UserServices.LoginService.jwtService")]
[assembly: SuppressMessage("Naming", "CA1704:Identifiers should be spelled correctly", Justification = "<>", Scope = "member", Target = "~M:BLL.Services.UserServices.SHA256HashingService.Hash(System.String)~System.String")]
[assembly: SuppressMessage("Naming", "CA1704:Identifiers should be spelled correctly", Justification = "<>", Scope = "member", Target = "~M:BLL.Services.UserServices.RegistrationService.MapUserRegistrationModel(Core.Models.UserModels.UserRegistrationModel)~Core.Models.UserModels.UserCreateModel")]
[assembly: SuppressMessage("Naming", "CA1704:Identifiers should be spelled correctly", Justification = "<>", Scope = "member", Target = "~M:BLL.Services.UserServices.TokenGeneratorService.GetUidb64(Core.Models.UserModels.UserModel)~System.String")]
[assembly: SuppressMessage("Naming", "CA1704:Identifiers should be spelled correctly", Justification = "<>", Scope = "member", Target = "~M:BLL.Services.UserServices.RegistrationService.SendActivationEmail(Core.Models.UserModels.UserModel)")]
[assembly: SuppressMessage("Naming", "CA1704:Identifiers should be spelled correctly", Justification = "<>", Scope = "member", Target = "~F:BLL.Services.UserServices.AuthenticationService.jwtService")]
[assembly: SuppressMessage("Naming", "CA1704:Identifiers should be spelled correctly", Justification = "<>", Scope = "member", Target = "~M:BLL.Services.UserServices.AuthenticationService.#ctor(BLL.Abstractions.Interfaces.UserInterfaces.IUserService,BLL.Abstractions.Interfaces.UserInterfaces.IJwtService)")]
[assembly: SuppressMessage("Naming", "CA1704:Identifiers should be spelled correctly", Justification = "<>", Scope = "member", Target = "~M:BLL.Services.UserServices.TokenGeneratorService.GetIdFromUidb64(System.String)~Core.DataClasses.OptionalResult{System.Int32}")]
[assembly: SuppressMessage("Naming", "CA1704:Identifiers should be spelled correctly", Justification = "<>", Scope = "member", Target = "~M:BLL.Services.UserServices.UserService.MapUserCreateModel(Core.Models.UserModels.UserCreateModel)~System.Threading.Tasks.Task{Core.Models.UserModels.UserModel}")]
[assembly: SuppressMessage("Naming", "CA1704:Identifiers should be spelled correctly", Justification = "<>", Scope = "member", Target = "~M:BLL.Services.UserServices.UserService.MapUserUpdateModel(Core.Models.UserModels.UserUpdateModel)~System.Threading.Tasks.Task{Core.Models.UserModels.UserModel}")]
[assembly: SuppressMessage("Naming", "CA1704:Identifiers should be spelled correctly", Justification = "<>", Scope = "member", Target = "~M:BLL.Services.UserServices.AuthenticationService.GetUserByToken(System.String)~System.Threading.Tasks.Task{Core.DataClasses.OptionalResult{Core.Models.UserModels.UserModel}}")]
[assembly: SuppressMessage("Naming", "CA1704:Identifiers should be spelled correctly", Justification = "<>", Scope = "member", Target = "~M:BLL.Services.UserServices.EditUserInfoService.SendChangeEmail(Core.Models.UserModels.UserModel)~System.Threading.Tasks.Task")]
[assembly: SuppressMessage("Naming", "CA1704:Identifiers should be spelled correctly", Justification = "<>", Scope = "member", Target = "~M:BLL.Services.UserServices.LoginService.#ctor(BLL.Abstractions.Interfaces.UserInterfaces.IUserService,BLL.Abstractions.Interfaces.UserInterfaces.IJwtService,BLL.Abstractions.Interfaces.UserInterfaces.IHashingService)")]
