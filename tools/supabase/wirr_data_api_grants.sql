-- WiRR Data API privileges. Apply alongside the corresponding CREATE TABLE/VIEW
-- in the owning Supabase migrations, not as an automatic production migration.
-- All objects must already exist. RLS policies are required separately.
-- Never grant access to anon for student submissions or grades.
BEGIN;
GRANT USAGE ON SCHEMA public TO authenticated;
GRANT SELECT ON TABLE public.courses TO authenticated;
GRANT SELECT ON TABLE public.enrollments TO authenticated;
GRANT SELECT, INSERT ON TABLE public.wirr_submissions TO authenticated;
GRANT SELECT, INSERT, UPDATE ON TABLE public.wirr_grades TO authenticated;
GRANT SELECT ON TABLE public.wirr_instructor_overview TO authenticated;
COMMIT;
