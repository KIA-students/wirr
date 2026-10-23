-- WiRR Data API privileges. Apply alongside the corresponding CREATE TABLE/VIEW
-- in the owning Supabase migrations, not as an automatic production migration.
-- All objects must already exist. RLS policies are required separately.
-- Never grant access to anon for student submissions or grades.
BEGIN;
GRANT USAGE ON SCHEMA public TO authenticated;
GRANT SELECT ON TABLE public.courses TO authenticated;
GRANT SELECT ON TABLE public.enrollments TO authenticated;
GRANT SELECT, INSERT ON TABLE public.wirr_submissions TO authenticated;
GRANT SELECT, INSERT ON TABLE public.wirr_grades TO authenticated;
GRANT UPDATE (grade, graded_by, graded_at) ON TABLE public.wirr_grades TO authenticated;
GRANT SELECT ON TABLE public.wirr_instructor_overview TO authenticated;
-- Edge intake v5: count, idempotent insert and confirmation read.
-- No browser access to this queue; exporter privileges need a separate audit.
GRANT USAGE ON SCHEMA public TO service_role;
GRANT SELECT, INSERT ON TABLE public.wirr_public_intake TO service_role;
COMMIT;
