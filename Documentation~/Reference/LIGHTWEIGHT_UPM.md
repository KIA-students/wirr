# WiRR lightweight UPM channels

The student-facing repository is exclusively `KIA-students/wirr`.

- Core: `https://github.com/KIA-students/wirr.git#upm`.
- Optional HDRI/PBR: `https://github.com/KIA-students/wirr.git#assets`.

The optional package exposes a Unity Package Manager sample named **HDRI PBR**. After installing the optional package, explicitly click **Import** in its Samples panel. Merely adding the optional package does not import heavy textures into Assets.\n\nThe lightweight core branch excludes `Project~`, `Textures/HDRI`, and
`Textures/Surfaces`; the optional branch contains those two asset trees and
an independent `package.json` and importable `Samples~/HDRI-PBR` (`pl.prz.kia.wirr.assets`).

Run `python3 scripts/publish_upm_channels.py --output /tmp/wirr-dist` to
inspect snapshots. Publishing is deliberately manual and uses the protected
GitHub Actions environment `student-package-publishing` plus secret
`KIA_STUDENTS_PUBLISH_TOKEN` with contents write access to
`KIA-students/wirr`. Only orphan `upm` and `assets` refs are overwritten;
student `main` must never be touched by this workflow.

Do not advertise the `#upm` and `#assets` URLs before publication,
independent clean-project installs, and Unity 6000.6.5f1 smoke tests.
No branch publication changes an existing Unity package lockfile automatically.
