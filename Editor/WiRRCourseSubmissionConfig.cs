#if UNITY_EDITOR
// Public identifier, NOT a GitHub credential or authentication secret.
// The server must independently validate and rate-limit every request.
internal static class WiRRCourseSubmissionConfig
{
    internal const string CourseCode = "WIRR";
    internal const string PublicCourseKey = "WIRR-2026-2027-PUBLIC-SUBMISSION-V1";
    // The endpoint must be deployed separately; do not fake a successful upload.
    internal const string SubmissionEndpoint = "";
}
#endif
