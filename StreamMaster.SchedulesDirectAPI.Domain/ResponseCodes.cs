namespace StreamMaster.SchedulesDirectAPI.Domain;

public enum SDHttpResponseCode
{
    OK = 0,
    INVALID_JSON = 1001,
    DEFLATE_REQUIRED = 1002,
    USERAGENT_REQUIRED = 1003,
    TOKEN_MISSING = 1004,
    TOKEN_INVALID = 1010,
    UNSUPPORTED_COMMAND = 2000,
    REQUIRED_ACTION_MISSING = 2001,
    REQUIRED_REQUEST_MISSING = 2002,
    REQUIRED_PARAMETER_MISSING_COUNTRY = 2004,
    REQUIRED_PARAMETER_MISSING_POSTALCODE = 2005,
    REQUIRED_PARAMETER_MISSING_MSGID = 2006,
    INVALID_PARAMETER_COUNTRY = 2050,
    INVALID_PARAMETER_POSTALCODE = 2051,
    INVALID_PARAMETER_FETCHTYPE = 2052,
    DUPLICATE_LINEUP = 2100,
    LINEUP_NOT_FOUND = 2101,
    UNKNOWN_LINEUP = 2102,
    INVALID_LINEUP_DELETE = 2103,
    LINEUP_WRONG_FORMAT = 2104,
    INVALID_LINEUP = 2105,
    LINEUP_DELETED = 2106,
    LINEUP_QUEUED = 2107,
    INVALID_COUNTRY = 2108,
    STATIONID_NOT_FOUND = 2200,
    SERVICE_OFFLINE = 3000,
    ACCOUNT_EXPIRED = 4001,
    INVALID_HASH = 4002,
    INVALID_USER = 4003,
    ACCOUNT_LOCKOUT = 4004,
    ACCOUNT_DISABLED = 4005,
    TOKEN_EXPIRED = 4006,
    APPLICATION_DISABLED = 4007,
    TOKEN_DUPLICATED = 4008,
    MAX_LINEUP_CHANGES_REACHED = 4100,
    MAX_LINEUPS = 4101,
    NO_LINEUPS = 4102,
    IMAGE_NOT_FOUND = 5000,
    IMAGE_QUEUED = 5001,
    MAX_IMAGE_DOWNLOADS = 5002,
    MAX_IMAGE_DOWNLOADS_TRIAL = 5003,
    UNKNOWN_USER = 5004,
    INVALID_PROGRAMID = 6000,
    PROGRAMID_QUEUED = 6001,
    SCHEDULE_NOT_FOUND = 7000,
    INVALID_SCHEDULE_REQUEST = 7010,
    SCHEDULE_RANGE_EXCEEDED = 7020,
    SCHEDULE_NOT_IN_LINEUP = 7030,
    SCHEDULE_QUEUED = 7100,
    UNKNOWN_ERROR = 9999
}

public static class SDHttpResponseCodeExtensions
{
    public static string GetMessage(this SDHttpResponseCode responseCode)
    {
        switch (responseCode)
        {
            case SDHttpResponseCode.OK:
                return "OK";

            case SDHttpResponseCode.INVALID_JSON:
                return "Unable to decode JSON";

            case SDHttpResponseCode.USERAGENT_REQUIRED:
                return "USERAGENT missing from request";

            case SDHttpResponseCode.DEFLATE_REQUIRED:
                return "Did not receive Accept-Encoding: deflate in request.";

            case SDHttpResponseCode.TOKEN_MISSING:
                return "Token required but not provided in request header.";

            case SDHttpResponseCode.UNSUPPORTED_COMMAND:
                return "Unsupported command";

            case SDHttpResponseCode.REQUIRED_ACTION_MISSING:
                return "Request is missing an action to take.";

            case SDHttpResponseCode.REQUIRED_REQUEST_MISSING:
                return "Did not receive request.";

            case SDHttpResponseCode.REQUIRED_PARAMETER_MISSING_COUNTRY:
                return "In order to search for lineups, you must supply a 3-letter country parameter.";

            case SDHttpResponseCode.REQUIRED_PARAMETER_MISSING_POSTALCODE:
                return "In order to search for lineups, you must supply a postal code parameter.";

            case SDHttpResponseCode.REQUIRED_PARAMETER_MISSING_MSGID:
                return "In order to delete a message, you must supply the messageID.";

            case SDHttpResponseCode.INVALID_PARAMETER_COUNTRY:
                return "The COUNTRY parameter must be ISO-3166-1 alpha 3. See http://en.wikipedia.org/wiki/ISO_3166-1_alpha-3";

            case SDHttpResponseCode.INVALID_PARAMETER_POSTALCODE:
                return "The POSTALCODE parameter must be valid for the country you are searching. Post message to http://forums.schedulesdirect.org/viewforum.php?f=6 if you are having issues.";

            case SDHttpResponseCode.INVALID_PARAMETER_FETCHTYPE:
                return "You didn't provide a fetchtype I know how to handle.";

            case SDHttpResponseCode.DUPLICATE_LINEUP:
                return "Lineup already in account.";

            case SDHttpResponseCode.LINEUP_NOT_FOUND:
                return "Lineup not in account. Add lineup to account before requesting mapping.";

            case SDHttpResponseCode.UNKNOWN_LINEUP:
                return "Invalid lineup requested. Check your COUNTRY / POSTALCODE combination for validity.";

            case SDHttpResponseCode.INVALID_LINEUP_DELETE:
                return "Delete of lineup not in account.";

            case SDHttpResponseCode.LINEUP_WRONG_FORMAT:
                return "Lineup must be formatted COUNTRY-LINEUP-DEVICE or COUNTRY-OTA-POSTALCODE";

            case SDHttpResponseCode.INVALID_LINEUP:
                return "The lineup you submitted doesn't exist.";

            case SDHttpResponseCode.LINEUP_DELETED:
                return "The lineup you requested has been deleted from the server.";

            case SDHttpResponseCode.LINEUP_QUEUED:
                return "The lineup is being generated on the server. Please retry.";

            case SDHttpResponseCode.INVALID_COUNTRY:
                return "The country you requested is either mis-typed or does not have valid data.";

            case SDHttpResponseCode.STATIONID_NOT_FOUND:
                return "The stationID you requested is not in any of your lineups.";

            case SDHttpResponseCode.SERVICE_OFFLINE:
                return "Server offline for maintenance.";

            case SDHttpResponseCode.ACCOUNT_EXPIRED:
                return "Account expired.";

            case SDHttpResponseCode.INVALID_HASH:
                return "Password hash must be lowercase 40 character sha1_hex of password.";

            case SDHttpResponseCode.INVALID_USER:
                return "Invalid username or password.";

            case SDHttpResponseCode.ACCOUNT_LOCKOUT:
                return "Too many login failures. Locked for 15 minutes.";

            case SDHttpResponseCode.ACCOUNT_DISABLED:
                return "Account has been disabled. Please contact Schedules Direct support: admin@schedulesdirect.org for more information.";

            case SDHttpResponseCode.TOKEN_EXPIRED:
                return "Token has expired. Request new token.";

            case SDHttpResponseCode.MAX_LINEUP_CHANGES_REACHED:
                return "Exceeded maximum number of lineup changes for today.";

            case SDHttpResponseCode.MAX_LINEUPS:
                return "Exceeded number of lineups for this account.";

            case SDHttpResponseCode.NO_LINEUPS:
                return "No lineups have been added to this account.";

            case SDHttpResponseCode.IMAGE_NOT_FOUND:
                return "Could not find requested image. Post message to http://forums.schedulesdirect.org/viewforum.php?f=6 if you are having issues.";

            case SDHttpResponseCode.INVALID_PROGRAMID:
                return "Could not find requested programID. Permanent failure.";

            case SDHttpResponseCode.PROGRAMID_QUEUED:
                return "ProgramID should exist at the server, but doesn't. The server will regenerate the JSON for the program, so your application should retry.";

            case SDHttpResponseCode.SCHEDULE_NOT_FOUND:
                return "The schedule you requested should be available. Post message to http://forums.schedulesdirect.org/viewforum.php?f=6";

            case SDHttpResponseCode.INVALID_SCHEDULE_REQUEST:
                return "The server can't determine whether your schedule is valid or not. Open a support ticket.";

            case SDHttpResponseCode.SCHEDULE_RANGE_EXCEEDED:
                return "The date that you've requested is outside of the range of the data for that stationID.";

            case SDHttpResponseCode.SCHEDULE_NOT_IN_LINEUP:
                return "You have requested a schedule which is not in any of your configured lineups.";

            case SDHttpResponseCode.SCHEDULE_QUEUED:
                return "The schedule you requested has been queued for generation but is not yet ready for download. Retry.";

            case SDHttpResponseCode.UNKNOWN_ERROR:
                return "Unknown error. Open support ticket.";

            default:
                return "Unknown error.";
        }
    }
}