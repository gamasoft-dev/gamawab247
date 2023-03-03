using System;
using System.Collections.Generic;
using Domain.Entities.DialogMessageEntitties.ValueObjects;
using Domain.Entities.FormProcessing;
using Domain.Entities.FormProcessing.ValueObjects;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    public partial class reinitializedb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BusinessConversations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BusinessId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessConversations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BusinessFormConlusionConfigs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BusinessFormId = table.Column<Guid>(type: "uuid", nullable: false),
                    ConfigDetails = table.Column<List<FormConclusionProcessesConfig>>(type: "jsonb", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessFormConlusionConfigs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InboundMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Wa_Id = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Body = table.Column<string>(type: "text", nullable: true),
                    From = table.Column<string>(type: "text", nullable: true),
                    To = table.Column<string>(type: "text", nullable: true),
                    WhatsAppMessageId = table.Column<string>(type: "text", nullable: true),
                    WhatsUserName = table.Column<string>(type: "text", nullable: true),
                    Timestamp = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<string>(type: "text", nullable: true),
                    MsgOptionId = table.Column<string>(type: "text", nullable: true),
                    IsFirstMessageSent = table.Column<bool>(type: "boolean", nullable: false),
                    ResponseProcessingStatus = table.Column<string>(type: "text", nullable: true),
                    SendAttempt = table.Column<int>(type: "integer", nullable: false),
                    BusinessId = table.Column<Guid>(type: "uuid", nullable: false),
                    CanUseNLPMapping = table.Column<bool>(type: "boolean", nullable: false),
                    Language = table.Column<string>(type: "text", nullable: true),
                    ContextMessageId = table.Column<string>(type: "text", nullable: true),
                    WhatsAppId = table.Column<string>(type: "text", nullable: true),
                    ErrorMessage = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InboundMessages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Industries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Industries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Partners",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    BusinessId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Partners", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SystemSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BaseWebhook = table.Column<string>(type: "text", nullable: true),
                    MaxTestCount = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WhatsappUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WaId = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: false),
                    LastMessageTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WhatsappUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleClaims_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Businesses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false),
                    AvatarUrl = table.Column<string>(type: "text", nullable: true),
                    Slogan = table.Column<string>(type: "text", nullable: true),
                    RCNumber = table.Column<string>(type: "text", nullable: true),
                    Address = table.Column<string>(type: "text", nullable: true),
                    IndustryId = table.Column<Guid>(type: "uuid", nullable: false),
                    AdminUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    BusinessMessageSettingsId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Businesses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Businesses_Industries_IndustryId",
                        column: x => x.IndustryId,
                        principalTable: "Industries",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "BusinessMessageSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WebhookUrl = table.Column<string>(type: "text", nullable: true),
                    ApiKey = table.Column<string>(type: "text", nullable: true),
                    BotName = table.Column<string>(type: "text", nullable: true),
                    BotDescription = table.Column<string>(type: "text", nullable: true),
                    TestCounter = table.Column<int>(type: "integer", nullable: false),
                    IsTest = table.Column<bool>(type: "boolean", nullable: false),
                    IsWebhookConfigured = table.Column<bool>(type: "boolean", nullable: false),
                    BusinessId = table.Column<Guid>(type: "uuid", nullable: false),
                    BusinessId1 = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessMessageSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BusinessMessageSettings_Businesses_BusinessId1",
                        column: x => x.BusinessId1,
                        principalTable: "Businesses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MessageLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RequestResponseData = table.Column<string>(type: "text", nullable: false),
                    MessageType = table.Column<int>(type: "integer", nullable: false),
                    MessageDirection = table.Column<int>(type: "integer", nullable: false),
                    MessageBody = table.Column<string>(type: "text", nullable: true),
                    From = table.Column<string>(type: "text", nullable: true),
                    To = table.Column<string>(type: "text", nullable: true),
                    IsRead = table.Column<bool>(type: "boolean", nullable: false),
                    WhatsappUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    BusinessId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MessageLogs_Businesses_BusinessId",
                        column: x => x.BusinessId,
                        principalTable: "Businesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MessageLogs_WhatsappUsers_WhatsappUserId",
                        column: x => x.WhatsappUserId,
                        principalTable: "WhatsappUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    Verified = table.Column<bool>(type: "boolean", nullable: false),
                    LastLogin = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: true),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: true),
                    BusinessId = table.Column<Guid>(type: "uuid", nullable: true),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Businesses_BusinessId",
                        column: x => x.BusinessId,
                        principalTable: "Businesses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Tokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TokenType = table.Column<int>(type: "integer", nullable: false),
                    OTPToken = table.Column<string>(type: "text", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsValid = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserActivities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EventType = table.Column<string>(type: "text", nullable: false),
                    ObjectClass = table.Column<string>(type: "text", nullable: false),
                    ObjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    Details = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserActivities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserActivities_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserClaims_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_UserLogins_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserToken",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: true),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserToken", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_UserToken_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BusinessForms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BusinessConversationId = table.Column<Guid>(type: "uuid", nullable: true),
                    FormElements = table.Column<List<FormElement>>(type: "jsonb", nullable: true),
                    Headers = table.Column<List<KeyValueObj>>(type: "jsonb", nullable: true),
                    ResponseKvps = table.Column<List<FormResponseKvp>>(type: "jsonb", nullable: true),
                    IsFormToBeSubmittedToUrl = table.Column<bool>(type: "boolean", nullable: false),
                    SubmissionUrl = table.Column<string>(type: "text", nullable: true),
                    UrlMethodType = table.Column<int>(type: "integer", nullable: false),
                    Counter = table.Column<int>(type: "integer", nullable: false),
                    IsRequestSuccessful = table.Column<bool>(type: "boolean", nullable: false),
                    BusinessId = table.Column<Guid>(type: "uuid", nullable: false),
                    ConclusionBusinessMessageId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessForms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BusinessForms_Businesses_BusinessId",
                        column: x => x.BusinessId,
                        principalTable: "Businesses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "BusinessMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BusinessConversationId = table.Column<Guid>(type: "uuid", nullable: true),
                    Position = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    HasFollowUpMessage = table.Column<bool>(type: "boolean", nullable: false),
                    FollowParentMessageId = table.Column<Guid>(type: "uuid", nullable: true),
                    MessageType = table.Column<string>(type: "text", nullable: true),
                    RecipientType = table.Column<string>(type: "text", nullable: true),
                    InteractiveMessageId = table.Column<Guid>(type: "uuid", nullable: true),
                    BusinessId = table.Column<Guid>(type: "uuid", nullable: false),
                    ShouldTriggerFormProcessing = table.Column<bool>(type: "boolean", nullable: false),
                    BusinessFormId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BusinessMessages_BusinessConversations_BusinessConversation~",
                        column: x => x.BusinessConversationId,
                        principalTable: "BusinessConversations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BusinessMessages_Businesses_BusinessId",
                        column: x => x.BusinessId,
                        principalTable: "Businesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BusinessMessages_BusinessForms_BusinessFormId",
                        column: x => x.BusinessFormId,
                        principalTable: "BusinessForms",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "FormRequestResponses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Message = table.Column<string>(type: "text", nullable: true),
                    IsValidationResponse = table.Column<bool>(type: "boolean", nullable: false),
                    Direction = table.Column<string>(type: "text", nullable: true),
                    FormElement = table.Column<string>(type: "text", nullable: true),
                    MessageType = table.Column<string>(type: "text", nullable: true),
                    From = table.Column<string>(type: "text", nullable: true),
                    To = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: true),
                    ErrorMessage = table.Column<string>(type: "text", nullable: true),
                    BusinessFormId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsSummaryMessage = table.Column<bool>(type: "boolean", nullable: false),
                    BusinessId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormRequestResponses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FormRequestResponses_BusinessForms_BusinessFormId",
                        column: x => x.BusinessFormId,
                        principalTable: "BusinessForms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserFormDatas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BusinessId = table.Column<Guid>(type: "uuid", nullable: false),
                    BusinessFormId = table.Column<Guid>(type: "uuid", nullable: false),
                    DialogSessionId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserInputDetails = table.Column<List<UserInputDetail>>(type: "jsonb", nullable: true),
                    IsFormCompleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFormDatas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserFormDatas_BusinessForms_BusinessFormId",
                        column: x => x.BusinessFormId,
                        principalTable: "BusinessForms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ListMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SectionTitle = table.Column<string>(type: "text", nullable: true),
                    ButtonMessage = table.Column<string>(type: "text", nullable: true),
                    ListAction = table.Column<ListAction>(type: "jsonb", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: true),
                    BusinessMessageId = table.Column<Guid>(type: "uuid", nullable: false),
                    Header = table.Column<string>(type: "text", nullable: true),
                    Body = table.Column<string>(type: "text", nullable: true),
                    Footer = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ListMessages_BusinessMessages_BusinessMessageId",
                        column: x => x.BusinessMessageId,
                        principalTable: "BusinessMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OutboundMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    IsFirstMessageSent = table.Column<bool>(type: "boolean", nullable: false),
                    MessageType = table.Column<string>(type: "text", nullable: true),
                    RecipientWhatsappId = table.Column<string>(type: "text", nullable: true),
                    Body = table.Column<string>(type: "text", nullable: true),
                    From = table.Column<string>(type: "text", nullable: true),
                    WhatsAppMessageId = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<string>(type: "text", nullable: true),
                    BusinessId = table.Column<Guid>(type: "uuid", nullable: false),
                    BusinessMessageId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboundMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OutboundMessages_BusinessMessages_BusinessMessageId",
                        column: x => x.BusinessMessageId,
                        principalTable: "BusinessMessages",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ReplyButtonMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ButtonAction = table.Column<ButtonAction>(type: "jsonb", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: true),
                    BusinessMessageId = table.Column<Guid>(type: "uuid", nullable: false),
                    Header = table.Column<string>(type: "text", nullable: true),
                    Body = table.Column<string>(type: "text", nullable: true),
                    Footer = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReplyButtonMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReplyButtonMessages_BusinessMessages_BusinessMessageId",
                        column: x => x.BusinessMessageId,
                        principalTable: "BusinessMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TextMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NextMessagePosition = table.Column<int>(type: "integer", nullable: false),
                    KeyResponses = table.Column<string>(type: "text", nullable: true),
                    IsResponsePermitted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: true),
                    BusinessMessageId = table.Column<Guid>(type: "uuid", nullable: false),
                    Header = table.Column<string>(type: "text", nullable: true),
                    Body = table.Column<string>(type: "text", nullable: true),
                    Footer = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TextMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TextMessages_BusinessMessages_BusinessMessageId",
                        column: x => x.BusinessMessageId,
                        principalTable: "BusinessMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Businesses_AdminUserId",
                table: "Businesses",
                column: "AdminUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Businesses_BusinessMessageSettingsId",
                table: "Businesses",
                column: "BusinessMessageSettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_Businesses_Email",
                table: "Businesses",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Businesses_IndustryId",
                table: "Businesses",
                column: "IndustryId");

            migrationBuilder.CreateIndex(
                name: "IX_Businesses_Name",
                table: "Businesses",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Businesses_PhoneNumber",
                table: "Businesses",
                column: "PhoneNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BusinessForms_BusinessId",
                table: "BusinessForms",
                column: "BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessForms_ConclusionBusinessMessageId",
                table: "BusinessForms",
                column: "ConclusionBusinessMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessMessages_BusinessConversationId",
                table: "BusinessMessages",
                column: "BusinessConversationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BusinessMessages_BusinessFormId",
                table: "BusinessMessages",
                column: "BusinessFormId");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessMessages_BusinessId",
                table: "BusinessMessages",
                column: "BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessMessages_MessageType",
                table: "BusinessMessages",
                column: "MessageType");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessMessages_Name",
                table: "BusinessMessages",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessMessages_Position",
                table: "BusinessMessages",
                column: "Position");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessMessageSettings_ApiKey",
                table: "BusinessMessageSettings",
                column: "ApiKey");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessMessageSettings_BusinessId1",
                table: "BusinessMessageSettings",
                column: "BusinessId1");

            migrationBuilder.CreateIndex(
                name: "IX_FormRequestResponses_BusinessFormId",
                table: "FormRequestResponses",
                column: "BusinessFormId");

            migrationBuilder.CreateIndex(
                name: "IX_FormRequestResponses_CreatedAt",
                table: "FormRequestResponses",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_FormRequestResponses_From",
                table: "FormRequestResponses",
                column: "From");

            migrationBuilder.CreateIndex(
                name: "IX_FormRequestResponses_Status",
                table: "FormRequestResponses",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_FormRequestResponses_To",
                table: "FormRequestResponses",
                column: "To");

            migrationBuilder.CreateIndex(
                name: "IX_InboundMessages_BusinessId",
                table: "InboundMessages",
                column: "BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_InboundMessages_ContextMessageId",
                table: "InboundMessages",
                column: "ContextMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_InboundMessages_CreatedAt",
                table: "InboundMessages",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_InboundMessages_From",
                table: "InboundMessages",
                column: "From");

            migrationBuilder.CreateIndex(
                name: "IX_InboundMessages_MsgOptionId",
                table: "InboundMessages",
                column: "MsgOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_InboundMessages_Type",
                table: "InboundMessages",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_InboundMessages_Wa_Id",
                table: "InboundMessages",
                column: "Wa_Id");

            migrationBuilder.CreateIndex(
                name: "IX_InboundMessages_WhatsAppId",
                table: "InboundMessages",
                column: "WhatsAppId");

            migrationBuilder.CreateIndex(
                name: "IX_InboundMessages_WhatsAppMessageId",
                table: "InboundMessages",
                column: "WhatsAppMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_Industries_Name",
                table: "Industries",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ListMessages_BusinessMessageId",
                table: "ListMessages",
                column: "BusinessMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageLogs_BusinessId",
                table: "MessageLogs",
                column: "BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageLogs_CreatedAt",
                table: "MessageLogs",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_MessageLogs_From",
                table: "MessageLogs",
                column: "From");

            migrationBuilder.CreateIndex(
                name: "IX_MessageLogs_MessageDirection",
                table: "MessageLogs",
                column: "MessageDirection");

            migrationBuilder.CreateIndex(
                name: "IX_MessageLogs_To",
                table: "MessageLogs",
                column: "To");

            migrationBuilder.CreateIndex(
                name: "IX_MessageLogs_WhatsappUserId",
                table: "MessageLogs",
                column: "WhatsappUserId");

            migrationBuilder.CreateIndex(
                name: "IX_OutboundMessages_BusinessId",
                table: "OutboundMessages",
                column: "BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_OutboundMessages_BusinessMessageId",
                table: "OutboundMessages",
                column: "BusinessMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_OutboundMessages_CreatedAt",
                table: "OutboundMessages",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OutboundMessages_From",
                table: "OutboundMessages",
                column: "From");

            migrationBuilder.CreateIndex(
                name: "IX_OutboundMessages_RecipientWhatsappId",
                table: "OutboundMessages",
                column: "RecipientWhatsappId");

            migrationBuilder.CreateIndex(
                name: "IX_OutboundMessages_Type",
                table: "OutboundMessages",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_OutboundMessages_WhatsAppMessageId",
                table: "OutboundMessages",
                column: "WhatsAppMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_ReplyButtonMessages_BusinessMessageId",
                table: "ReplyButtonMessages",
                column: "BusinessMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleClaims_RoleId",
                table: "RoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "Roles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TextMessages_BusinessMessageId",
                table: "TextMessages",
                column: "BusinessMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_Tokens_UserId",
                table: "Tokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserActivities_UserId",
                table: "UserActivities",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserClaims_UserId",
                table: "UserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserFormDatas_BusinessFormId",
                table: "UserFormDatas",
                column: "BusinessFormId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLogins_UserId",
                table: "UserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "Users",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_Users_BusinessId",
                table: "Users",
                column: "BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "Users",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WhatsappUsers_PhoneNumber",
                table: "WhatsappUsers",
                column: "PhoneNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WhatsappUsers_WaId",
                table: "WhatsappUsers",
                column: "WaId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Businesses_BusinessMessageSettings_BusinessMessageSettingsId",
                table: "Businesses",
                column: "BusinessMessageSettingsId",
                principalTable: "BusinessMessageSettings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Businesses_Users_AdminUserId",
                table: "Businesses",
                column: "AdminUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BusinessForms_BusinessMessages_ConclusionBusinessMessageId",
                table: "BusinessForms",
                column: "ConclusionBusinessMessageId",
                principalTable: "BusinessMessages",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Businesses_BusinessMessageSettings_BusinessMessageSettingsId",
                table: "Businesses");

            migrationBuilder.DropForeignKey(
                name: "FK_Businesses_Industries_IndustryId",
                table: "Businesses");

            migrationBuilder.DropForeignKey(
                name: "FK_Businesses_Users_AdminUserId",
                table: "Businesses");

            migrationBuilder.DropForeignKey(
                name: "FK_BusinessForms_Businesses_BusinessId",
                table: "BusinessForms");

            migrationBuilder.DropForeignKey(
                name: "FK_BusinessMessages_Businesses_BusinessId",
                table: "BusinessMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_BusinessForms_BusinessMessages_ConclusionBusinessMessageId",
                table: "BusinessForms");

            migrationBuilder.DropTable(
                name: "BusinessFormConlusionConfigs");

            migrationBuilder.DropTable(
                name: "FormRequestResponses");

            migrationBuilder.DropTable(
                name: "InboundMessages");

            migrationBuilder.DropTable(
                name: "ListMessages");

            migrationBuilder.DropTable(
                name: "MessageLogs");

            migrationBuilder.DropTable(
                name: "OutboundMessages");

            migrationBuilder.DropTable(
                name: "Partners");

            migrationBuilder.DropTable(
                name: "ReplyButtonMessages");

            migrationBuilder.DropTable(
                name: "RoleClaims");

            migrationBuilder.DropTable(
                name: "SystemSettings");

            migrationBuilder.DropTable(
                name: "TextMessages");

            migrationBuilder.DropTable(
                name: "Tokens");

            migrationBuilder.DropTable(
                name: "UserActivities");

            migrationBuilder.DropTable(
                name: "UserClaims");

            migrationBuilder.DropTable(
                name: "UserFormDatas");

            migrationBuilder.DropTable(
                name: "UserLogins");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "UserToken");

            migrationBuilder.DropTable(
                name: "WhatsappUsers");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "BusinessMessageSettings");

            migrationBuilder.DropTable(
                name: "Industries");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Businesses");

            migrationBuilder.DropTable(
                name: "BusinessMessages");

            migrationBuilder.DropTable(
                name: "BusinessConversations");

            migrationBuilder.DropTable(
                name: "BusinessForms");
        }
    }
}
