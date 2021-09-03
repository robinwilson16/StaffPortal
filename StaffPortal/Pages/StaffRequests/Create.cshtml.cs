using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using StaffPortal.Data;
using StaffPortal.Models;
using StaffPortal.Shared;

namespace StaffPortal.Pages.StaffRequests
{
    public class CreateModel : PageModel
    {
        private readonly StaffPortal.Data.ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly string _destinationEmailAddress;

        public CreateModel(
            ApplicationDbContext context,
            IConfiguration configuration,
            UserManager<IdentityUser> userManager, 
            IEmailSender emailSender
            )
        {
            _context = context;
            _configuration = configuration;
            _userManager = userManager;
            _emailSender = emailSender;
            _destinationEmailAddress = configuration.GetSection("SystemSettings").GetValue<string>("StaffRequestDestinationEmailAddress");
        }

        public async Task<IActionResult> OnGetAsync(string academicYear)
        {
            AcademicYearID = academicYear;
            DefaultAcademicYearID = await AcademicYearFunctions.GetDefaultAcademicYear(_context);
            if (AcademicYearID == null)
            {
                AcademicYearID = DefaultAcademicYearID;
            }

            IsSubmitted = false;
            RequestSuccessful = true;
            ErrorMessage = "";

            string selectListDomain = null;

            selectListDomain = "ROOM";
            RoomData = await _context.SelectListData
                .FromSqlInterpolated($"EXEC SPR_SPO_SelectListData @AcademicYear={AcademicYearID}, @Domain={selectListDomain}")
                .ToListAsync();
            ViewData["RoomID"] = new SelectList(RoomData, "Code", "Description");

            selectListDomain = "CAMPUS";
            CampusData = await _context.SelectListData
                .FromSqlInterpolated($"EXEC SPR_SPO_SelectListData @AcademicYear={AcademicYearID}, @Domain={selectListDomain}")
                .ToListAsync();
            ViewData["CampusID"] = new SelectList(CampusData, "Code", "Description");

            selectListDomain = "TEAM";
            TeamData = await _context.SelectListData
                .FromSqlInterpolated($"EXEC SPR_SPO_SelectListData @AcademicYear={AcademicYearID}, @Domain={selectListDomain}")
                .ToListAsync();
            ViewData["TeamID"] = new SelectList(TeamData, "Code", "Description");

            selectListDomain = "EMPLOYEE_TYPE";
            EmployeeData = await _context.SelectListData
                .FromSqlInterpolated($"EXEC SPR_SPO_SelectListData @AcademicYear={AcademicYearID}, @Domain={selectListDomain}")
                .ToListAsync();
            ViewData["EmployeeTypeID"] = new SelectList(EmployeeData, "Code", "Description");

            return Page();
        }

        [BindProperty]
        public StaffRequest StaffRequest { get; set; }

        public string AcademicYearID { get; set; }
        public string DefaultAcademicYearID { get; set; }

        public IList<SelectListData> RoomData { get; set; }
        public IList<SelectListData> CampusData { get; set; }
        public IList<SelectListData> TeamData { get; set; }
        public IList<SelectListData> EmployeeData { get; set; }

        public bool IsSubmitted { get; set; } 
        public bool RequestSuccessful { get; set; }

        public string ErrorMessage { get; set; }

        public SubmitResult SubmitResult { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync(string academicYear)
        {
            SubmitResult = new SubmitResult
            {
                SubmitResultID = "CREATE_USER"
            };

            AcademicYearID = academicYear;
            DefaultAcademicYearID = await AcademicYearFunctions.GetDefaultAcademicYear(_context);
            if (AcademicYearID == null)
            {
                AcademicYearID = DefaultAcademicYearID;
            }

            string username = Identity.GetUserName(User, _context);
            string userFullName = await Identity.GetFullName(AcademicYearID, username, _context);

            IsSubmitted = true;

            if (!ModelState.IsValid)
            {
                SubmitResult.IsSuccessful = false;
                SubmitResult.ErrorLevel = ErrorLevel.Error;
                SubmitResult.ErrorCode = "MODEL";
                SubmitResult.ErrorDescription = "Data Model Is Invalid";
                return new JsonResult(SubmitResult);
                //return Page();
            }

            string selectListDomain = null;

            selectListDomain = "ROOM";
            RoomData = await _context.SelectListData
                .FromSqlInterpolated($"EXEC SPR_SPO_SelectListData @AcademicYear={AcademicYearID}, @Domain={selectListDomain}")
                .ToListAsync();
            ViewData["RoomID"] = new SelectList(RoomData, "Code", "Description");

            selectListDomain = "CAMPUS";
            CampusData = await _context.SelectListData
                .FromSqlInterpolated($"EXEC SPR_SPO_SelectListData @AcademicYear={AcademicYearID}, @Domain={selectListDomain}")
                .ToListAsync();
            ViewData["CampusID"] = new SelectList(CampusData, "Code", "Description");

            selectListDomain = "TEAM";
            TeamData = await _context.SelectListData
                .FromSqlInterpolated($"EXEC SPR_SPO_SelectListData @AcademicYear={AcademicYearID}, @Domain={selectListDomain}")
                .ToListAsync();
            ViewData["TeamID"] = new SelectList(TeamData, "Code", "Description");

            selectListDomain = "EMPLOYEE_TYPE";
            EmployeeData = await _context.SelectListData
                .FromSqlInterpolated($"EXEC SPR_SPO_SelectListData @AcademicYear={AcademicYearID}, @Domain={selectListDomain}")
                .ToListAsync();
            ViewData["EmployeeTypeID"] = new SelectList(EmployeeData, "Code", "Description");

            //Get descriptions from drop-downs
            SelectListData roomDetails = RoomData.Where(m => m.Code == StaffRequest.RoomNumber).FirstOrDefault();
            string roomName = null;
            if (roomDetails != null)
            {
                roomName = roomDetails.Description;
            }
            else
            {
                roomName = StaffRequest.RoomNumber;
            }

            SelectListData campusDetails = CampusData.Where(m => m.Code == StaffRequest.Campus).FirstOrDefault();
            string campusName = null;
            if (campusDetails != null)
            {
                campusName = StaffRequest.Campus + " - " + campusDetails.Description;
            }
            else
            {
                campusName = StaffRequest.Campus;
            }

            SelectListData teamDetails = TeamData.Where(m => m.Code == StaffRequest.Department).FirstOrDefault();
            string teamName = null;
            if (teamDetails != null)
            {
                teamName = StaffRequest.Department + " - " + teamDetails.Description;
            }
            else
            {
                teamName = StaffRequest.Department;
            }

            SelectListData employeeTypeDetails = EmployeeData.Where(m => m.Code == StaffRequest.EmployeeType).FirstOrDefault();
            string employeeTypeName = null;
            if (employeeTypeDetails != null)
            {
                employeeTypeName = employeeTypeDetails.Description;
            }
            else
            {
                employeeTypeName = StaffRequest.EmployeeType;
            }

            //Contents of email
            string emailTo = _destinationEmailAddress;
            string emailSubject = $"New Staff Member Request from Staff Portal for {StaffRequest.Forename} {StaffRequest.Surname}, Requested by {userFullName}";
            //string emailMsg = @$"<h4>Please create the following member of staff requested by {userFullName}</h4>";

            //Taken from https://beefree.io/editor/?template=marketing-webinar-invite&type=email
            string emailMsg =
                    @"
                    <!DOCTYPE html>
<html lang=""en"" xmlns:o=""urn:schemas-microsoft-com:office:office"" xmlns:v=""urn:schemas-microsoft-com:vml"">

<head>
    <title></title>
    <meta charset=""utf-8"" />
    <meta content=""width=device-width,initial-scale=1"" name=""viewport"" />
    <!--[if mso]><xml><o:OfficeDocumentSettings><o:PixelsPerInch>96</o:PixelsPerInch><o:AllowPNG/></o:OfficeDocumentSettings></xml><![endif]-->
    <style>
        * {
            box-sizing: border-box
        }

        th.column {
            padding: 0
        }

        a[x-apple-data-detectors] {
            color: inherit !important;
            text-decoration: inherit !important
        }

        #MessageViewBody a {
            color: inherit;
            text-decoration: none
        }

        p {
            line-height: inherit
        }

        @media (max-width:620px) {
            .icons-inner {
                text-align: center
            }

            .icons-inner td {
                margin: 0 auto
            }

            .row-content {
                width: 100% !important
            }

            .stack .column {
                width: 100%;
                display: block
            }
        }
    </style>
</head>

<body style=""background-color:#fff;margin:0;padding:0;-webkit-text-size-adjust:none;text-size-adjust:none"">
    <table border=""0"" cellpadding=""0"" cellspacing=""0"" class=""nl-container"" role=""presentation""
        style=""mso-table-lspace:0;mso-table-rspace:0;background-color:#fff"" width=""100%"">
        <tbody>
            <tr>
                <td>
                    <table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" class=""row row-1""
                        role=""presentation"" style=""mso-table-lspace:0;mso-table-rspace:0"" width=""100%"">
                        <tbody>
                            <tr>
                                <td>
                                    <table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0""
                                        class=""row-content stack"" role=""presentation""
                                        style=""mso-table-lspace:0;mso-table-rspace:0;background-color:#fff"" width=""600"">
                                        <tbody>
                                            <tr>
                                                <th class=""column""
                                                    style=""mso-table-lspace:0;mso-table-rspace:0;font-weight:400;text-align:left;vertical-align:top;padding-left:10px;padding-right:5px""
                                                    width=""100%"">
                                                    <table border=""0"" cellpadding=""0"" cellspacing=""0""
                                                        class=""image_block"" role=""presentation""
                                                        style=""mso-table-lspace:0;mso-table-rspace:0"" width=""100%"">
                                                        <tr>
                                                            <td
                                                                style=""width:100%;padding-right:0;padding-left:0;padding-top:5px;padding-bottom:10px"">
                                                                <div align=""center"" style=""line-height:10px""><a
                                                                        href=""www.wlc.ac.uk"" style=""outline:0""
                                                                        tabindex=""-1"" target=""_blank""><img
                                                                            alt=""West London College""
                                                                            src=""https://www.wlc.ac.uk/templates/qlue/images/west-london-college.png""
                                                                            style=""display:block;height:auto;border:0;width:176px;max-width:100%""
                                                                            title=""West London College""
                                                                            width=""176"" /></a></div>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </th>
                                            </tr>
                                        </tbody>
                                    </table>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                    <table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" class=""row row-2""
                        role=""presentation"" style=""mso-table-lspace:0;mso-table-rspace:0"" width=""100%"">
                        <tbody>
                            <tr>
                                <td>
                                    <table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0""
                                        class=""row-content stack"" role=""presentation""
                                        style=""mso-table-lspace:0;mso-table-rspace:0;background-color:#212529"" width=""600"">
                                        <tbody>
                                            <tr>
                                                <th class=""column""
                                                    style=""mso-table-lspace:0;mso-table-rspace:0;font-weight:400;text-align:left;vertical-align:top;padding-left:10px;padding-right:5px""
                                                    width=""100%"">
                                                    <table border=""0"" cellpadding=""0"" cellspacing=""0""
                                                        class=""heading_block"" role=""presentation""
                                                        style=""mso-table-lspace:0;mso-table-rspace:0"" width=""100%"">
                                                        <tr>
                                                            <td
                                                                style=""padding-bottom:15px;padding-top:60px;text-align:center;width:100%"">
                                                                <h1
                                                                    style=""margin:0;color:#fff;direction:ltr;font-family:Helvetica Neue,Helvetica,Arial,sans-serif;font-size:23px;font-weight:400;letter-spacing:3px;line-height:120%;text-align:left;margin-top:0;margin-bottom:0"">
                                                                    NEW STAFF MEMBER REQUEST:</h1>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                    <table border=""0"" cellpadding=""0"" cellspacing=""0""
                                                        class=""heading_block"" role=""presentation""
                                                        style=""mso-table-lspace:0;mso-table-rspace:0"" width=""100%"">
                                                        <tr>
                                                            <td style=""text-align:center;width:100%"">
                                                                <h1
                                                                    style=""margin:0;color:#fff;direction:ltr;font-family:Helvetica Neue,Helvetica,Arial,sans-serif;font-size:26px;font-weight:400;letter-spacing:normal;line-height:120%;text-align:left;margin-top:0;margin-bottom:0"">
                                                                    <strong>New Staff Member Request from Staff Portal
                                                                        for " + StaffRequest.Forename + @" " + StaffRequest.Surname + @"</strong></h1>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                    <table border=""0"" cellpadding=""0"" cellspacing=""0"" class=""text_block""
                                                        role=""presentation""
                                                        style=""mso-table-lspace:0;mso-table-rspace:0;word-break:break-word""
                                                        width=""100%"">
                                                        <tr>
                                                            <td style=""padding-bottom:25px;padding-top:20px"">
                                                                <div style=""font-family:sans-serif"">
                                                                    <div
                                                                        style=""font-size:14px;color:#fff;line-height:1.2;font-family:Helvetica Neue,Helvetica,Arial,sans-serif"">
                                                                        <p
                                                                            style=""margin:0;font-size:14px;letter-spacing:1px"">
                                                                            Requested by " + userFullName + @"</p>
                                                                    </div>
                                                                </div>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </th>
                                            </tr>
                                        </tbody>
                                    </table>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                    <table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" class=""row row-3""
                        role=""presentation"" style=""mso-table-lspace:0;mso-table-rspace:0"" width=""100%"">
                        <tbody>
                            <tr>
                                <td>
                                    <table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0""
                                        class=""row-content stack"" role=""presentation""
                                        style=""mso-table-lspace:0;mso-table-rspace:0;background-color:#fbfbfb""
                                        width=""600"">
                                        <tbody>
                                            <tr>
                                                <th class=""column""
                                                    style=""mso-table-lspace:0;mso-table-rspace:0;font-weight:400;text-align:left;vertical-align:top""
                                                    width=""100%"">
                                                    <table border=""0"" cellpadding=""0"" cellspacing=""0""
                                                        class=""divider_block"" role=""presentation""
                                                        style=""mso-table-lspace:0;mso-table-rspace:0"" width=""100%"">
                                                        <tr>
                                                            <td
                                                                style=""padding-bottom:10px;padding-left:5px;padding-right:5px;padding-top:40px"">
                                                                <div align=""center"">
                                                                    <table border=""0"" cellpadding=""0"" cellspacing=""0""
                                                                        role=""presentation""
                                                                        style=""mso-table-lspace:0;mso-table-rspace:0""
                                                                        width=""10%"">
                                                                        <tr>
                                                                            <td class=""divider_inner""
                                                                                style=""font-size:1px;line-height:1px;border-top:4px dotted #009ad2"">
                                                                                <span></span></td>
                                                                        </tr>
                                                                    </table>
                                                                </div>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                    <table border=""0"" cellpadding=""0"" cellspacing=""0""
                                                        class=""heading_block"" role=""presentation""
                                                        style=""mso-table-lspace:0;mso-table-rspace:0"" width=""100%"">
                                                        <tr>
                                                            <td style=""text-align:center;width:100%"">
                                                                <h1
                                                                    style=""margin:0;color:#555;direction:ltr;font-family:Helvetica Neue,Helvetica,Arial,sans-serif;font-size:26px;font-weight:400;letter-spacing:normal;line-height:120%;text-align:center;margin-top:0;margin-bottom:0"">
                                                                    <strong>STAFF MEMBER DETAILS</strong></h1>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                    <table border=""0"" cellpadding=""0"" cellspacing=""0"" class=""text_block""
                                                        role=""presentation""
                                                        style=""mso-table-lspace:0;mso-table-rspace:0;word-break:break-word""
                                                        width=""100%"">
                                                        <tr>
                                                            <td
                                                                style=""padding-bottom:30px;padding-left:20px;padding-right:20px;padding-top:10px"">
                                                                <div style=""font-family:sans-serif"">
                                                                    <div
                                                                        style=""font-size:14px;color:#393d47;line-height:1.5;font-family:Helvetica Neue,Helvetica,Arial,sans-serif"">
                                                                        <p
                                                                            style=""margin:0;font-size:14px;text-align:center;mso-line-height-alt:27px"">
                                                                            <span style=""font-size:18px"">Please add the
                                                                                following person to the system:</span>
                                                                        </p>
                                                                    </div>
                                                                </div>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </th>
                                            </tr>
                                        </tbody>
                                    </table>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                    <table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" class=""row row-4""
                        role=""presentation"" style=""mso-table-lspace:0;mso-table-rspace:0"" width=""100%"">
                        <tbody>
                            <tr>
                                <td>
                                    <table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0""
                                        class=""row-content stack"" role=""presentation""
                                        style=""mso-table-lspace:0;mso-table-rspace:0;background-color:#fbfbfb""
                                        width=""600"">
                                        <tbody>
                                            <tr>
                                                <th class=""column""
                                                    style=""mso-table-lspace:0;mso-table-rspace:0;font-weight:400;text-align:left;vertical-align:top""
                                                    width=""50%"">
                                                    <table border=""0"" cellpadding=""0"" cellspacing=""0"" class=""text_block""
                                                        role=""presentation""
                                                        style=""mso-table-lspace:0;mso-table-rspace:0;word-break:break-word""
                                                        width=""100%"">
                                                        <tr>
                                                            <td
                                                                style=""padding-bottom:15px;padding-left:10px;padding-right:10px;padding-top:15px"">
                                                                <div style=""font-family:sans-serif"">
                                                                    <div
                                                                        style=""font-size:14px;color:#555;line-height:1.2;font-family:Helvetica Neue,Helvetica,Arial,sans-serif"">
                                                                        <p style=""margin:0;font-size:14px"">Forename</p>
                                                                    </div>
                                                                </div>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </th>
                                                <th class=""column""
                                                    style=""mso-table-lspace:0;mso-table-rspace:0;font-weight:400;text-align:left;vertical-align:top""
                                                    width=""50%"">
                                                    <table border=""0"" cellpadding=""0"" cellspacing=""0"" class=""text_block""
                                                        role=""presentation""
                                                        style=""mso-table-lspace:0;mso-table-rspace:0;word-break:break-word""
                                                        width=""100%"">
                                                        <tr>
                                                            <td
                                                                style=""padding-bottom:15px;padding-left:10px;padding-right:10px;padding-top:15px"">
                                                                <div style=""font-family:sans-serif"">
                                                                    <div
                                                                        style=""font-size:14px;color:#555;line-height:1.2;font-family:Helvetica Neue,Helvetica,Arial,sans-serif"">
                                                                        <p style=""margin:0;font-size:14px"">" + StaffRequest.Forename + @"</p>
                                                                    </div>
                                                                </div>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </th>
                                            </tr>
                                        </tbody>
                                    </table>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                    <table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" class=""row row-5""
                        role=""presentation"" style=""mso-table-lspace:0;mso-table-rspace:0"" width=""100%"">
                        <tbody>
                            <tr>
                                <td>
                                    <table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0""
                                        class=""row-content stack"" role=""presentation""
                                        style=""mso-table-lspace:0;mso-table-rspace:0;background-color:#fbfbfb""
                                        width=""600"">
                                        <tbody>
                                            <tr>
                                                <th class=""column""
                                                    style=""mso-table-lspace:0;mso-table-rspace:0;font-weight:400;text-align:left;vertical-align:top""
                                                    width=""50%"">
                                                    <table border=""0"" cellpadding=""0"" cellspacing=""0"" class=""text_block""
                                                        role=""presentation""
                                                        style=""mso-table-lspace:0;mso-table-rspace:0;word-break:break-word""
                                                        width=""100%"">
                                                        <tr>
                                                            <td
                                                                style=""padding-bottom:15px;padding-left:10px;padding-right:10px;padding-top:15px"">
                                                                <div style=""font-family:sans-serif"">
                                                                    <div
                                                                        style=""font-size:14px;color:#555;line-height:1.2;font-family:Helvetica Neue,Helvetica,Arial,sans-serif"">
                                                                        <p style=""margin:0;font-size:14px"">Surname</p>
                                                                    </div>
                                                                </div>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </th>
                                                <th class=""column""
                                                    style=""mso-table-lspace:0;mso-table-rspace:0;font-weight:400;text-align:left;vertical-align:top""
                                                    width=""50%"">
                                                    <table border=""0"" cellpadding=""0"" cellspacing=""0"" class=""text_block""
                                                        role=""presentation""
                                                        style=""mso-table-lspace:0;mso-table-rspace:0;word-break:break-word""
                                                        width=""100%"">
                                                        <tr>
                                                            <td
                                                                style=""padding-top:15px;padding-right:10px;padding-bottom:15px;padding-left:10px"">
                                                                <div style=""font-family:sans-serif"">
                                                                    <div
                                                                        style=""font-size:14px;color:#555;line-height:1.2;font-family:Helvetica Neue,Helvetica,Arial,sans-serif"">
                                                                        <p style=""margin:0;font-size:14px"">" + StaffRequest.Surname + @"</p>
                                                                    </div>
                                                                </div>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </th>
                                            </tr>
                                        </tbody>
                                    </table>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                    <table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" class=""row row-6""
                        role=""presentation"" style=""mso-table-lspace:0;mso-table-rspace:0"" width=""100%"">
                        <tbody>
                            <tr>
                                <td>
                                    <table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0""
                                        class=""row-content stack"" role=""presentation""
                                        style=""mso-table-lspace:0;mso-table-rspace:0;background-color:#fbfbfb""
                                        width=""600"">
                                        <tbody>
                                            <tr>
                                                <th class=""column""
                                                    style=""mso-table-lspace:0;mso-table-rspace:0;font-weight:400;text-align:left;vertical-align:top""
                                                    width=""50%"">
                                                    <table border=""0"" cellpadding=""0"" cellspacing=""0"" class=""text_block""
                                                        role=""presentation""
                                                        style=""mso-table-lspace:0;mso-table-rspace:0;word-break:break-word""
                                                        width=""100%"">
                                                        <tr>
                                                            <td
                                                                style=""padding-bottom:15px;padding-left:10px;padding-right:10px;padding-top:15px"">
                                                                <div style=""font-family:sans-serif"">
                                                                    <div
                                                                        style=""font-size:14px;color:#555;line-height:1.2;font-family:Helvetica Neue,Helvetica,Arial,sans-serif"">
                                                                        <p style=""margin:0;font-size:14px"">Room No</p>
                                                                    </div>
                                                                </div>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </th>
                                                <th class=""column""
                                                    style=""mso-table-lspace:0;mso-table-rspace:0;font-weight:400;text-align:left;vertical-align:top""
                                                    width=""50%"">
                                                    <table border=""0"" cellpadding=""0"" cellspacing=""0"" class=""text_block""
                                                        role=""presentation""
                                                        style=""mso-table-lspace:0;mso-table-rspace:0;word-break:break-word""
                                                        width=""100%"">
                                                        <tr>
                                                            <td
                                                                style=""padding-bottom:15px;padding-left:10px;padding-right:10px;padding-top:15px"">
                                                                <div style=""font-family:sans-serif"">
                                                                    <div
                                                                        style=""font-size:14px;color:#555;line-height:1.2;font-family:Helvetica Neue,Helvetica,Arial,sans-serif"">
                                                                        <p style=""margin:0;font-size:14px"">" + roomName + @"</p>
                                                                    </div>
                                                                </div>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </th>
                                            </tr>
                                        </tbody>
                                    </table>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                    <table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" class=""row row-7""
                        role=""presentation"" style=""mso-table-lspace:0;mso-table-rspace:0"" width=""100%"">
                        <tbody>
                            <tr>
                                <td>
                                    <table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0""
                                        class=""row-content stack"" role=""presentation""
                                        style=""mso-table-lspace:0;mso-table-rspace:0;background-color:#fbfbfb""
                                        width=""600"">
                                        <tbody>
                                            <tr>
                                                <th class=""column""
                                                    style=""mso-table-lspace:0;mso-table-rspace:0;font-weight:400;text-align:left;vertical-align:top""
                                                    width=""50%"">
                                                    <table border=""0"" cellpadding=""0"" cellspacing=""0"" class=""text_block""
                                                        role=""presentation""
                                                        style=""mso-table-lspace:0;mso-table-rspace:0;word-break:break-word""
                                                        width=""100%"">
                                                        <tr>
                                                            <td
                                                                style=""padding-bottom:15px;padding-left:10px;padding-right:10px;padding-top:15px"">
                                                                <div style=""font-family:sans-serif"">
                                                                    <div
                                                                        style=""font-size:14px;color:#555;line-height:1.2;font-family:Helvetica Neue,Helvetica,Arial,sans-serif"">
                                                                        <p style=""margin:0;font-size:14px"">Campus</p>
                                                                    </div>
                                                                </div>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </th>
                                                <th class=""column""
                                                    style=""mso-table-lspace:0;mso-table-rspace:0;font-weight:400;text-align:left;vertical-align:top""
                                                    width=""50%"">
                                                    <table border=""0"" cellpadding=""0"" cellspacing=""0"" class=""text_block""
                                                        role=""presentation""
                                                        style=""mso-table-lspace:0;mso-table-rspace:0;word-break:break-word""
                                                        width=""100%"">
                                                        <tr>
                                                            <td
                                                                style=""padding-top:15px;padding-right:10px;padding-bottom:15px;padding-left:10px"">
                                                                <div style=""font-family:sans-serif"">
                                                                    <div
                                                                        style=""font-size:14px;color:#555;line-height:1.2;font-family:Helvetica Neue,Helvetica,Arial,sans-serif"">
                                                                        <p style=""margin:0;font-size:14px"">" + campusName + @"</p>
                                                                    </div>
                                                                </div>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </th>
                                            </tr>
                                        </tbody>
                                    </table>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                    <table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" class=""row row-8""
                        role=""presentation"" style=""mso-table-lspace:0;mso-table-rspace:0"" width=""100%"">
                        <tbody>
                            <tr>
                                <td>
                                    <table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0""
                                        class=""row-content stack"" role=""presentation""
                                        style=""mso-table-lspace:0;mso-table-rspace:0;background-color:#fbfbfb""
                                        width=""600"">
                                        <tbody>
                                            <tr>
                                                <th class=""column""
                                                    style=""mso-table-lspace:0;mso-table-rspace:0;font-weight:400;text-align:left;vertical-align:top""
                                                    width=""50%"">
                                                    <table border=""0"" cellpadding=""0"" cellspacing=""0"" class=""text_block""
                                                        role=""presentation""
                                                        style=""mso-table-lspace:0;mso-table-rspace:0;word-break:break-word""
                                                        width=""100%"">
                                                        <tr>
                                                            <td
                                                                style=""padding-bottom:15px;padding-left:10px;padding-right:10px;padding-top:15px"">
                                                                <div style=""font-family:sans-serif"">
                                                                    <div
                                                                        style=""font-size:14px;color:#555;line-height:1.2;font-family:Helvetica Neue,Helvetica,Arial,sans-serif"">
                                                                        <p style=""margin:0;font-size:14px"">Department
                                                                        </p>
                                                                    </div>
                                                                </div>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </th>
                                                <th class=""column""
                                                    style=""mso-table-lspace:0;mso-table-rspace:0;font-weight:400;text-align:left;vertical-align:top""
                                                    width=""50%"">
                                                    <table border=""0"" cellpadding=""0"" cellspacing=""0"" class=""text_block""
                                                        role=""presentation""
                                                        style=""mso-table-lspace:0;mso-table-rspace:0;word-break:break-word""
                                                        width=""100%"">
                                                        <tr>
                                                            <td
                                                                style=""padding-top:15px;padding-right:10px;padding-bottom:15px;padding-left:10px"">
                                                                <div style=""font-family:sans-serif"">
                                                                    <div
                                                                        style=""font-size:14px;color:#555;line-height:1.2;font-family:Helvetica Neue,Helvetica,Arial,sans-serif"">
                                                                        <p style=""margin:0;font-size:14px"">" + teamName + @"
                                                                        </p>
                                                                    </div>
                                                                </div>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </th>
                                            </tr>
                                        </tbody>
                                    </table>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                    <table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" class=""row row-9""
                        role=""presentation"" style=""mso-table-lspace:0;mso-table-rspace:0"" width=""100%"">
                        <tbody>
                            <tr>
                                <td>
                                    <table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0""
                                        class=""row-content stack"" role=""presentation""
                                        style=""mso-table-lspace:0;mso-table-rspace:0;background-color:#fbfbfb""
                                        width=""600"">
                                        <tbody>
                                            <tr>
                                                <th class=""column""
                                                    style=""mso-table-lspace:0;mso-table-rspace:0;font-weight:400;text-align:left;vertical-align:top""
                                                    width=""50%"">
                                                    <table border=""0"" cellpadding=""0"" cellspacing=""0"" class=""text_block""
                                                        role=""presentation""
                                                        style=""mso-table-lspace:0;mso-table-rspace:0;word-break:break-word""
                                                        width=""100%"">
                                                        <tr>
                                                            <td
                                                                style=""padding-top:15px;padding-right:10px;padding-bottom:15px;padding-left:10px"">
                                                                <div style=""font-family:sans-serif"">
                                                                    <div
                                                                        style=""font-size:14px;color:#555;line-height:1.2;font-family:Helvetica Neue,Helvetica,Arial,sans-serif"">
                                                                        <p style=""margin:0;font-size:14px"">Employee Type
                                                                        </p>
                                                                    </div>
                                                                </div>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </th>
                                                <th class=""column""
                                                    style=""mso-table-lspace:0;mso-table-rspace:0;font-weight:400;text-align:left;vertical-align:top""
                                                    width=""50%"">
                                                    <table border=""0"" cellpadding=""0"" cellspacing=""0"" class=""text_block""
                                                        role=""presentation""
                                                        style=""mso-table-lspace:0;mso-table-rspace:0;word-break:break-word""
                                                        width=""100%"">
                                                        <tr>
                                                            <td
                                                                style=""padding-top:15px;padding-right:10px;padding-bottom:15px;padding-left:10px"">
                                                                <div style=""font-family:sans-serif"">
                                                                    <div
                                                                        style=""font-size:14px;color:#555;line-height:1.2;font-family:Helvetica Neue,Helvetica,Arial,sans-serif"">
                                                                        <p style=""margin:0;font-size:14px"">" + employeeTypeName + @"
                                                                        </p>
                                                                    </div>
                                                                </div>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </th>
                                            </tr>
                                        </tbody>
                                    </table>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                    <table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" class=""row row-10""
                        role=""presentation"" style=""mso-table-lspace:0;mso-table-rspace:0"" width=""100%"">
                        <tbody>
                            <tr>
                                <td>
                                    <table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0""
                                        class=""row-content stack"" role=""presentation""
                                        style=""mso-table-lspace:0;mso-table-rspace:0;background-color:#fbfbfb""
                                        width=""600"">
                                        <tbody>
                                            <tr>
                                                <th class=""column""
                                                    style=""mso-table-lspace:0;mso-table-rspace:0;font-weight:400;text-align:left;vertical-align:top""
                                                    width=""50%"">
                                                    <table border=""0"" cellpadding=""0"" cellspacing=""0"" class=""text_block""
                                                        role=""presentation""
                                                        style=""mso-table-lspace:0;mso-table-rspace:0;word-break:break-word""
                                                        width=""100%"">
                                                        <tr>
                                                            <td
                                                                style=""padding-top:15px;padding-right:10px;padding-bottom:15px;padding-left:10px"">
                                                                <div style=""font-family:sans-serif"">
                                                                    <div
                                                                        style=""font-size:14px;color:#555;line-height:1.2;font-family:Helvetica Neue,Helvetica,Arial,sans-serif"">
                                                                        <p style=""margin:0;font-size:14px"">Agency Name
                                                                            (if applicable)</p>
                                                                    </div>
                                                                </div>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </th>
                                                <th class=""column""
                                                    style=""mso-table-lspace:0;mso-table-rspace:0;font-weight:400;text-align:left;vertical-align:top""
                                                    width=""50%"">
                                                    <table border=""0"" cellpadding=""0"" cellspacing=""0"" class=""text_block""
                                                        role=""presentation""
                                                        style=""mso-table-lspace:0;mso-table-rspace:0;word-break:break-word""
                                                        width=""100%"">
                                                        <tr>
                                                            <td
                                                                style=""padding-top:15px;padding-right:10px;padding-bottom:15px;padding-left:10px"">
                                                                <div style=""font-family:sans-serif"">
                                                                    <div
                                                                        style=""font-size:14px;color:#555;line-height:1.2;font-family:Helvetica Neue,Helvetica,Arial,sans-serif"">
                                                                        <p style=""margin:0;font-size:14px"">" + StaffRequest.AgencyName + @"
                                                                        </p>
                                                                    </div>
                                                                </div>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </th>
                                            </tr>
                                        </tbody>
                                    </table>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                    <table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" class=""row row-11""
                        role=""presentation"" style=""mso-table-lspace:0;mso-table-rspace:0"" width=""100%"">
                        <tbody>
                            <tr>
                                <td>
                                    <table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0""
                                        class=""row-content stack"" role=""presentation""
                                        style=""mso-table-lspace:0;mso-table-rspace:0;background-color:#fbfbfb""
                                        width=""600"">
                                        <tbody>
                                            <tr>
                                                <th class=""column""
                                                    style=""mso-table-lspace:0;mso-table-rspace:0;font-weight:400;text-align:left;vertical-align:top""
                                                    width=""50%"">
                                                    <table border=""0"" cellpadding=""0"" cellspacing=""0"" class=""text_block""
                                                        role=""presentation""
                                                        style=""mso-table-lspace:0;mso-table-rspace:0;word-break:break-word""
                                                        width=""100%"">
                                                        <tr>
                                                            <td
                                                                style=""padding-top:15px;padding-right:10px;padding-bottom:15px;padding-left:10px"">
                                                                <div style=""font-family:sans-serif"">
                                                                    <div
                                                                        style=""font-size:14px;color:#555;line-height:1.2;font-family:Helvetica Neue,Helvetica,Arial,sans-serif"">
                                                                        <p style=""margin:0;font-size:14px"">Phone
                                                                            Extension</p>
                                                                    </div>
                                                                </div>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </th>
                                                <th class=""column""
                                                    style=""mso-table-lspace:0;mso-table-rspace:0;font-weight:400;text-align:left;vertical-align:top""
                                                    width=""50%"">
                                                    <table border=""0"" cellpadding=""0"" cellspacing=""0"" class=""text_block""
                                                        role=""presentation""
                                                        style=""mso-table-lspace:0;mso-table-rspace:0;word-break:break-word""
                                                        width=""100%"">
                                                        <tr>
                                                            <td
                                                                style=""padding-top:15px;padding-right:10px;padding-bottom:15px;padding-left:10px"">
                                                                <div style=""font-family:sans-serif"">
                                                                    <div
                                                                        style=""font-size:14px;color:#555;line-height:1.2;font-family:Helvetica Neue,Helvetica,Arial,sans-serif"">
                                                                        <p style=""margin:0;font-size:14px"">
                                                                            " + StaffRequest.PhoneExtension + @"</p>
                                                                    </div>
                                                                </div>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </th>
                                            </tr>
                                        </tbody>
                                    </table>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                    <table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" class=""row row-12""
                        role=""presentation"" style=""mso-table-lspace:0;mso-table-rspace:0"" width=""100%"">
                        <tbody>
                            <tr>
                                <td>
                                    <table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0""
                                        class=""row-content stack"" role=""presentation""
                                        style=""mso-table-lspace:0;mso-table-rspace:0;background-color:#fbfbfb""
                                        width=""600"">
                                        <tbody>
                                            <tr>
                                                <th class=""column""
                                                    style=""mso-table-lspace:0;mso-table-rspace:0;font-weight:400;text-align:left;vertical-align:top""
                                                    width=""50%"">
                                                    <table border=""0"" cellpadding=""0"" cellspacing=""0"" class=""text_block""
                                                        role=""presentation""
                                                        style=""mso-table-lspace:0;mso-table-rspace:0;word-break:break-word""
                                                        width=""100%"">
                                                        <tr>
                                                            <td
                                                                style=""padding-top:15px;padding-right:10px;padding-bottom:15px;padding-left:10px"">
                                                                <div style=""font-family:sans-serif"">
                                                                    <div
                                                                        style=""font-size:14px;color:#555;line-height:1.2;font-family:Helvetica Neue,Helvetica,Arial,sans-serif"">
                                                                        <p style=""margin:0;font-size:14px"">Expected Job
                                                                            Title</p>
                                                                    </div>
                                                                </div>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </th>
                                                <th class=""column""
                                                    style=""mso-table-lspace:0;mso-table-rspace:0;font-weight:400;text-align:left;vertical-align:top""
                                                    width=""50%"">
                                                    <table border=""0"" cellpadding=""0"" cellspacing=""0"" class=""text_block""
                                                        role=""presentation""
                                                        style=""mso-table-lspace:0;mso-table-rspace:0;word-break:break-word""
                                                        width=""100%"">
                                                        <tr>
                                                            <td
                                                                style=""padding-top:15px;padding-right:10px;padding-bottom:15px;padding-left:10px"">
                                                                <div style=""font-family:sans-serif"">
                                                                    <div
                                                                        style=""font-size:14px;color:#555;line-height:1.2;font-family:Helvetica Neue,Helvetica,Arial,sans-serif"">
                                                                        <p style=""margin:0;font-size:14px"">
                                                                            " + StaffRequest.JobTitle + @"</p>
                                                                    </div>
                                                                </div>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </th>
                                            </tr>
                                        </tbody>
                                    </table>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                    <table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" class=""row row-13""
                        role=""presentation"" style=""mso-table-lspace:0;mso-table-rspace:0"" width=""100%"">
                        <tbody>
                            <tr>
                                <td>
                                    <table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0""
                                        class=""row-content stack"" role=""presentation""
                                        style=""mso-table-lspace:0;mso-table-rspace:0;background-color:#fbfbfb""
                                        width=""600"">
                                        <tbody>
                                            <tr>
                                                <th class=""column""
                                                    style=""mso-table-lspace:0;mso-table-rspace:0;font-weight:400;text-align:left;vertical-align:top""
                                                    width=""100%"">
                                                    <table border=""0"" cellpadding=""0"" cellspacing=""0""
                                                        class=""divider_block"" role=""presentation""
                                                        style=""mso-table-lspace:0;mso-table-rspace:0"" width=""100%"">
                                                        <tr>
                                                            <td
                                                                style=""padding-bottom:30px;padding-left:5px;padding-right:5px;padding-top:40px"">
                                                                <div align=""center"">
                                                                    <table border=""0"" cellpadding=""0"" cellspacing=""0""
                                                                        role=""presentation""
                                                                        style=""mso-table-lspace:0;mso-table-rspace:0""
                                                                        width=""10%"">
                                                                        <tr>
                                                                            <td class=""divider_inner""
                                                                                style=""font-size:1px;line-height:1px;border-top:4px dotted #009ad2"">
                                                                                <span></span></td>
                                                                        </tr>
                                                                    </table>
                                                                </div>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </th>
                                            </tr>
                                        </tbody>
                                    </table>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                    <table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" class=""row row-14""
                        role=""presentation"" style=""mso-table-lspace:0;mso-table-rspace:0;background-position:center top""
                        width=""100%"">
                        <tbody>
                            <tr>
                                <td>
                                    <table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0""
                                        class=""row-content stack"" role=""presentation""
                                        style=""mso-table-lspace:0;mso-table-rspace:0;background-color:#212529"" width=""600"">
                                        <tbody>
                                            <tr>
                                                <th class=""column""
                                                    style=""mso-table-lspace:0;mso-table-rspace:0;font-weight:400;text-align:left;vertical-align:top;border-bottom:0 solid #d4f9ef;border-left:0 solid #d4f9ef;border-right:0 solid #d4f9ef;border-top:0 solid #d4f9ef;padding-left:25px;padding-right:25px""
                                                    width=""100%"">
                                                    <table border=""0"" cellpadding=""0"" cellspacing=""0""
                                                        class=""divider_block"" role=""presentation""
                                                        style=""mso-table-lspace:0;mso-table-rspace:0"" width=""100%"">
                                                        <tr>
                                                            <td
                                                                style=""padding-bottom:25px;padding-left:10px;padding-right:10px;padding-top:20px"">
                                                                <div align=""center"">
                                                                    <table border=""0"" cellpadding=""0"" cellspacing=""0""
                                                                        role=""presentation""
                                                                        style=""mso-table-lspace:0;mso-table-rspace:0""
                                                                        width=""100%"">
                                                                        <tr>
                                                                            <td class=""divider_inner""
                                                                                style=""font-size:1px;line-height:1px;border-top:2px solid #fff"">
                                                                                <span></span></td>
                                                                        </tr>
                                                                    </table>
                                                                </div>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                    <table border=""0"" cellpadding=""0"" cellspacing=""0"" class=""text_block""
                                                        role=""presentation""
                                                        style=""mso-table-lspace:0;mso-table-rspace:0;word-break:break-word""
                                                        width=""100%"">
                                                        <tr>
                                                            <td
                                                                style=""padding-bottom:35px;padding-left:10px;padding-right:10px;padding-top:10px"">
                                                                <div style=""font-family:sans-serif"">
                                                                    <div
                                                                        style=""font-size:12px;color:#66787f;line-height:1.2;font-family:Helvetica Neue,Helvetica,Arial,sans-serif"">
                                                                        <p
                                                                            style=""margin:0;font-size:14px;text-align:center"">
                                                                            West London College &copy; 2021</p>
                                                                    </div>
                                                                </div>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </th>
                                            </tr>
                                        </tbody>
                                    </table>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                    <table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" class=""row row-15""
                        role=""presentation"" style=""mso-table-lspace:0;mso-table-rspace:0"" width=""100%"">
                        <tbody>
                            <tr>
                                <td>
                                    
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </td>
            </tr>
        </tbody>
    </table><!-- End -->
</body>

</html>";

            try
            {
                await _emailSender.SendEmailAsync(
                emailTo,
                emailSubject,
                emailMsg);

                RequestSuccessful = true;
            }
            catch (Exception ex)
            {
                RequestSuccessful = false;
                ErrorMessage = ex.Message;

                SubmitResult.IsSuccessful = false;
                SubmitResult.ErrorLevel = ErrorLevel.Error;
                SubmitResult.ErrorCode = "EMAIL_FAIL";
                SubmitResult.ErrorDescription = $"Unfortunately, details for {StaffRequest.Forename} {StaffRequest.Surname} could not be sent to HR.<br />Please try again and if the problem persists please contact IT Services.";
                return new JsonResult(SubmitResult);
            }

            //return Page();

            SubmitResult.IsSuccessful = true;
            return new JsonResult(SubmitResult);

            //if (!ModelState.IsValid)
            //{
            //    return Page();
            //}

            //_context.StaffRequest_1.Add(StaffRequest);
            //await _context.SaveChangesAsync();

            //return RedirectToPage("./Index");
        }
    }
}
